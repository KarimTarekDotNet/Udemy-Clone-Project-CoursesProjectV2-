using Hangfire;
using UCourses_Back_End.Api.Filters;
using UCourses_Back_End.Infrastructure.BackgroundJobs;

namespace UCourses_Back_End.Api.Extensions
{
    public static class HangfireExtensions
    {
        public static void UseCustomHangfire(this WebApplication app)
        {
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationFilter() }
            });

            RecurringJob.AddOrUpdate<RefreshTokenCleanupJob>(
                "cleanup-refresh-tokens",
                job => job.ExecuteAllUsers(),
                Cron.Hourly
            );
        }
    }
}
