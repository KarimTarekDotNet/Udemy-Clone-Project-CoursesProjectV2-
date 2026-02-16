using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using UCourses_Back_End.Core.Entites.AuthModel;
using UCourses_Back_End.Core.Interfaces.IBackground;
using UCourses_Back_End.Core.Interfaces.IRepositories;
using UCourses_Back_End.Core.Interfaces.IServices;
using UCourses_Back_End.Infrastructure.BackgroundJobs;
using UCourses_Back_End.Infrastructure.Data;
using UCourses_Back_End.Infrastructure.RealTime;
using UCourses_Back_End.Infrastructure.Repositories.CoreRepo;
using UCourses_Back_End.Infrastructure.Repositories.UserRepo;
using UCourses_Back_End.Infrastructure.Services;
using UCourses_Back_End.Infrastructure.Services.PaymentService;
using UCourses_Back_End.Infrastructure.Services.SignInServices;
using UCourses_Back_End.Infrastructure.Services.UserServices;

namespace UCourses_Back_End.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFileService _fileService;
        private readonly IMailService _mailService;
        private readonly IPhoneVerificationService _phoneVerificationService;
        private readonly ILogger<SendConfirmationEmailJob> _loggerConfirmation;
        private readonly ILogger<SendPasswordResetEmailJob> _loggerPasswordReset;
        private readonly ILogger<AuthService> _loggerAuth;
        private readonly ILogger<ChatService> _loggerChat;
        private readonly ILogger<NotificationService> _loggerNotification;
        private readonly ILogger<PaymobService> _logger;
        private readonly IHubContext<NotificationHub, INotificationHub> _hubContext;

        public UnitOfWork(
            ApplicationDbContext context,
            UserManager<AppUser> userManager,
            IConfiguration configuration,
            IMapper mapper,
            IConnectionMultiplexer connectionMultiplexer,
            IHttpContextAccessor httpContextAccessor,
            IFileService fileService,
            IMailService mailService,
            IPhoneVerificationService phoneVerificationService,
            ILogger<SendConfirmationEmailJob> loggerConfirmation,
            ILogger<SendPasswordResetEmailJob> loggerPasswordReset,
            ILogger<AuthService> loggerAuth,
            ILogger<ChatService> loggerChat,
            ILogger<NotificationService> loggerNotification,
            IHubContext<NotificationHub, INotificationHub> hubContext,
            ILogger<PaymobService> logger)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _mapper = mapper;
            _connectionMultiplexer = connectionMultiplexer;
            _httpContextAccessor = httpContextAccessor;
            _fileService = fileService;
            _mailService = mailService;
            _phoneVerificationService = phoneVerificationService;
            _loggerConfirmation = loggerConfirmation;
            _loggerPasswordReset = loggerPasswordReset;
            _loggerAuth = loggerAuth;
            _loggerChat = loggerChat;
            _loggerNotification = loggerNotification;
            _logger = logger;
            _hubContext = hubContext;

            TokenService = new TokenService(_context, _userManager, _configuration, httpContextAccessor);
            RedisService = new RedisService(_connectionMultiplexer, configuration);
            SendConfirmationEmailJob = new SendConfirmationEmailJob(_mailService, _loggerConfirmation);
            SendPasswordResetEmailJob = new SendPasswordResetEmailJob(_mailService, _loggerPasswordReset);
            AuthService = new AuthService(_context, _userManager, TokenService, _mapper, RedisService,
            _httpContextAccessor, SendConfirmationEmailJob, SendPasswordResetEmailJob, _loggerAuth,
            _phoneVerificationService);
            RefreshJob = new RefreshTokenCleanupJob(_context);
            UserProfileService = new UserProfileService(_userManager, _fileService, _phoneVerificationService);
            DepartmentRepository = new DepartmentRepository(_context, _mapper, _fileService);
            CourseRepository = new CourseRepository(_context, _mapper, _fileService);
            SectionRepository = new SectionRepository(_context, _mapper, _fileService);
            EnrollmentRepository = new EnrollmentRepository(_context, _mapper);
            InstructorRepository = new InstructorRepository(_context, _mapper, _mailService);
            CourseProgressRepository = new CourseProgressRepository(_context, _mapper);
            AdminRepository = new AdminRepository(_context, _mapper);
            ChatService = new ChatService(_mapper, _context, _loggerChat, _userManager, _hubContext);
            NotificationService = new NotificationService(_hubContext, _context, _loggerNotification);
            PaymobService = new PaymobService(_configuration, _context, _mapper, _logger);
        }

        public IAuthService AuthService { get; }
        public ITokenService TokenService { get; }
        public IRedisService RedisService { get; }
        public IRefreshTokenCleanupJob RefreshJob { get; }
        public IUserProfileService UserProfileService { get; }
        public ISendConfirmationEmailJob SendConfirmationEmailJob { get; }
        public ISendPasswordResetEmailJob SendPasswordResetEmailJob { get; }
        public IDepartmentRepository DepartmentRepository { get; }
        public ICourseRepository CourseRepository { get; }
        public ISectionRepository SectionRepository { get; }
        public IEnrollmentRepository EnrollmentRepository { get; }
        public IInstructorRepository InstructorRepository { get; }
        public ICourseProgressRepository CourseProgressRepository { get; }
        public IAdminRepository AdminRepository { get; }
        public IChatService ChatService { get; }
        public INotificationService NotificationService { get; }
        public IPaymobService PaymobService { get; }
    }
}