using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using UCourses_Back_End.Core.Interfaces.IServices;
using UCourses_Back_End.Core.Settings;
using MailKit.Security;

namespace UCourses_Back_End.Infrastructure.Services.HelperService
{
    public class MailService : IMailService
    {
        private readonly MailSettings mailSettings;

        public MailService(IOptions<MailSettings> mailSettings)
        {
            this.mailSettings = mailSettings.Value;
        }

        public async Task SendEmail(string mailTo, string subject, string body, IList<IFormFile>? attachments = null)
        {
            var email = new MimeMessage
            {
                Sender = MailboxAddress.Parse(mailSettings.Email),
                Subject = subject
            };

            email.To.Add(MailboxAddress.Parse(mailTo));

            var builder = new BodyBuilder();

            if(attachments != null)
            {
                foreach (var item in attachments)
                {
                    if(item.Length > 0)
                    {
                        using var ms = new MemoryStream();
                        await item.CopyToAsync(ms);
                        var fileBytes = ms.ToArray();
                        builder.Attachments.Add(item.FileName, fileBytes, ContentType.Parse(item.ContentType));
                    }
                }
            }

            builder.HtmlBody = body;
            email.Body = builder.ToMessageBody();
            email.From.Add(new MailboxAddress(mailSettings.DisplayName, mailSettings.Email));

            using var smtp = new SmtpClient();
            
            await smtp.ConnectAsync(mailSettings.Host, mailSettings.Port, SecureSocketOptions.StartTls);
            
            // For SendGrid: username is "apikey" and password is the API key
            if (mailSettings.Host.Contains("sendgrid", StringComparison.OrdinalIgnoreCase))
            {
                await smtp.AuthenticateAsync("apikey", mailSettings.Password);
            }
            else
            {
                await smtp.AuthenticateAsync(mailSettings.Email, mailSettings.Password);
            }
            
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
