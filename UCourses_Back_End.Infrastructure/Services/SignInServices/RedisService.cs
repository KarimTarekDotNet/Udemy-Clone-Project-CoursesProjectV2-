using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using UCourses_Back_End.Core.Interfaces.IServices;

namespace UCourses_Back_End.Infrastructure.Services.SignInServices
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _database;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IConfiguration _configuration;

        public RedisService(IConnectionMultiplexer connectionMultiplexer, IConfiguration configuration)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _database = connectionMultiplexer.GetDatabase();
            _configuration = configuration;
        }

        public async Task<bool> SetAsync(string key, string value, TimeSpan expiry)
        {
            return await _database.StringSetAsync(key, value, expiry);
        }

        public async Task<string?> GetAsync(string key)
        {
            var value = await _database.StringGetAsync(key);

            return value.IsNull ? null : value.ToString();
        }
        public async Task<IEnumerable<string>> GetKeysValuesAsync(string pattern)
        {
            var server = _connectionMultiplexer.GetServer(_configuration["Redis:Host"]!,
                int.Parse(_configuration["Redis:Port"]!));
            var keys = server.Keys(pattern: pattern).ToArray();

            var tasks = keys.Select(key => _database.StringGetAsync(key));
            var results = await Task.WhenAll(tasks);

            return results.Where(v => !v.IsNull).Select(v => v.ToString());
        }
        public async Task<bool> DeleteAsync(string key)
        {
            return await _database.KeyDeleteAsync(key);
        }

        public async Task<bool> BlacklistTokenAsync(string token, TimeSpan expiry)
        {
            var key = $"blacklist:{token}";
            return await _database.StringSetAsync(key, "revoked", expiry);
        }

        public async Task<bool> IsTokenBlacklistedAsync(string token)
        {
            var key = $"blacklist:{token}";
            return await _database.KeyExistsAsync(key);
        }
    }
}