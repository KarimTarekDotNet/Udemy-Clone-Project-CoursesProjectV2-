using UCourses_Back_End.Core.DTOs.CoreDTOs;
using UCourses_Back_End.Core.Entites.CoreModels;
using UCourses_Back_End.Core.ModelsView;

namespace UCourses_Back_End.Core.Interfaces.IRepositories
{
    public interface ICourseRepository
    {
        Task<(IEnumerable<CourseDTO> courses, int totalCount)> GetAllAsync(QueryParams queryParams);
        Task<(IEnumerable<CourseDTO> courses, int totalCount)> GetPublishedCoursesAsync(QueryParams queryParams);
        Task<(IEnumerable<CourseDTO> courses, int totalCount)> GetByDepartmentAsync(string departmentPublicId, QueryParams queryParams);
        Task<(IEnumerable<CourseDTO> courses, int totalCount)> GetByInstructorAsync(string instructorPublicId, QueryParams queryParams);
        Task<CourseDetailsDTO?> GetByIdAsync(string publicId);
        Task<Course?> CreateAsync(CreateCourseDTO dto);
        Task<bool> UpdateAsync(string publicId, UpdateCourseDTO dto);
        Task<bool> DeleteAsync(string publicId);
        Task<bool> ExistsAsync(string publicId);
        Task<bool> PublishAsync(string publicId);
        Task<bool> UnpublishAsync(string publicId);
        Task<bool> ArchiveAsync(string publicId);
    }
}
