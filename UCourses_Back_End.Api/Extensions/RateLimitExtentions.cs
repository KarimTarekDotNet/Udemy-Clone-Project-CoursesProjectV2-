using System.Threading.RateLimiting;

namespace UCourses_Back_End.Api.Extensions
{
    public static class RateLimitExtentions
    {
        public static void AddRateLimitServices(this IServiceCollection services)
        {
            services.AddRateLimiter(op =>
            {
                op.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                // Strict rate limiting for authentication endpoints
                op.AddPolicy("authPolicy", context =>
                {
                    var key = context.User?.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";

                    return RateLimitPartition.GetSlidingWindowLimiter(key,
                    partition => new SlidingWindowRateLimiterOptions
                    {
                        Window = TimeSpan.FromMinutes(1),
                        PermitLimit = 5, // Only 5 auth attempts per minute
                        SegmentsPerWindow = 5,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 2
                    });
                });

                // Moderate rate limiting for general API endpoints
                op.AddPolicy("apiPolicy", context =>
                {
                    var key = context.User?.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";

                    return RateLimitPartition.GetSlidingWindowLimiter(key,
                    partition => new SlidingWindowRateLimiterOptions
                    {
                        Window = TimeSpan.FromMinutes(1),
                        PermitLimit = 30, // 30 requests per minute for general API
                        SegmentsPerWindow = 6,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 10
                    });
                });

                // Lenient rate limiting for public read-only endpoints
                op.AddPolicy("publicPolicy", context =>
                {
                    var key = context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";

                    return RateLimitPartition.GetSlidingWindowLimiter(key,
                    partition => new SlidingWindowRateLimiterOptions
                    {
                        Window = TimeSpan.FromMinutes(1),
                        PermitLimit = 60, // 60 requests per minute for public endpoints
                        SegmentsPerWindow = 6,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 20
                    });
                });

                // Legacy policy for backward compatibility
                op.AddPolicy("distributedPolicy", context =>
                {
                    var key = context.User?.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";

                    return RateLimitPartition.GetSlidingWindowLimiter(key,
                    partition => new SlidingWindowRateLimiterOptions
                    {
                        Window = TimeSpan.FromMinutes(1),
                        PermitLimit = 30,
                        SegmentsPerWindow = 6,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 10
                    });
                });
            });
        }
    }
}
