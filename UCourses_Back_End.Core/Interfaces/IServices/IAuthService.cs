using UCourses_Back_End.Core.DTOs.AuthDTOs;
using UCourses_Back_End.Core.ModelsView;

namespace UCourses_Back_End.Core.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<AuthResult> Register(RegisterDTO registerDTO);
        Task<AuthResult> Login(LoginDTO loginDTO);
        Task<bool> LogoutAsync(string userId, string accessToken);
        Task<bool> LogoutAllAsync(string userId, string accessToken);
        Task<AuthResult> RefreshTokenAsync(RefreshTokenDTO dto);
        Task<AuthResult> VerifyEmailAsync(string email, string code);
        Task<AuthResult> VerifyPhoneAsync(string userId, string code);
        Task<bool> ResendPhoneVerificationCodeAsync(string userId);
        Task<bool> RequestPasswordResetAsync(ForgetPasswordDTO passwordDTO);
        Task<bool> VerifyResetCodeAsync(VerifyResetCodeDTO resetCodeDTO);
        Task<bool> ResetPasswordAsync(ResetPasswordDTO passwordDTO);
        Task<AuthResult> ExternalLoginAsync(ExternalAuthDTO dto, string? role = null);
    }
}