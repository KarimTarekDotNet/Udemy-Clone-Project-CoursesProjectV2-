using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace UCourses_Back_End.Infrastructure.RealTime
{
    public class ServerTimeNotification : BackgroundService
    {
        private static TimeSpan period = TimeSpan.FromSeconds(10);
        private readonly ILogger<ServerTimeNotification> _logger;
        private readonly IHubContext<NotificationHub, INotificationHub> context;

        public ServerTimeNotification(ILogger<ServerTimeNotification> logger, IHubContext<NotificationHub, INotificationHub> context)
        {
            _logger = logger;
            this.context = context;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var timer = new PeriodicTimer(period);

            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                var date = DateTime.UtcNow;
                _logger.LogInformation($"ServerTimeNotification tick: {date}");

                var users = NotificationHub.GetAllConnectedUsers();
                foreach (var userId in users)
                {
                    await context.Clients.User(userId)
                        .ReceiveNotification($"Server Time = {date}");
                }
            }
        }
    }
}