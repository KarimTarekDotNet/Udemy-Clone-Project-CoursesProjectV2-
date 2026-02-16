using UCourses_Back_End.Core.Interfaces.IBackground;
using UCourses_Back_End.Core.Interfaces.IServices;

namespace UCourses_Back_End.Core.Interfaces.IRepositories
{
    public interface IUnitOfWork
    {
        IAuthService AuthService { get; }
        ITokenService TokenService { get; }
        IRedisService RedisService { get; }
        IRefreshTokenCleanupJob RefreshJob { get; }
        ISendConfirmationEmailJob SendConfirmationEmailJob { get; }
        ISendPasswordResetEmailJob SendPasswordResetEmailJob { get; }
        IUserProfileService UserProfileService { get; }
        IDepartmentRepository DepartmentRepository { get; }
        ICourseRepository CourseRepository { get; }
        ISectionRepository SectionRepository { get; }
        IEnrollmentRepository EnrollmentRepository { get; }
        IInstructorRepository InstructorRepository { get; }
        ICourseProgressRepository CourseProgressRepository { get; }
        IAdminRepository AdminRepository { get; }
        IChatService ChatService { get; }
        INotificationService NotificationService { get; }
        IPaymobService PaymobService { get; }
    }
}
