using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UCourses_Back_End.Api.Mappings.Core;
using UCourses_Back_End.Api.Mappings.Users;
using UCourses_Back_End.Core.Interfaces.IServices;
using UCourses_Back_End.Core.Settings;
using UCourses_Back_End.Core.Validators.AuthValidator;
using UCourses_Back_End.Infrastructure;
using UCourses_Back_End.Infrastructure.Data;
using UCourses_Back_End.Infrastructure.RealTime;

namespace UCourses_Back_End.Api.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddCustomServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Infrastructure
            services.AddInfrastructure(configuration);

            // HttpContextAccessor (required for getting client IP)
            services.AddHttpContextAccessor();

            // HSTS Configuration
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });

            services.AddSingleton<Microsoft.AspNetCore.SignalR.IUserIdProvider, NameUserIdProvider>();
            
            services.AddSignalR()
                .AddHubOptions<NotificationHub>(options =>
                {
                    options.EnableDetailedErrors = true;
                });

            services.AddHostedService<ServerTimeNotification>();

            // Configure Mail Settings
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            // Configure Phone Settings
            services.Configure<TwilioSettings>(configuration.GetSection("Twilio"));
            // Configure File Upload Settings
            services.Configure<FileUploadSettings>(configuration.GetSection("FileUpload"));

            // CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", policy =>
                {
                    policy.SetIsOriginAllowed(origin => true)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            // Authentication & JWT
            var key = Encoding.UTF8.GetBytes(configuration["JWT:Key"]!);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidAudience = configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }
                        
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = async context =>
                    {
                        var db = context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
                        var userId = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);

                        if (userId == null)
                        {
                            context.Fail("No user id claim.");
                            return;
                        }

                        var redis = context.HttpContext.RequestServices.GetRequiredService<IRedisService>();
                        var accessToken = context.SecurityToken as JwtSecurityToken;

                        // Check if token is blacklisted (only if accessToken is not null)
                        if (accessToken != null && !string.IsNullOrEmpty(accessToken.RawData))
                        {
                            if (await redis.IsTokenBlacklistedAsync(accessToken.RawData))
                            {
                                context.Fail("This access token has been revoked.");
                                return;
                            }
                        }

                        var now = DateTime.UtcNow;

                        var hasActiveTokens = await db.RefreshTokens
                            .Where(r => r.UserId == userId)
                            .AnyAsync(r => !r.IsRevoked && !r.IsUsed && r.ExpiryDate > now);

                        if (!hasActiveTokens)
                            context.Fail("This token has been revoked or user logged out.");
                    }
                };
            })
            .AddGoogle(options =>
            {
                options.ClientId = configuration["Authentication:Google:ClientId"]!;
                options.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;
                options.CallbackPath = "/signin-google";

                options.SaveTokens = true;

                options.Scope.Add("profile");
                options.Scope.Add("email");
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("InstructorOnly", policy => policy.RequireRole("Instructor"));
                options.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));
                options.AddPolicy("InstructorOrAdmin", policy =>
                    policy.RequireRole("Instructor", "Admin"));
            });
            // FluentValidation
            services.AddValidatorsFromAssemblyContaining<RegisterValidator>();
            services.AddFluentValidationAutoValidation();

            // AutoMapper
            services.AddAutoMapper(op =>
            {
                op.AddProfile<UserMapping>();
                op.AddProfile<DepartmentMapping>();
                op.AddProfile<CourseMapping>();
                op.AddProfile<SectionMapping>();
                op.AddProfile<EnrollmentMapping>();
                op.AddProfile<InstructorMapping>();
                op.AddProfile<CourseProgressMapping>();
                op.AddProfile<InstructorAnalyticsMapping>();
                op.AddProfile<AdminDashboardMapping>();
                op.AddProfile<ChatMappingProfile>();
            });

            // Swagger
            services.AddSwaggerGen();
            services.AddOpenApi();
        }
    }

}
