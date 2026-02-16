using UCourses_Back_End.Core.DTOs.DashboardDTOs;

namespace UCourses_Back_End.Core.Interfaces.IRepositories
{
    public interface ICourseProgressRepository
    {
        Task<bool> UpdateWatchedDurationAsync(string enrollmentPublicId, string sectionPublicId, int duration);
        Task<bool> MarkSectionCompletedAsync(string enrollmentPublicId, string sectionPublicId);
        Task<CourseProgressSummaryDTO?> GetCourseProgressAsync(string enrollmentPublicId);
    }
}
