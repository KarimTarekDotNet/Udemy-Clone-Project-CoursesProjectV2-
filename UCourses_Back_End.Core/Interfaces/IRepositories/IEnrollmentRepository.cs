using UCourses_Back_End.Core.DTOs.CoreDTOs;
using UCourses_Back_End.Core.Entites.CoreModels;

namespace UCourses_Back_End.Core.Interfaces.IRepositories
{
    public interface IEnrollmentRepository
    {
        Task<IEnumerable<EnrollmentDTO>> GetAllAsync();
        Task<EnrollmentDetailsDTO?> GetByIdAsync(string publicId);
        Task<StudentEnrollmentsDTO?> GetStudentEnrollmentsAsync(string studentPublicId);
        Task<CourseEnrollmentsDTO?> GetCourseEnrollmentsAsync(string coursePublicId);
        Task<Enrollment?> CreateAsync(CreateEnrollmentDTO dto);
        Task<bool> DeleteAsync(string publicId);
        Task<bool> IsStudentEnrolledAsync(string studentPublicId, string coursePublicId);
        Task<bool> ExistsAsync(string publicId);
        Task<Enrollment?> GetByStudentAndCourseAsync(string studentPublicId, string coursePublicId);
    }
}
