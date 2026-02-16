using UCourses_Back_End.Core.Entites.AuthModel;

namespace UCourses_Back_End.Core.Interfaces.IServices
{
    public interface ITokenService
    {
        Task<string> GenerateToken(AppUser user);
        string GenerateRefreshToken();
        Task<RefreshToken> SaveRefreshTokenAsync(string userId, string refreshToken);
        RefreshToken? GetRefreshTokenAsync(string userId);
        Task RevokeAllUserTokensAsync(string userId, string? ipAddress = null, string? replacedByToken = null);
        Task RevokeUserTokensAsync(int tokenId, string userId, string? ipAddress = null, string? replacedByToken = null);
    }
}