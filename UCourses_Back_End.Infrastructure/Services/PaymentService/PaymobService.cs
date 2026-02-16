using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using UCourses_Back_End.Core.DTOs.CoreDTOs;
using UCourses_Back_End.Core.Entites.CoreModels;
using UCourses_Back_End.Core.Enums.CoreEnum;
using UCourses_Back_End.Core.Interfaces.IServices;
using UCourses_Back_End.Infrastructure.Data;

namespace UCourses_Back_End.Infrastructure.Services.PaymentService
{
    public class PaymobService : IPaymobService
    {

        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly ILogger<PaymobService> _logger;
        private readonly ApplicationDbContext _context;

        public PaymobService(IConfiguration configuration, ApplicationDbContext context, IMapper mapper, ILogger<PaymobService> logger)
        {
            _configuration = configuration;
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<(EnrollmentDetailsDTO Enrollment, string RedirectUrl)> ProcessPaymentAsync(Guid enrollmentId, string paymentMethod, decimal amount)
        {
            var enrollment = await _context.Enrollments
                .Include(x =>x.Course)
                .Include(x => x.Student)
                    .ThenInclude(s => s.AppUser)
                .FirstOrDefaultAsync(e => e.Id == enrollmentId);

            if (enrollment == null)
            {
                throw new KeyNotFoundException($"Enrollment with ID {enrollmentId} not found.");
            }

            string apiKey = _configuration["Paymob:APIKey"];
            
            if (string.IsNullOrEmpty(apiKey))
            {
                // Try alternative casing
                apiKey = _configuration["Paymob:APIkey"] ?? _configuration["Paymob:ApiKey"];
            }
            
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentException("Paymob API key not configured");

            string secretKey = _configuration["Paymob:SecretKey"] ??
                throw new ArgumentException("Paymob secret key not configured");

            string publicKey = _configuration["Paymob:PublicKey"] ??
                throw new ArgumentException("Paymob public key not configured");

            var amountCents = (int)(amount * 100);
            var billingData = new
            {
                apartment = "N/A",
                first_name = enrollment.Student.AppUser.FirstName ?? "Guest",
                last_name = enrollment.Student.AppUser.LastName ?? "User",
                street = "N/A",
                building = "N/A",
                phone_number = string.IsNullOrWhiteSpace(enrollment.Student.AppUser.PhoneNumber) 
                    ? "+201000000000" 
                    : enrollment.Student.AppUser.PhoneNumber,
                country = "EG",
                email = enrollment.Student.AppUser.Email ?? "guest@example.com",
                floor = "N/A",
                state = "N/A",
                city = "Cairo"
            };
            var integrationId = int.Parse(DetermineIntegrationId(paymentMethod));

            var payload = new
            {
                amount = amountCents,
                currency = "EGP",
                payment_methods = new[] { integrationId },
                billing_data = billingData,
                items = new[]
               {
                    new
                    {
                        name = $"Enrollment #{enrollment.PublicId}",
                        amount = amountCents,
                        description = $"Course Enrollment Payment for course #{enrollment.Course.Name}",
                        quantity = 1
                    }
                },
                customer = new
                {
                    first_name = billingData.first_name,
                    last_name = billingData.last_name,
                    email = billingData.email,
                    extras = new { enrollmentId = enrollment.Id }
                },
                extras = new
                {
                    enrollmentId = enrollment.Id,
                    customerId = enrollment.Student.Id
                },
                special_reference = enrollment.PublicId,
                expiration = 3600, // 1 hour expiration
                merchant_order_id = enrollment.PublicId
            };

            // create request
            using var httpClient = new HttpClient();
            var requestMessage = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, "https://accept.paymob.com/v1/intention/");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Token", secretKey);
            requestMessage.Content = JsonContent.Create(payload);

            // Send the request and process response
            var response = await httpClient.SendAsync(requestMessage);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Paymob API Error: {responseContent}");
                throw new Exception($"Paymob Intention API call failed with status {response.StatusCode}: {responseContent}");
            }

            // Parse the response to get client_secret
            var resultJson = JsonDocument.Parse(responseContent);
            var clientSecret = resultJson.RootElement.GetProperty("client_secret").GetString();

            var payment = new Payment
            {
                Amount = enrollment.Course.Price,
                PaymentMethod = paymentMethod,
                Status = PaymentStatus.Pending,
                TransactionId = enrollment.PublicId,
                EnrollmentId = enrollment.Id,
                PaymentDate = DateTime.UtcNow,
                StudentId = enrollment.Student.Id,
            };

            _context.Payments.Add(payment);
            enrollment.Status = PaymentStatus.Pending;
            await _context.SaveChangesAsync();

            // Generate payment URL for the unified checkout
            string redirectUrl = $"https://accept.paymob.com/unifiedcheckout/?publicKey={publicKey}&clientSecret={clientSecret}";

            var Dto = _mapper.Map<EnrollmentDetailsDTO>(enrollment);
            return (Dto, redirectUrl);

        }
        private string DetermineIntegrationId(string paymentMethod)
        {
            return paymentMethod?.ToLower() switch
            {
                "card" => _configuration["Paymob:CardIntegrationId"] ?? throw new ArgumentException("Card integration ID not configured"),
                _ => throw new ArgumentException($"Invalid payment method: {paymentMethod}. Only 'card' is supported.")
            };
        }
        public async Task<Enrollment> UpdateOrderFailed(string specialReference)
        {
            var payment = await _context.Payments
                           .Include(p => p.Enrollment)
                           .FirstOrDefaultAsync(p => p.TransactionId == specialReference);

            if (payment == null)
            {
                throw new KeyNotFoundException($"Payment with transaction ID {specialReference} not found.");
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Student)
                .FirstOrDefaultAsync(e => e.Id == payment.EnrollmentId);

            if (enrollment == null)
            {
                throw new KeyNotFoundException($"Enrollment with ID {payment.EnrollmentId} not found.");
            }

            // Update enrollment status and payment status
            enrollment.Status = PaymentStatus.Failed;
            payment.Status = PaymentStatus.Failed;

            await _context.SaveChangesAsync();

            return payment.Enrollment;
        }

        public async Task<Enrollment> UpdateOrderSuccess(string specialReference)
        {
            var payment = await _context.Payments
                            .Include(p => p.Enrollment)
                            .FirstOrDefaultAsync(p => p.TransactionId == specialReference);

            if (payment == null)
            {
                throw new KeyNotFoundException($"Payment with transaction ID {specialReference} not found.");
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == payment.EnrollmentId);

            if (enrollment == null)
            {
                throw new KeyNotFoundException($"Enrollment with ID {payment.EnrollmentId} not found.");
            }

            enrollment.Status = PaymentStatus.Captured;
            payment.Status = PaymentStatus.Captured;

            await _context.SaveChangesAsync();

            return payment.Enrollment;

        }
        public string ComputeHmacSHA512(string data, string secret)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var dataBytes = Encoding.UTF8.GetBytes(data);

            using (var hmac = new HMACSHA512(keyBytes))
            {
                var hash = hmac.ComputeHash(dataBytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}