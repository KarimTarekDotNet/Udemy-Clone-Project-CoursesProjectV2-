namespace UCourses_Back_End.Core.Interfaces.IBackground
{
    public interface ISendPasswordResetEmailJob
    {
        Task SendPasswordResetEmail(string email, string code);
    }
}
