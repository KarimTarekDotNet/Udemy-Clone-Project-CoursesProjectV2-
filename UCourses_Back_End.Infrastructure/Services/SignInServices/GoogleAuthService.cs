using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UCourses_Back_End.Core.Interfaces.IServices;

namespace UCourses_Back_End.Infrastructure.Services.SignInServices
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly string _clientId;
        private readonly ILogger<GoogleAuthService> _logger;

        public GoogleAuthService(IConfiguration configuration, ILogger<GoogleAuthService> logger)
        {
            _clientId = configuration["Authentication:Google:ClientId"] 
                ?? throw new ArgumentNullException("Authentication:Google:ClientId is not configured");
            _logger = logger;
        }

        public async Task<GoogleJsonWebSignature.Payload?> VerifyGoogleTokenAsync(string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _clientId }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
                
                _logger.LogInformation("Successfully verified Google token for email: {Email}", payload.Email);
                
                return payload;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to verify Google ID token");
                return null;
            }
        }
    }
}
