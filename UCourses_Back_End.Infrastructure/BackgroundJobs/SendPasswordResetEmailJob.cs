using Microsoft.Extensions.Logging;
using UCourses_Back_End.Core.Interfaces.IBackground;
using UCourses_Back_End.Core.Interfaces.IServices;

namespace UCourses_Back_End.Infrastructure.BackgroundJobs
{
    public class SendPasswordResetEmailJob : ISendPasswordResetEmailJob
    {
        private readonly IMailService mailService;
        private readonly ILogger<SendPasswordResetEmailJob> logger;

        public SendPasswordResetEmailJob(IMailService mailService, ILogger<SendPasswordResetEmailJob> logger)
        {
            this.mailService = mailService;
            this.logger = logger;
        }

        public async Task SendPasswordResetEmail(string email, string code)
        {
            var subject = "Reset Your Password - UCourses";
            var body = $@"
                <div style='font-family: Arial, sans-serif; padding: 20px;'>
                    <h2>Password Reset Request</h2>
                    <p>You have requested to reset your password.</p>
                    <p>Your password reset code is: <strong style='font-size: 24px; color: #007bff;'>{code}</strong></p>
                    <p>This code will expire in 15 minutes.</p>
                    <p>If you didn't request this, please ignore this email.</p>
                    <br/>
                    <p>Thanks,<br/>UCourses Team</p>
                </div>
            ";

            try
            {
                await mailService.SendEmail(email, subject, body);
                logger.LogInformation("Password reset email sent successfully to {Email}", email);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send password reset email to {Email}", email);
                throw;
            }
        }
    }
}
