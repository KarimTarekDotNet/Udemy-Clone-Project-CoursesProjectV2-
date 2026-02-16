using Hangfire.Annotations;
using Hangfire.Dashboard;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace UCourses_Back_End.Api.Filters
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            // Try to get token from Authorization header first
            var authHeader = httpContext.Request.Headers["Authorization"].ToString();
            string? token = null;

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                token = authHeader.Replace("Bearer ", "");
            }
            // Fallback: Try to get token from cookie
            else if (httpContext.Request.Cookies.TryGetValue("accessToken", out var cookieToken))
            {
                token = cookieToken;
            }

            if (string.IsNullOrEmpty(token)) 
                return false;

            try
            {
                var handler = new JwtSecurityTokenHandler();
                
                // Validate token can be read
                if (!handler.CanReadToken(token))
                    return false;

                var jwt = handler.ReadJwtToken(token);

                // Check if token is expired
                if (jwt.ValidTo < DateTime.UtcNow)
                    return false;

                // Check for Admin role
                var roleClaim = jwt.Claims.FirstOrDefault(c => 
                    c.Type.Equals("role", StringComparison.OrdinalIgnoreCase) ||
                    c.Type.Equals(ClaimTypes.Role, StringComparison.OrdinalIgnoreCase)
                )?.Value;

                return roleClaim != null && roleClaim.Equals("Admin", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false; // Invalid token
            }
        }
    }
}