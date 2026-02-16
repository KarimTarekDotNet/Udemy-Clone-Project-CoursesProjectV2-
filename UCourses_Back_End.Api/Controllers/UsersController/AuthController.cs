using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using UCourses_Back_End.Core.DTOs.AuthDTOs;
using UCourses_Back_End.Core.Entites.AuthModel;
using UCourses_Back_End.Core.Interfaces.IRepositories;
using UCourses_Back_End.Core.Interfaces.IServices;

namespace UCourses_Back_End.Api.Controllers.UsersController
{
    public class AuthController : BaseController
    {
        private readonly IGoogleAuthService _googleAuthService;
        private readonly UserManager<AppUser> _userManager;

        public AuthController(IUnitOfWork work, IGoogleAuthService googleAuthService, UserManager<AppUser> userManager) : base(work)
        {
            _googleAuthService = googleAuthService;
            _userManager = userManager;
        }

        // ========================= Register =========================
        [HttpPost("register")]
        [EnableRateLimiting("authPolicy")]
            public async Task<IActionResult> Register(RegisterDTO dto)
            {
                var result = await work.AuthService.Register(dto);

                if (!result.Succeeded)
                    return BadRequest(new { errors = result.Errors });

                return Ok(new
                {
                    succeeded = result.Succeeded,
                    userId = result.UserId,
                    email = result.Email,
                    username = result.Username,
                    role = result.Role,
                    message = result.Message
                });
            }

            // ========================= Verify Email =========================
            [HttpPost("verify-email")]
            [EnableRateLimiting("authPolicy")]
            public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDTO dto)
            {
                var result = await work.AuthService.VerifyEmailAsync(dto.Email, dto.Code);

                if (!result.Succeeded)
                    return BadRequest(new { errors = result.Errors });

                SetRefreshTokenCookie(result.RefreshToken!);

                return Ok(new
                {
                    succeeded = result.Succeeded,
                    token = result.Token,
                    refreshToken = result.RefreshToken,
                    userId = result.UserId,
                    email = result.Email,
                    username = result.Username,
                    role = result.Role,
                    message = result.Message
                });
            }

            // ========================= Verify Phone =========================
            [HttpPost("verify-phone")]
            [Authorize]
            public async Task<IActionResult> VerifyPhone([FromBody] VerifyPhoneDTO dto)
            {
                var userId = User.FindFirst("uid")?.Value ?? 
                             User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                             
                if (userId == null)
                    return Unauthorized();

                var result = await work.AuthService.VerifyPhoneAsync(userId, dto.Code);

                if (!result.Succeeded)
                    return BadRequest(result);

                return Ok(result);
            }

            [HttpPost("resend-phone-code")]
            public async Task<IActionResult> ResendPhoneCode()
            {
                var userId = User.FindFirst("uid")?.Value ?? 
                             User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                             
                if (userId == null)
                    return Unauthorized();

                var result = await work.AuthService.ResendPhoneVerificationCodeAsync(userId);

                if (!result)
                    return BadRequest(new { message = "Failed to resend verification code. Please try again later or check rate limit." });

                return Ok(new { message = "Verification code sent successfully" });
            }

            // ========================= Login =========================

            [HttpPost("login")]
            [EnableRateLimiting("authPolicy")]
            public async Task<IActionResult> Login(LoginDTO dto)
            {
                var result = await work.AuthService.Login(dto);

                if (!result.Succeeded)
                    return BadRequest(new { errors = result.Errors });

                SetRefreshTokenCookie(result.RefreshToken!);

                return Ok(new
                {
                    succeeded = result.Succeeded,
                    token = result.Token,
                    refreshToken = result.RefreshToken,
                    userId = result.UserId,
                    email = result.Email,
                    username = result.Username,
                    role = result.Role,
                    message = result.Message
                });
            }

            // ========================= Logout =========================
            [HttpPost("logout")]
            [Authorize]
            public async Task<IActionResult> Logout()
            {
                var userId = User.FindFirst("uid")?.Value ??
                             User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                await work.AuthService.LogoutAsync(userId, accessToken);

                return Ok(new { message = "Logged out successfully" });
            }

            // ========================= Logout All =========================
            [HttpPost("logout-all")]
            [Authorize]
            public async Task<IActionResult> LogoutAll()
            {
                var userId = User.FindFirst("uid")?.Value ??
                             User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                await work.AuthService.LogoutAllAsync(userId, accessToken);

                return Ok(new { message = "Logged out from all sessions successfully" });
            }

