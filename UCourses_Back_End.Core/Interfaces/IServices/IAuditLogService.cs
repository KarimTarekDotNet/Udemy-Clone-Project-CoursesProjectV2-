namespace UCourses_Back_End.Core.Interfaces.IServices
{
    public interface IAuditLogService
    {
        Task LogAsync(string action, string entityType, string entityId, string? userId = null, string? details = null);
    }
}
