using UCourses_Back_End.Api.Middlewares;
using UCourses_Back_End.Infrastructure.RealTime;

namespace UCourses_Back_End.Api.Extensions
{
    public static class MiddlewareExtensions
    {
        public static void UseCustomMiddlewares(this WebApplication app)
        {
            app.UseRateLimiter();
            app.UseMiddleware<GlobalExceptionsMiddleware>();
            
            // HSTS - Force HTTPS for 1 year in production
            if (!app.Environment.IsDevelopment())
            {
                app.UseHsts();
            }
            
            app.UseCors("AllowSpecificOrigin");
            app.UseHttpsRedirection();
            app.UseStaticFiles(); // Serve static files from wwwroot
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHub<NotificationHub>("/hubs/notifications");
        }
    }
}