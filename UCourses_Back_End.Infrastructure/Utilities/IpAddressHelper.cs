using Microsoft.AspNetCore.Http;

namespace UCourses_Back_End.Infrastructure.Utilities
{
    public static class IpAddressHelper
    {
        public static string GetClientIpAddress(IHttpContextAccessor httpContextAccessor)
        {
            try
            {
                var httpContext = httpContextAccessor?.HttpContext;
                if (httpContext == null)
                    return "Unknown";

                // Try X-Forwarded-For header (for proxies/load balancers)
                var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (!string.IsNullOrEmpty(forwardedFor))
                {
                    var ips = forwardedFor.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (ips.Length > 0)
                    {
                        var ip = ips[0].Trim();
                        if (!string.IsNullOrEmpty(ip) && ip != "::1")
                            return ip;
                    }
                }

                // Try X-Real-IP header
                var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
                if (!string.IsNullOrEmpty(realIp) && realIp != "::1")
                    return realIp;

                // Get from connection
                var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString();
                
                // Convert IPv6 loopback to IPv4
                if (remoteIp == "::1")
                    remoteIp = "127.0.0.1";
                
                return remoteIp ?? "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}
