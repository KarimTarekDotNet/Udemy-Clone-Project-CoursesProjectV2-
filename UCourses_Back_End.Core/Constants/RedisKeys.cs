namespace UCourses_Back_End.Core.Constants
{
    public static class RedisKeys
    {
        public static string Verification(string email) => $"verification:{email}";
        public static string PasswordReset(string email) => $"password-reset:{email}";
        public static string PasswordResetAttempts(string email) => $"password-reset-attempts:{email}";
        public static string RefreshToken(string userId, string tokenId) => $"user:{userId}:refreshToken:{tokenId}";
        public static string RefreshTokenPattern(string userId) => $"user:{userId}:refreshToken:*";
        public static string BlacklistedToken(string token) => $"blacklisted:{token}";
        public static string PhoneVerificationRateLimit(string phone) => $"phone-verify-rate:{phone}";
    }
}
