using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using UCourses_Back_End.Core.DTOs.CoreDTOs;
using UCourses_Back_End.Core.Entites.CoreModels;
using UCourses_Back_End.Core.Interfaces.IRepositories;
using UCourses_Back_End.Core.Interfaces.IServices;
using UCourses_Back_End.Infrastructure.Services.PaymentService;

namespace UCourses_Back_End.Api.Controllers.UsersController
{
    public class PaymentController : BaseController
    {
    private readonly IConfiguration _configuration;
        public PaymentController(IUnitOfWork work, IConfiguration configuration, IAuditLogService? auditLog = null) : base(work, auditLog)
        {
            _configuration = configuration;
        }

        [Authorize]
        [HttpPost("create-payment-token")]
        public async Task<IActionResult> CreatePaymentToken([FromQuery] string courseId, [FromQuery] string paymentMethod)
        {
            // Get the current user's PublicId from JWT claims
            var studentPublicId = HttpContext.User.FindFirst("PublicId")?.Value;
            if (string.IsNullOrEmpty(studentPublicId))
                return Unauthorized("User not authenticated or PublicId not found in token.");

            // Validate inputs
            if (string.IsNullOrWhiteSpace(courseId))
                return BadRequest("CourseId is required.");

            if (string.IsNullOrWhiteSpace(paymentMethod))
                return BadRequest("Payment method is required.");

            // Check if course exists
            var courseExists = await work.CourseRepository.ExistsAsync(courseId);
            if (!courseExists)
                return NotFound($"Course with ID '{courseId}' not found.");

            // Check if already enrolled
            var alreadyEnrolled = await work.EnrollmentRepository.IsStudentEnrolledAsync(studentPublicId, courseId);
            
            Enrollment? enrollment;

            if (alreadyEnrolled)
            {
                // Get existing enrollment
                var enrollments = await work.EnrollmentRepository.GetStudentEnrollmentsAsync(studentPublicId);
                if (enrollments == null)
                    return NotFound("Student enrollments not found.");

                // Find the specific enrollment for this course
                var existingEnrollment = await work.EnrollmentRepository.GetByStudentAndCourseAsync(studentPublicId, courseId);
                if (existingEnrollment == null)
                    return NotFound("Enrollment not found.");

                enrollment = existingEnrollment;
            }
            else
            {
                // Create new enrollment
                var enrollmentDTO = new CreateEnrollmentDTO
                {
                    CourseId = courseId,
                    StudentId = studentPublicId
                };

                enrollment = await work.EnrollmentRepository.CreateAsync(enrollmentDTO);
                
                if (enrollment == null)
                    return StatusCode(500, "Failed to create enrollment. Please try again.");
            }

            try
            {
                var amount = enrollment.Course.Price;
                decimal totalAmount = amount;

                if (paymentMethod.Equals("card", StringComparison.OrdinalIgnoreCase))
                {
                    var (enrollmentResult, redirectUrl) = 
                        await work.PaymobService.ProcessPaymentAsync(enrollment.Id, paymentMethod, totalAmount);
                    return Ok(new { RedirectUrl = redirectUrl });
                }
                else
                {
                    return BadRequest("Invalid payment method. Supported methods are 'card'.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error processing payment: {ex.Message}");
            }
        }

        [HttpGet("callback")]
        public async Task<IActionResult> CallbackAsync()
        {
            var query = Request.Query;

            string[] fields = new[]
            {
                "amount_cents", "created_at", "currency", "error_occured", "has_parent_transaction",
                "id", "integration_id", "is_3d_secure", "is_auth", "is_capture", "is_refunded",
                "is_standalone_payment", "is_voided", "order", "owner", "pending",
                "source_data.pan", "source_data.sub_type", "source_data.type", "success"
            };

            var concatenated = new StringBuilder();
            foreach (var field in fields)
            {
                if (query.TryGetValue(field, out var value))
                {
                    concatenated.Append(value);
                }
                else
                {
                    return BadRequest($"Missing expected field: {field}");
                }
            }

            string receivedHmac = query["hmac"]!;
            string calculatedHmac = work.PaymobService.ComputeHmacSHA512(concatenated.ToString(), _configuration["Paymob:HMAC"]!);

            if (receivedHmac.Equals(calculatedHmac, StringComparison.OrdinalIgnoreCase))
            {
                bool.TryParse(query["success"], out bool isSuccess);
                var specialReference = query["merchant_order_id"];

                if (isSuccess)
                {
                    return Ok();
                }
            }

            return BadRequest();
        }

        [HttpPost("server-callback")]
        public async Task<IActionResult> ServerCallback([FromBody] JsonElement payload)
        {
            try
            {
                string receivedHmac = Request.Query["hmac"]!;
                string secret = _configuration["Paymob:HMAC"]!;

                if (!payload.TryGetProperty("obj", out var obj))
                    return BadRequest("Missing 'obj' in payload.");

                string[] fields = new[]
                {
                    "amount_cents", "created_at", "currency", "error_occured", "has_parent_transaction",
                    "id", "integration_id", "is_3d_secure", "is_auth", "is_capture", "is_refunded",
                    "is_standalone_payment", "is_voided", "order.id", "owner", "pending",
                    "source_data.pan", "source_data.sub_type", "source_data.type", "success"
                };

                var concatenated = new StringBuilder();
                foreach (var field in fields)
                {
                    string[] parts = field.Split('.');
                    JsonElement current = obj;
                    bool found = true;
                    foreach (var part in parts)
                    {
                        if (current.ValueKind == JsonValueKind.Object && current.TryGetProperty(part, out var next))
                            current = next;
                        else
                        {
                            found = false;
                            break;
                        }
                    }

                    if (!found || current.ValueKind == JsonValueKind.Null)
                    {
                        concatenated.Append(""); // Use empty string for missing/null fields
                    }
                    else if (current.ValueKind == JsonValueKind.True || current.ValueKind == JsonValueKind.False)
                    {
                        concatenated.Append(current.GetBoolean() ? "true" : "false"); // Lowercase boolean
                    }
                    else
                    {
                        concatenated.Append(current.ToString());
                    }
                }

                string calculatedHmac = work.PaymobService.ComputeHmacSHA512(concatenated.ToString(), secret);

                if (!receivedHmac.Equals(calculatedHmac, StringComparison.OrdinalIgnoreCase))
                    return Unauthorized("Invalid HMAC");

                string merchantOrderId = null!;
                if (obj.TryGetProperty("order", out var order) &&
                    order.TryGetProperty("merchant_order_id", out var merchantOrderIdElement) &&
                    merchantOrderIdElement.ValueKind != JsonValueKind.Null)
                {
                    merchantOrderId = merchantOrderIdElement.ToString();
                }

                bool isSuccess = obj.TryGetProperty("success", out var successElement) && successElement.GetBoolean();

                if (!string.IsNullOrEmpty(merchantOrderId))
                {
                    if (isSuccess)
                        await work.PaymobService.UpdateOrderSuccess(merchantOrderId);
                    else
                        await work.PaymobService.UpdateOrderFailed(merchantOrderId);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error processing server callback: {ex.Message}");
            }
        }
    }
}