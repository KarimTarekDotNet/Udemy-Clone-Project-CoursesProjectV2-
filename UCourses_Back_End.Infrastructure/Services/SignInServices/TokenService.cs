using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UCourses_Back_End.Core.Entites.AuthModel;
using UCourses_Back_End.Core.Entites.Users;
using UCourses_Back_End.Core.Interfaces.IServices;
using UCourses_Back_End.Infrastructure.Data;
using UCourses_Back_End.Infrastructure.Utilities;

namespace UCourses_Back_End.Infrastructure.Services.SignInServices
{
    public class TokenService : ITokenService
    {
        private readonly ApplicationDbContext context;
        private readonly IConfiguration _config;
        private readonly UserManager<AppUser> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;

        public TokenService(ApplicationDbContext context, UserManager<AppUser> userManager, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            this.userManager = userManager;
            _config = config;
            this.httpContextAccessor = httpContextAccessor;
        }

        public string GenerateRefreshToken()
        {
            var random = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
                return Convert.ToBase64String(random);
            }
        }

        public async Task<RefreshToken> SaveRefreshTokenAsync(string userId, string refreshToken)
        {
            var ipAddress = IpAddressHelper.GetClientIpAddress(httpContextAccessor);
            var refreshTokenRecord = new RefreshToken
            {
                UserId = userId,
                Token = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(15),
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };

            await context.RefreshTokens.AddAsync(refreshTokenRecord);
            await context.SaveChangesAsync();
            return refreshTokenRecord;
        }

        public RefreshToken? GetRefreshTokenAsync(string userId)
        {
            return context.RefreshTokens
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .AsEnumerable()
                .FirstOrDefault(r => r.IsActive);
        }

        public async Task RevokeAllUserTokensAsync(string userId, string? ipAddress = null, string? replacedByToken = null)
        {
            var tokens = await context.RefreshTokens
                .Where(r => r.UserId == userId && !r.IsRevoked)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.Revoke(ipAddress, replacedByToken);
            }

            await context.SaveChangesAsync();
        }
        
        public async Task RevokeUserTokensAsync(int tokenId, string userId, string? ipAddress = null, string? replacedByToken = null)
        {
            var token = await context.RefreshTokens
                .FirstOrDefaultAsync(r => r.Id == tokenId && r.UserId == userId);

            if (token is not null && token.IsActive)
            {
                token.Revoke(ipAddress, replacedByToken);
                await context.SaveChangesAsync();
            }
        }

        public async Task<string> GenerateToken(AppUser user)
        {
            var roles = await userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("uid", user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!)
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            // Add PublicId claim for Student/Instructor
            var publicId = await GetUserPublicIdAsync(user.Id, roles);
            if (!string.IsNullOrEmpty(publicId))
            {
                claims.Add(new Claim("PublicId", publicId));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]!));
            var token = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<string?> GetUserPublicIdAsync(string appUserId, IList<string> roles)
        {
            if (roles.Contains("Student"))
            {
                var student = await context.Students
                    .FirstOrDefaultAsync(s => s.AppUserId == appUserId);
                return student?.PublicId;
            }
            else if (roles.Contains("Instructor"))
            {
                var instructor = await context.Instructors
                    .FirstOrDefaultAsync(i => i.AppUserId == appUserId);
                return instructor?.PublicId;
            }

            return null;
        }
    }
}