using Microsoft.Extensions.Logging;
using UCourses_Back_End.Core.Interfaces.IBackground;
using UCourses_Back_End.Core.Interfaces.IServices;

namespace UCourses_Back_End.Infrastructure.BackgroundJobs
{
    public class SendConfirmationEmailJob : ISendConfirmationEmailJob
    {
        private readonly IMailService mailService;
        private readonly ILogger<SendConfirmationEmailJob> logger;

        public SendConfirmationEmailJob(IMailService mailService, ILogger<SendConfirmationEmailJob> logger)
        {
            this.mailService = mailService;
            this.logger = logger;
        }

        public async Task SendConfirmationEmail(string email, string subject, string body)
        {
            try
            {
                await mailService.SendEmail(email, subject, body);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send confirmation email to {Email}", email);
                throw;
            }
        }

        public async Task SendVerificationCodeEmail(string email, string code)
        {
            var subject = "Your verification code";
            var body = $"<p>Your verification code is: <strong>{code}</strong></p>";

            await SendConfirmationEmail(email, subject, body);
        }
    }
}