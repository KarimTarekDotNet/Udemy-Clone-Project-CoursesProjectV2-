using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace UCourses_Back_End.Infrastructure.RealTime
{
    public class NameUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst("uid")?.Value ?? 
                   connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}