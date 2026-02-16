using Microsoft.AspNetCore.Http;

namespace UCourses_Back_End.Core.Interfaces.IServices
{
    public interface IMailService
    {
        Task SendEmail(string mailTo, string subject, string body, IList<IFormFile>? attachments = null);
    }
}