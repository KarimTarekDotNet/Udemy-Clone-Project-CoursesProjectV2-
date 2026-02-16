using FluentValidation;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using nClam;
using StackExchange.Redis;
using UCourses_Back_End.Core.Entites.AuthModel;
using UCourses_Back_End.Core.Interfaces.IBackground;
using UCourses_Back_End.Core.Interfaces.IRepositories;
using UCourses_Back_End.Core.Interfaces.IServices;
using UCourses_Back_End.Core.Validators.AuthValidator;
using UCourses_Back_End.Infrastructure.BackgroundJobs;
using UCourses_Back_End.Infrastructure.Data;
using UCourses_Back_End.Infrastructure.Repositories;
using UCourses_Back_End.Infrastructure.Services.HelperService;
using UCourses_Back_End.Infrastructure.Services.SignInServices;

namespace UCourses_Back_End.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.AddSingleton(new ClamClient("localhost", 3310));
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var connectionString = configuration.GetConnectionString("Redis") ?? throw new Exception("Not Implement");
                return ConnectionMultiplexer.Connect(connectionString);
            });
            services.AddHangfire(config =>
                config.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));

            services.AddHangfireServer();

            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IRedisService, RedisService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IPhoneVerificationService, TwilioVerifyService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IGoogleAuthService, GoogleAuthService>();
            services.AddScoped<IAuditLogService, AuditLogService>();
            
            // Background Jobs
            services.AddScoped<ISendConfirmationEmailJob, SendConfirmationEmailJob>();
            services.AddScoped<ISendPasswordResetEmailJob, SendPasswordResetEmailJob>();
            services.AddScoped<IRefreshTokenCleanupJob, RefreshTokenCleanupJob>();
            
            services.AddValidatorsFromAssembly(typeof(RegisterValidator).Assembly);

            return services;
        }

    }
}