using Google.Apis.Auth;

namespace UCourses_Back_End.Core.Interfaces.IServices
{
    public interface IGoogleAuthService
    {
        Task<GoogleJsonWebSignature.Payload?> VerifyGoogleTokenAsync(string idToken);
    }
}
