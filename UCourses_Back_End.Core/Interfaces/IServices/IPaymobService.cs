using UCourses_Back_End.Core.DTOs.CoreDTOs;
using UCourses_Back_End.Core.Entites.CoreModels;

namespace UCourses_Back_End.Core.Interfaces.IServices
{
    public interface IPaymobService
    {
        Task<(EnrollmentDetailsDTO Enrollment, string RedirectUrl)> ProcessPaymentAsync(Guid enrollmentId, string paymentMethod, decimal amount);
        Task<Enrollment> UpdateOrderSuccess(string specialReference);
        Task<Enrollment> UpdateOrderFailed(string specialReference);
        string ComputeHmacSHA512(string data, string secret);
    }
}