            // ========================= Refresh Token =========================
            [HttpPost("refresh-token")]
            public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO dto)
            {
                if (string.IsNullOrWhiteSpace(dto.RefreshToken))
                    return BadRequest(new { message = "Refresh token is required" });

                var result = await work.AuthService.RefreshTokenAsync(dto);

                if (!result.Succeeded)
                    return Unauthorized(new { errors = result.Errors });

                SetRefreshTokenCookie(result.RefreshToken!);

                return Ok(new
                {
                    token = result.Token,
                    refreshToken = result.RefreshToken
                });
            }

            // ========================= Forget Password =========================

            [HttpPost("forgot-password")]
            [EnableRateLimiting("authPolicy")]
            public async Task<IActionResult> ForgotPassword([FromBody] ForgetPasswordDTO dto)
            {
                var result = await work.AuthService.RequestPasswordResetAsync(dto);
                if (!result)
                    return BadRequest(new { message = "Email not found" });

                return Ok(new { message = "Password reset code sent to email" });
            }

            [HttpPost("verify-reset-code")]
            [EnableRateLimiting("authPolicy")]
            public async Task<IActionResult> VerifyResetCode([FromBody] VerifyResetCodeDTO dto)
            {
                var result = await work.AuthService.VerifyResetCodeAsync(dto);
                if (!result)
                    return BadRequest(new { message = "Invalid or expired code" });

                return Ok(new { message = "Code verified successfully" });
            }

            [HttpPost("reset-password")]
            [EnableRateLimiting("authPolicy")]
            public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
            {
                var result = await work.AuthService.ResetPasswordAsync(dto);
                if (!result)
                    return BadRequest(new { message = "Failed to reset password" });

                return Ok(new { message = "Password reset successfully" });
            }

            // ========================= Google Sign-In =========================

            [HttpGet("google-login")]
            public IActionResult GoogleLogin()
            {
                var redirectUrl = Url.Action(nameof(GoogleResponse), "Auth");
                var properties = new AuthenticationProperties
                {
                    RedirectUri = redirectUrl
                };

                return Challenge(properties, GoogleDefaults.AuthenticationScheme);
            }

            [HttpGet("google-response")]
            public async Task<IActionResult> GoogleResponse()
            {
                var result = await HttpContext.AuthenticateAsync(
                    IdentityConstants.ExternalScheme);

                if (!result.Succeeded)
                    return BadRequest(new { message = "External authentication failed" });

                var externalUser = result.Principal;
                var email = externalUser.FindFirstValue(ClaimTypes.Email);
                var name = externalUser.FindFirstValue(ClaimTypes.Name);
                var providerId = externalUser.FindFirstValue(ClaimTypes.NameIdentifier);
                var picture = externalUser.FindFirstValue("picture");

                if (string.IsNullOrEmpty(email))
                    return BadRequest(new { message = "Email not provided by Google" });

                var dto = new ExternalAuthDTO
                {
                    Provider = "Google",
                    Email = email,
                    Name = name ?? email,
                    ProviderId = providerId!,
                    ProfilePicture = picture
                };

                var authResult = await work.AuthService.ExternalLoginAsync(dto, null);

                if (!authResult.Succeeded)
                    return BadRequest(new { errors = authResult.Errors });

                // Set tokens in secure HTTP-only cookies
                SetRefreshTokenCookie(authResult.RefreshToken!);
                SetAccessTokenCookie(authResult.Token!);

                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

                // Redirect to frontend without token in URL
                var frontendUrl = "https://localhost:5501/auth/callback";
                return Redirect(frontendUrl);
            }

            // ========================= Alternative: POST endpoint for mobile/SPA =========================

            [HttpPost("google-signin")]
            [EnableRateLimiting("authPolicy")]
            public async Task<IActionResult> GoogleSignIn([FromBody] GoogleSignInDTO dto)
            {
                try
                {
                    // Verify Google ID Token
                    var payload = await _googleAuthService.VerifyGoogleTokenAsync(dto.IdToken);

                    if (payload == null)
                        return BadRequest(new { errors = new[] { "Invalid Google token" } });

                    // Validate required fields from Google
                    if (string.IsNullOrEmpty(payload.Email))
                        return BadRequest(new { errors = new[] { "Email not provided by Google" } });

                    if (string.IsNullOrEmpty(payload.Subject))
                        return BadRequest(new { errors = new[] { "User ID not provided by Google" } });

                    // Create ExternalAuthDTO from verified payload
                    var externalAuthDto = new ExternalAuthDTO
                    {
                        Provider = "Google",
                        Email = payload.Email,
                        Name = !string.IsNullOrWhiteSpace(payload.Name) ? payload.Name : payload.Email.Split('@')[0],
                        ProviderId = payload.Subject,
                        ProfilePicture = payload.Picture
                    };

                    var result = await work.AuthService.ExternalLoginAsync(externalAuthDto, dto.Role);

                    if (!result.Succeeded)
                        return BadRequest(new { errors = result.Errors });

                    SetRefreshTokenCookie(result.RefreshToken!);

                    return Ok(new
                    {
                        succeeded = result.Succeeded,
                        token = result.Token,
                        refreshToken = result.RefreshToken,
                        userId = result.UserId,
                        email = result.Email,
                        username = result.Username,
                        role = result.Role,
                        message = result.Message
                    });
                }
                catch (Exception)
                {
                    // Log the error (handled by global exception middleware)
                    return StatusCode(500, new { errors = new[] { "An error occurred during Google sign-in. Please try again." } });
                }
            }

            // ========================= Helpers =========================
            private void SetRefreshTokenCookie(string refresh)
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                };

                Response.Cookies.Append("refreshToken", refresh, cookieOptions);
            }

            private void SetAccessTokenCookie(string accessToken)
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(30) // Match JWT expiry
                };

                Response.Cookies.Append("accessToken", accessToken, cookieOptions);
            }
        }
    }