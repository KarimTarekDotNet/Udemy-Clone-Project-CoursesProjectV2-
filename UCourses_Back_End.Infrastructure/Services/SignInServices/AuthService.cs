using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UCourses_Back_End.Core.Constants;
using UCourses_Back_End.Core.DTOs.AuthDTOs;
using UCourses_Back_End.Core.Entites.AuthModel;
using UCourses_Back_End.Core.Entites.Users;
using UCourses_Back_End.Core.Enums.CoreEnum;
using UCourses_Back_End.Core.Interfaces.IBackground;
using UCourses_Back_End.Core.Interfaces.IServices;
using UCourses_Back_End.Core.ModelsView;
using UCourses_Back_End.Infrastructure.Data;
using UCourses_Back_End.Infrastructure.Utilities;

namespace UCourses_Back_End.Infrastructure.Services.SignInServices
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IRedisService _redisService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISendConfirmationEmailJob _confirmationEmailJob;
        private readonly ISendPasswordResetEmailJob _passwordResetEmailJob;
        private readonly ILogger<AuthService> _logger;
        private readonly IPhoneVerificationService _phoneVerificationService;

        public AuthService(
            ApplicationDbContext context,
            UserManager<AppUser> userManager,
            ITokenService tokenService,
            IMapper mapper,
            IRedisService redisService,
            IHttpContextAccessor httpContextAccessor,
            ISendConfirmationEmailJob confirmationEmailJob,
            ISendPasswordResetEmailJob passwordResetEmailJob,
            ILogger<AuthService> logger,
            IPhoneVerificationService phoneVerificationService)
        {
            _context = context;
            _userManager = userManager;
            _tokenService = tokenService;
            _mapper = mapper;
            _redisService = redisService;
            _httpContextAccessor = httpContextAccessor;
            _confirmationEmailJob = confirmationEmailJob;
            _passwordResetEmailJob = passwordResetEmailJob;
            _logger = logger;
            _phoneVerificationService = phoneVerificationService;
        }

        // ========================= Register =========================
        public async Task<AuthResult> Register(RegisterDTO dto)
        {
            try
            {
                if (await _userManager.FindByEmailAsync(dto.Email) != null ||
                    await _userManager.FindByNameAsync(dto.Username) != null)
                    return Fail("Email or username already exists");

                var user = _mapper.Map<AppUser>(dto);

                if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                {
                    user.PhoneNumber = dto.PhoneNumber;
                    user.PhoneNumberConfirmed = false;
                }

                var createResult = await _userManager.CreateAsync(user, dto.Password);
                if (!createResult.Succeeded)
                    return Fail(createResult.Errors.Select(e => e.Description));

                var roleResult = await _userManager.AddToRoleAsync(user, dto.Role);
                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    return Fail("Role assignment failed");
                }

                try
                {
                    if (dto.Role.Equals(UserRole.Instructor.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        await _context.Instructors.AddAsync(new Instructor { AppUserId = user.Id });
                    }
                    else if (dto.Role.Equals(UserRole.Student.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        await _context.Students.AddAsync(new Student { AppUserId = user.Id });
                    }

                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to create role-specific record for user {Email}", dto.Email);
                    await _userManager.DeleteAsync(user);
                    return Fail("Failed to complete registration");
                }

                await SendVerificationEmailAsync(user.Email!);

                bool phoneCodeSent = false;
                if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
                {
                    try
                    {
                        // Rate limiting check
                        var rateLimitKey = RedisKeys.PhoneVerificationRateLimit(user.PhoneNumber);
                        var lastSent = await _redisService.GetAsync(rateLimitKey);
                        
                        if (string.IsNullOrEmpty(lastSent))
                        {
                            await _phoneVerificationService.SendCodeAsync(user.PhoneNumber);
                            await _redisService.SetAsync(rateLimitKey, DateTime.UtcNow.ToString(), TimeSpan.FromMinutes(1));
                            phoneCodeSent = true;
                            _logger.LogInformation("Phone verification code sent to {Phone}", user.PhoneNumber);
                        }
                        else
                        {
                            _logger.LogWarning("Rate limit prevented sending code to {Phone}", user.PhoneNumber);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send phone verification code to {Phone}. Registration will continue.", user.PhoneNumber);
                    }
                }

                var roles = await _userManager.GetRolesAsync(user);

                _logger.LogInformation("User {Email} registered successfully", dto.Email);

                string message = "Registration successful. Verification code sent to email";
                if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
                {
                    message += phoneCodeSent 
                        ? " and phone." 
                        : ". Phone verification code could not be sent, please use resend option.";
                }
                else
                {
                    message += ".";
                }

                return new AuthResult
                {
                    Succeeded = true,
                    UserId = user.Id,
                    Email = user.Email,
                    Username = user.UserName,
                    Role = roles.FirstOrDefault(),
                    Message = message
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for {Email}", dto.Email);
                return Fail("An error occurred during registration");
            }
        }

        // ========================= Verify Email =========================
        public async Task<AuthResult> VerifyEmailAsync(string email, string code)
        {
            try
            {
                var savedCode = await _redisService.GetAsync(RedisKeys.Verification(email));
                if (savedCode == null || savedCode != code)
                {
                    _logger.LogWarning("Invalid verification code for {Email}", email);
                    return Fail("Invalid or expired verification code");
                }

                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return Fail("User not found");

                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);

                await _redisService.DeleteAsync(RedisKeys.Verification(email));

                // Revoke all previous refresh tokens
                var ipAddress = IpAddressHelper.GetClientIpAddress(_httpContextAccessor);
                await _tokenService.RevokeAllUserTokensAsync(user.Id, ipAddress, "EmailVerification");

                var accessToken = await _tokenService.GenerateToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken();
                var savedRefresh = await _tokenService.SaveRefreshTokenAsync(user.Id, refreshToken);

                var redisKey = RedisKeys.RefreshToken(user.Id, savedRefresh.Id.ToString());
                await _redisService.SetAsync(redisKey, refreshToken, TimeSpan.FromDays(15));

                var roles = await _userManager.GetRolesAsync(user);

                _logger.LogInformation("Email verified for {Email}", email);

                return new AuthResult
                {
                    Succeeded = true,
                    Token = accessToken,
                    RefreshToken = refreshToken,
                    UserId = user.Id,
                    Email = user.Email,
                    Username = user.UserName,
                    Role = roles.FirstOrDefault(),
                    Message = "Email verified successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during email verification for {Email}", email);
                return Fail("An error occurred during email verification");
            }
        }

        // ========================= Verify Phone =========================
        public async Task<AuthResult> VerifyPhoneAsync(string userId, string code)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("Phone verification attempted for non-existent user {UserId}", userId);
                    return Fail("User not found");
                }

                if (user.PhoneNumberConfirmed)
                {
                    _logger.LogWarning("Phone verification attempted for already verified phone {UserId}", userId);
                    return Fail("Phone already verified");
                }

                if (string.IsNullOrWhiteSpace(user.PhoneNumber))
                {
                    _logger.LogWarning("Phone verification attempted but no phone number for user {UserId}", userId);
                    return Fail("No phone number provided");
                }

                var isValid = await _phoneVerificationService.VerifyCodeAsync(user.PhoneNumber, code);

                if (!isValid)
                {
                    _logger.LogWarning("Invalid verification code for user {UserId}", userId);
                    return Fail("Invalid or expired verification code");
                }

                user.PhoneNumberConfirmed = true;

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    _logger.LogError("Failed to update phone confirmation for user {UserId}", userId);
                    return Fail(updateResult.Errors.Select(e => e.Description));
                }

                _logger.LogInformation("Phone verified successfully for user {UserId}", userId);

                return new AuthResult
                {
                    Succeeded = true,
                    Message = "Phone number verified successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during phone verification for user {UserId}", userId);
                return Fail("An error occurred during phone verification");
            }
        }

        // ========================= Resend Phone Verification Code =========================
        public async Task<bool> ResendPhoneVerificationCodeAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("Resend phone code attempted for non-existent user {UserId}", userId);
                    return false;
                }

                if (user.PhoneNumberConfirmed)
                {
                    _logger.LogWarning("Resend phone code attempted for already verified phone {UserId}", userId);
                    return false;
                }

                if (string.IsNullOrWhiteSpace(user.PhoneNumber))
                {
                    _logger.LogWarning("Resend phone code attempted but no phone number for user {UserId}", userId);
                    return false;
                }

                // Rate limiting: Check if code was sent recently
                var rateLimitKey = RedisKeys.PhoneVerificationRateLimit(user.PhoneNumber);
                var lastSent = await _redisService.GetAsync(rateLimitKey);
                
                if (!string.IsNullOrEmpty(lastSent))
                {
                    _logger.LogWarning("Rate limit exceeded for phone {Phone}", user.PhoneNumber);
                    return false;
                }

                await _phoneVerificationService.SendCodeAsync(user.PhoneNumber);

                // Set rate limit: 1 minute cooldown
                await _redisService.SetAsync(rateLimitKey, DateTime.UtcNow.ToString(), TimeSpan.FromMinutes(1));

                _logger.LogInformation("Phone verification code resent to user {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending phone verification code for user {UserId}", userId);
                return false;
            }
        }


        // ========================= Login =========================
        public async Task<AuthResult> Login(LoginDTO dto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null)
                    return Fail("Invalid email or password");

                if (!await _userManager.CheckPasswordAsync(user, dto.Password))
                    return Fail("Invalid email or password");

                if (!user.EmailConfirmed)
                {
                    await SendVerificationEmailAsync(user.Email!);
                    return Fail("Email not verified. A new verification code has been sent.");
                }

                // Revoke all previous refresh tokens
                var ipAddress = IpAddressHelper.GetClientIpAddress(_httpContextAccessor);
                await _tokenService.RevokeAllUserTokensAsync(user.Id, ipAddress, "Login");

                var accessToken = await _tokenService.GenerateToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken();
                var savedRefresh = await _tokenService.SaveRefreshTokenAsync(user.Id, refreshToken);

                var redisKey = RedisKeys.RefreshToken(user.Id, savedRefresh.Id.ToString());
                await _redisService.SetAsync(redisKey, refreshToken, TimeSpan.FromDays(15));

                var roles = await _userManager.GetRolesAsync(user);

                _logger.LogInformation("User {Email} logged in successfully", dto.Email);

                return new AuthResult
                {
                    Succeeded = true,
                    Token = accessToken,
                    RefreshToken = refreshToken,
                    UserId = user.Id,
                    Email = user.Email,
                    Username = user.UserName,
                    Role = roles.FirstOrDefault(),
                    Message = "Login successful"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Email}", dto.Email);
                return Fail("An error occurred during login");
            }
        }

        // ========================= Logout =========================
        public async Task<bool> LogoutAsync(string userId, string accessToken)
        {
            try
            {
                // Blacklist the current access token for its remaining lifetime (30 minutes to match JWT expiry)
                await _redisService.SetAsync(RedisKeys.BlacklistedToken(accessToken), "blacklisted", TimeSpan.FromMinutes(30));
                
                // Revoke all refresh tokens (since we don't know which one is current)
                var ipAddress = IpAddressHelper.GetClientIpAddress(_httpContextAccessor);
                await _tokenService.RevokeAllUserTokensAsync(userId, ipAddress, "Logout");
                
                _logger.LogInformation("User {UserId} logged out", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> LogoutAllAsync(string userId, string accessToken)
        {
            try
            {
                // Blacklist the current access token for its remaining lifetime (30 minutes to match JWT expiry)
                await _redisService.SetAsync(RedisKeys.BlacklistedToken(accessToken), "blacklisted", TimeSpan.FromMinutes(30));

                // Revoke all refresh tokens
                var ipAddress = IpAddressHelper.GetClientIpAddress(_httpContextAccessor);
                await _tokenService.RevokeAllUserTokensAsync(userId, ipAddress, "LogoutAll");

                _logger.LogInformation("User {UserId} logged out from all sessions", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout all for user {UserId}", userId);
                return false;
            }
        }

        // ========================= Refresh Token =========================
        public async Task<AuthResult> RefreshTokenAsync(RefreshTokenDTO dto)
        {
            try
            {
                var refreshToken = await _context.RefreshTokens
                    .FirstOrDefaultAsync(r => r.Token == dto.RefreshToken);

                if (refreshToken == null || !refreshToken.IsActive)
                    return Fail("Invalid or expired refresh token");

                var user = await _userManager.FindByIdAsync(refreshToken.UserId);
                if (user == null)
                    return Fail("User not found");

                var newAccessToken = await _tokenService.GenerateToken(user);
                var newRefreshToken = _tokenService.GenerateRefreshToken();

                var ipAddress = IpAddressHelper.GetClientIpAddress(_httpContextAccessor);
                refreshToken.Revoke(ipAddress, "Refresh");

                var savedRefresh = await _tokenService.SaveRefreshTokenAsync(user.Id, newRefreshToken);
                await _context.SaveChangesAsync();

                var redisKey = RedisKeys.RefreshToken(user.Id, savedRefresh.Id.ToString());
                await _redisService.SetAsync(redisKey, newRefreshToken, TimeSpan.FromDays(15));

                var roles = await _userManager.GetRolesAsync(user);

                _logger.LogInformation("Token refreshed for user {UserId}", user.Id);

                return new AuthResult
                {
                    Succeeded = true,
                    Token = newAccessToken,
                    RefreshToken = newRefreshToken,
                    UserId = user.Id,
                    Email = user.Email!,
                    Username = user.UserName!,
                    Role = roles.FirstOrDefault() ?? string.Empty,
                    Message = "Token refreshed successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                return Fail("An error occurred while refreshing token");
            }
        }

        // ========================= Password Reset =========================
        public async Task<bool> RequestPasswordResetAsync(ForgetPasswordDTO dto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null)
                    return false;

                var code = GenerateVerificationCode();
                await _redisService.SetAsync(RedisKeys.PasswordReset(dto.Email), code, TimeSpan.FromMinutes(15));
                await _passwordResetEmailJob.SendPasswordResetEmail(dto.Email, code);

                _logger.LogInformation("Password reset requested for {Email}", dto.Email);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error requesting password reset for {Email}", dto.Email);
                return false;
            }
        }

        public async Task<bool> VerifyResetCodeAsync(VerifyResetCodeDTO dto)
        {
            try
            {
                // Rate limiting: Check failed attempts
                var attemptsKey = RedisKeys.PasswordResetAttempts(dto.Email);
                var attemptsStr = await _redisService.GetAsync(attemptsKey);
                
                if (!int.TryParse(attemptsStr, out var attempts))
                {
                    attempts = 0;
                }

                if (attempts >= 5)
                {
                    _logger.LogWarning("Password reset verification blocked for {Email} due to too many attempts", dto.Email);
                    return false;
                }

                var savedCode = await _redisService.GetAsync(RedisKeys.PasswordReset(dto.Email));
                
                if (savedCode == null || savedCode != dto.Code)
                {
                    // Increment failed attempts
                    attempts++;
                    await _redisService.SetAsync(attemptsKey, attempts.ToString(), TimeSpan.FromMinutes(15));
                    _logger.LogWarning("Invalid reset code attempt {Attempts}/5 for {Email}", attempts, dto.Email);
                    return false;
                }

                // Clear attempts on success
                await _redisService.DeleteAsync(attemptsKey);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying reset code for {Email}", dto.Email);
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDTO dto)
        {
            try
            {
                var savedCode = await _redisService.GetAsync(RedisKeys.PasswordReset(dto.Email));
                if (savedCode == null || savedCode != dto.Code)
                    return false;

                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null)
                    return false;

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);

                if (result.Succeeded)
                {
                    await _redisService.DeleteAsync(RedisKeys.PasswordReset(dto.Email));
                    await _redisService.DeleteAsync(RedisKeys.PasswordResetAttempts(dto.Email));
                    _logger.LogInformation("Password reset successfully for {Email}", dto.Email);
                }

                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for {Email}", dto.Email);
                return false;
            }
        }

        // ========================= External Login =========================
        public async Task<AuthResult> ExternalLoginAsync(ExternalAuthDTO dto, string? role = null)
        {
            try
            {
                // Use transaction to prevent race condition
                using var transaction = await _context.Database.BeginTransactionAsync();
                
                try
                {
                    var user = await _userManager.FindByEmailAsync(dto.Email);

                    if (user == null)
                    {
                        if (string.IsNullOrEmpty(role))
                        {
                            await transaction.RollbackAsync();
                            return Fail("Role is required for new users");
                        }

                        var selectedRole = role.Equals(Roles.Student, StringComparison.OrdinalIgnoreCase) ? Roles.Student :
                                          role.Equals(Roles.Instructor, StringComparison.OrdinalIgnoreCase) ? Roles.Instructor :
                                          role.Equals(Roles.Admin, StringComparison.OrdinalIgnoreCase) ? Roles.Admin : null;

                        if (selectedRole == null)
                        {
                            await transaction.RollbackAsync();
                            return Fail("Invalid role specified");
                        }

                        var username = GenerateUniqueUsername(dto.Email);
                        
                        // Split name from Google (e.g., "John Doe" -> FirstName: "John", LastName: "Doe") 
                        var nameParts = dto.Name?.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
                        var firstName = nameParts.Length > 0 ? nameParts[0] : "User";
                        var lastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;

                        user = new AppUser
                        {
                            Email = dto.Email,
                            UserName = username,
                            FirstName = firstName,
                            LastName = lastName,
                            EmailConfirmed = true,
                            ImageUrl = dto.ProfilePicture,
                            Provider = dto.Provider,
                            ProviderId = dto.ProviderId
                        };

                        var createResult = await _userManager.CreateAsync(user);
                        if (!createResult.Succeeded)
                        {
                            await transaction.RollbackAsync();
                            
                            // Check if error is due to duplicate email (race condition)
                            if (createResult.Errors.Any(e => e.Code.Contains("Duplicate") || e.Description.Contains("already")))
                            {
                                // Retry: User was created by another request
                                user = await _userManager.FindByEmailAsync(dto.Email);
                                if (user == null)
                                    return Fail("Failed to create or find user");
                            }
                            else
                            {
                                return Fail(createResult.Errors.Select(e => e.Description));
                            }
                        }
                        else
                        {
                            // User created successfully, assign role
                            var roleResult = await _userManager.AddToRoleAsync(user, selectedRole);
                            if (!roleResult.Succeeded)
                            {
                                await _userManager.DeleteAsync(user);
                                await transaction.RollbackAsync();
                                return Fail("Failed to assign user role");
                            }

                            try
                            {
                                if (selectedRole == Roles.Student)
                                {
                                    await _context.Students.AddAsync(new Student { AppUserId = user.Id });
                                }
                                else if (selectedRole == Roles.Instructor)
                                {
                                    await _context.Instructors.AddAsync(new Instructor { AppUserId = user.Id });
                                }

                                await _context.SaveChangesAsync();
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Failed to create role record for external user {Email}", dto.Email);
                                await _userManager.DeleteAsync(user);
                                await transaction.RollbackAsync();
                                return Fail("Failed to complete user registration");
                            }
                        }
                    }
                    else
                    {
                        if (!user.EmailConfirmed)
                        {
                            user.EmailConfirmed = true;
                            await _userManager.UpdateAsync(user);
                        }

                        if (!string.IsNullOrEmpty(dto.ProfilePicture) && string.IsNullOrEmpty(user.ImageUrl))
                        {
                            user.ImageUrl = dto.ProfilePicture;
                            await _userManager.UpdateAsync(user);
                        }
                    }

                    // Revoke all previous refresh tokens
                    var ipAddress = IpAddressHelper.GetClientIpAddress(_httpContextAccessor);
                    await _tokenService.RevokeAllUserTokensAsync(user.Id, ipAddress, "ExternalLogin");

                    var accessToken = await _tokenService.GenerateToken(user);
                    var refreshToken = _tokenService.GenerateRefreshToken();
                    var savedRefresh = await _tokenService.SaveRefreshTokenAsync(user.Id, refreshToken);

                    var redisKey = RedisKeys.RefreshToken(user.Id, savedRefresh.Id.ToString());
                    await _redisService.SetAsync(redisKey, refreshToken, TimeSpan.FromDays(15));

                    var roles = await _userManager.GetRolesAsync(user);

                    await transaction.CommitAsync();

                    _logger.LogInformation("External login successful for {Email}", dto.Email);

                    return new AuthResult
                    {
                        Succeeded = true,
                        Token = accessToken,
                        RefreshToken = refreshToken,
                        UserId = user.Id,
                        Email = user.Email,
                        Username = user.UserName,
                        Role = roles.FirstOrDefault(),
                        Message = "Login successful"
                    };
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during external login for {Email}", dto.Email);
                return Fail("An error occurred during external login");
            }
        }

        // ========================= Helper Methods =========================
        private async Task SendVerificationEmailAsync(string email)
        {
            try
            {
                var code = GenerateVerificationCode();
                await _redisService.SetAsync(RedisKeys.Verification(email), code, TimeSpan.FromMinutes(15));
                await _confirmationEmailJob.SendVerificationCodeEmail(email, code);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send verification email to {Email}", email);
                throw;
            }
        }

        private static string GenerateVerificationCode(int length = 6)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var result = new char[length];
            
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            var randomBytes = new byte[length];
            rng.GetBytes(randomBytes);
            
            for (int i = 0; i < length; i++)
            {
                result[i] = chars[randomBytes[i] % chars.Length];
            }
            
            return new string(result);
        }

        private static string GenerateUniqueUsername(string email)
        {
            var baseUsername = email.Split('@')[0];
            var uniquePart = Guid.NewGuid().ToString("N").Substring(0, 8);
            return $"{baseUsername}_{uniquePart}";
        }

        private static AuthResult Fail(string error)
        {
            return new AuthResult
            {
                Succeeded = false,
                Errors = new List<string> { error }
            };
        }

        private static AuthResult Fail(IEnumerable<string> errors)
        {
            return new AuthResult
            {
                Succeeded = false,
                Errors = errors.ToList()
            };
        }
    }
}
