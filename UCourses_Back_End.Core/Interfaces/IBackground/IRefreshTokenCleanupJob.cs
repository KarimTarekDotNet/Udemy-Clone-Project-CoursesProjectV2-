namespace UCourses_Back_End.Core.Interfaces.IBackground
{
    public interface IRefreshTokenCleanupJob
    {
        Task Execute(string userId);
        Task ExecuteAllUsers();
    }
}