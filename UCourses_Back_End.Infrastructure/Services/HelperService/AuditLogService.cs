using Microsoft.Extensions.Logging;
using UCourses_Back_End.Core.Interfaces.IServices;

namespace UCourses_Back_End.Infrastructure.Services.HelperService
{
    public class AuditLogService : IAuditLogService
    {
        private readonly ILogger<AuditLogService> _logger;

        public AuditLogService(ILogger<AuditLogService> logger)
        {
            _logger = logger;
        }

        public Task LogAsync(string action, string entityType, string entityId, string? userId = null, string? details = null)
        {
            var logMessage = $"AUDIT: Action={action}, EntityType={entityType}, EntityId={entityId}";
            
            if (!string.IsNullOrEmpty(userId))
                logMessage += $", UserId={userId}";
            
            if (!string.IsNullOrEmpty(details))
                logMessage += $", Details={details}";

            _logger.LogInformation(logMessage);
            
            return Task.CompletedTask;
        }
    }
}
