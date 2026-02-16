using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Verify.V2.Service;
using UCourses_Back_End.Core.Interfaces.IServices;
using UCourses_Back_End.Core.Settings;

namespace UCourses_Back_End.Infrastructure.Services.HelperService
{
    public class TwilioVerifyService : IPhoneVerificationService
    {
        private readonly TwilioSettings _settings;
        private readonly ILogger<TwilioVerifyService> _logger;

        public TwilioVerifyService(IOptions<TwilioSettings> settings, ILogger<TwilioVerifyService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
            TwilioClient.Init(_settings.AccountSID, _settings.AuthToken);
        }

        public async Task SendCodeAsync(string phone)
        {
            try
            {
                // Validate phone format
                if (string.IsNullOrWhiteSpace(phone))
                {
                    _logger.LogWarning("Attempted to send verification code to empty phone number");
                    throw new ArgumentException("Phone number cannot be empty");
                }

                if (!phone.StartsWith("+"))
                {
                    _logger.LogWarning("Phone number {Phone} is not in international format", phone);
                    throw new ArgumentException("Phone number must be in international format (e.g., +201234567890)");
                }

                await VerificationResource.CreateAsync(
                    to: phone,
                    channel: "sms",
                    pathServiceSid: _settings.VerifyServiceSid
                );

                _logger.LogInformation("Verification code sent successfully to {Phone}", phone);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send verification code to {Phone}", phone);
                throw new InvalidOperationException("Failed to send verification code. Please try again later.", ex);
            }
        }

        public async Task<bool> VerifyCodeAsync(string phone, string code)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(code))
                {
                    _logger.LogWarning("Attempted to verify with empty phone or code");
                    return false;
                }

                var result = await VerificationCheckResource.CreateAsync(
                    to: phone,
                    code: code,
                    pathServiceSid: _settings.VerifyServiceSid
                );

                if (result.Status == "approved")
                {
                    _logger.LogInformation("Phone {Phone} verified successfully", phone);
                    return true;
                }

                if (result.Status == "pending")
                {
                    _logger.LogWarning("Verification code for {Phone} is still pending", phone);
                    return false;
                }

                _logger.LogWarning("Unexpected verification status {Status} for {Phone}", result.Status, phone);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to verify code for {Phone}", phone);
                return false;
            }
        }
    }
}
