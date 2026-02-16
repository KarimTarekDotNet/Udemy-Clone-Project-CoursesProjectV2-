using UCourses_Back_End.Core.DTOs.DashboardDTOs;

namespace UCourses_Back_End.Core.Interfaces.IRepositories
{
    public interface IAdminRepository
    {
        Task<AdminStatisticsDTO> GetStatisticsAsync();
        Task<IEnumerable<RecentActivityDTO>> GetRecentActivitiesAsync(int count = 10);
        Task<IEnumerable<AllUsersDTO>> GetAllUsersAsync();
    }
}
