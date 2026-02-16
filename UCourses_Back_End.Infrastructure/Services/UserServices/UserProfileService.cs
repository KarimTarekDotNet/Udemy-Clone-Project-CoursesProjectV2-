using Microsoft.AspNetCore.Identity;
using UCourses_Back_End.Core.DTOs;
using UCourses_Back_End.Core.DTOs.AuthDTOs;
using UCourses_Back_End.Core.Entites.AuthModel;
using UCourses_Back_End.Core.Interfaces.IServices;

namespace UCourses_Back_End.Infrastructure.Services.UserServices
{
    public class UserProfileService : IUserProfileService
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IPhoneVerificationService phoneVerificationService;
        private readonly IFileService fileService;

        public UserProfileService(UserManager<AppUser> userManager, IFileService fileService, IPhoneVerificationService phoneVerificationService)
        {
            this.userManager = userManager;
            this.fileService = fileService;
            this.phoneVerificationService = phoneVerificationService;
        }

        public async Task<UserProfileDTO?> GetUserProfileAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return null;

            var roles = await userManager.GetRolesAsync(user);

            return new UserProfileDTO
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.UserName!,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber,
                ImageUrl = user.ImageUrl,
                Role = roles.FirstOrDefault() ?? "Student"
            };
        }

        public async Task<bool> UpdateUserProfileAsync(string userId, UpdateProfileDTO dto)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            // Update only provided fields
            if (!string.IsNullOrEmpty(dto.FirstName))
                user.FirstName = dto.FirstName;

            if (!string.IsNullOrEmpty(dto.LastName))
                user.LastName = dto.LastName;

            // Update phone if provided
            if (!string.IsNullOrEmpty(dto.PhoneNumber))
            {
                user.PhoneNumber = dto.PhoneNumber;
                user.PhoneNumberConfirmed = false;

                // Send OTP for verification
                await phoneVerificationService.SendCodeAsync(user.PhoneNumber);
            }

            // Handle image upload
            if (dto.ImageFile != null)
            {
                var imageUrl = await fileService.UploadProfileImageAsync(dto.ImageFile, user.Id, user.UserName!);
                if (imageUrl != null)
                    user.ImageUrl = imageUrl;
            }

            var updateResult = await userManager.UpdateAsync(user);
            return updateResult.Succeeded;
        }


        public async Task<bool> DeleteProfileImageAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            // Delete physical file
            await fileService.DeleteProfileImageAsync(user.Id, user.UserName!);

            // Clear ImageUrl from database
            user.ImageUrl = null;
            var result = await userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}