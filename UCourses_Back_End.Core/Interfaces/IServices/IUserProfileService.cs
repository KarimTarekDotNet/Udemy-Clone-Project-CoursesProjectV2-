using UCourses_Back_End.Core.DTOs;
using UCourses_Back_End.Core.DTOs.AuthDTOs;

namespace UCourses_Back_End.Core.Interfaces.IServices
{
    public interface IUserProfileService
    {
        Task<UserProfileDTO?> GetUserProfileAsync(string userId);
        Task<bool> UpdateUserProfileAsync(string userId, UpdateProfileDTO dto);
        Task<bool> DeleteProfileImageAsync(string userId);
    }
}
