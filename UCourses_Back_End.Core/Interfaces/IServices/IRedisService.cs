namespace UCourses_Back_End.Core.Interfaces.IServices
{
    public interface IRedisService
    {
        Task<bool> SetAsync(string key, string value, TimeSpan expiry);
        Task<string?> GetAsync(string key);
        Task<IEnumerable<string>> GetKeysValuesAsync(string pattern);
        Task<bool> DeleteAsync(string key);
        Task<bool> BlacklistTokenAsync(string token, TimeSpan expiry);
        Task<bool> IsTokenBlacklistedAsync(string token);
    }
}
