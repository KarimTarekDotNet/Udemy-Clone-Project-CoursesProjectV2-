using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UCourses_Back_End.Core.DTOs;
using UCourses_Back_End.Core.Interfaces.IRepositories;

namespace UCourses_Back_End.Api.Controllers.UsersController
{
    [Authorize]
    public class UserProfileController : BaseController
    {
        public UserProfileController(IUnitOfWork work) : base(work)
        {
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.FindFirst("uid")?.Value ??
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var profile = await work.UserProfileService.GetUserProfileAsync(userId);

            if (profile == null)
                return NotFound(new { message = "User not found" });

            return Ok(profile);
        }

        [HttpGet("{userId}")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> GetUserProfile(string userId)
        {
            var profile = await work.UserProfileService.GetUserProfileAsync(userId);

            if (profile == null)
                return NotFound(new { message = "User not found" });

            return Ok(profile);
        }

        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileDTO dto)
        {
            var userId = User.FindFirst("uid")?.Value ??
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var result = await work.UserProfileService.UpdateUserProfileAsync(userId, dto);

                if (!result)
                    return BadRequest(new { message = "Failed to update profile" });

                var updatedProfile = await work.UserProfileService.GetUserProfileAsync(userId);

                return Ok(new
                {
                    message = "Profile updated successfully",
                    profile = updatedProfile
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("image")]
        [Authorize]
        public async Task<IActionResult> DeleteProfileImage()
        {
            var userId = User.FindFirst("uid")?.Value ??
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await work.UserProfileService.DeleteProfileImageAsync(userId);

            if (!result)
                return BadRequest(new { message = "Failed to delete profile image" });

            return Ok(new { message = "Profile image deleted successfully" });
        }
    }
}
