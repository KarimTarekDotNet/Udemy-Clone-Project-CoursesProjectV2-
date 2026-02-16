namespace UCourses_Back_End.Core.Interfaces.IBackground
{
    public interface ISendConfirmationEmailJob
    {
        Task SendConfirmationEmail(string email, string subject, string body);
        Task SendVerificationCodeEmail(string email, string code);
    }
}
