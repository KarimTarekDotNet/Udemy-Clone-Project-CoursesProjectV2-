using UCourses_Back_End.Api.Extensions;
using UCourses_Back_End.Core.Role;

namespace UCourses_Back_End.Api
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddCustomServices(builder.Configuration);
            builder.Services.AddRateLimitServices();
            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                await SeedData.InitializeAsync(scope.ServiceProvider);
            }

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCustomMiddlewares();
            app.UseCustomHangfire();

            app.MapControllers().RequireRateLimiting("distributedPolicy");

            app.Run();
        }
    }
}