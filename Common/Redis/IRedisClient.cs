using StackExchange.Redis;

namespace UrlShorter.Common.Redis;

public interface IRedisClient
{
    // String
    Task<bool> SetAsync<T>(string key, T value, TimeSpan expiry);
    Task<T?> GetAsync<T>(string key);
    Task<bool> DeleteAsync(string key);

    // Key
    Task<bool> ExistsAsync(string key);
    Task<bool> ExpireAsync(string key, TimeSpan expiry);
    Task<long> DeleteManyAsync(IEnumerable<string> keys);

    // Counter
    Task<long> IncrementAsync(string key, long value = 1);
    Task<long> DecrementAsync(string key, long value = 1);

    // Sets
    Task<bool> AddToSetAsync(string key, string value);

    Task<string[]> GetSetMembersAsync(string key);

    Task<bool> RemoveSetAsync(string key);


    // Lock
    Task<bool> LockTakeAsync(string key, string value, TimeSpan expiry);
    Task<bool> LockReleaseAsync(string key, string value);
}