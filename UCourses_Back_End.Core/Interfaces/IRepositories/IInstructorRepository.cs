using UCourses_Back_End.Core.DTOs.DashboardDTOs;
using UCourses_Back_End.Core.Entites.Users;

namespace UCourses_Back_End.Core.Interfaces.IRepositories
{
    public interface IInstructorRepository
    {
        Task<IEnumerable<InstructorDTO>> GetAllAsync();
        Task<IEnumerable<InstructorDTO>> GetPendingInstructorsAsync();
        Task<InstructorDetailsDTO?> GetByIdAsync(string publicId);
        Task<Instructor?> GetByUserIdAsync(string userId);
        Task<bool> ApproveInstructorAsync(string publicId);
        Task<bool> RejectInstructorAsync(string publicId, string rejectionReason);
        Task<bool> ExistsAsync(string publicId);
        Task<InstructorEarningsDTO?> GetInstructorEarningsAsync(string instructorPublicId);
        Task<CourseAnalyticsDTO?> GetCourseAnalyticsAsync(string coursePublicId);
    }
}
