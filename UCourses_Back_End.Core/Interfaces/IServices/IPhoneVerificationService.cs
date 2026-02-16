using Twilio.Rest.Api.V2010.Account;

namespace UCourses_Back_End.Core.Interfaces.IServices
{
    public interface IPhoneVerificationService
    {
        Task SendCodeAsync(string phone);
        Task<bool> VerifyCodeAsync(string phone, string code);
    }
}