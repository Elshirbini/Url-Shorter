using System.Text.Json;
using StackExchange.Redis;

namespace UrlShorter.Common.Redis;

public class RedisClient : IRedisClient
{
    private readonly IDatabase _db;

    public RedisClient(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    // ============================
    // String
    // ============================

    public Task<bool> SetAsync<T>(
        string key,
        T value,
        TimeSpan expiry)
    {
        string data = value is string s
        ? s
        : JsonSerializer.Serialize(value);

        return _db.StringSetAsync(key, data, expiry);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        RedisValue value = await _db.StringGetAsync(key);

        if (value.IsNull)
            return default;

        return JsonSerializer.Deserialize<T>(value.ToString());
    }

    public Task<bool> DeleteAsync(string key)
    {
        return _db.KeyDeleteAsync(key);
    }

    // ============================
    // Key
    // ============================

    public Task<bool> ExistsAsync(string key)
    {
        return _db.KeyExistsAsync(key);
    }

    public Task<bool> ExpireAsync(
        string key,
        TimeSpan expiry)
    {
        return _db.KeyExpireAsync(key, expiry);
    }

    // ============================
    // Counter
    // ============================

    public Task<long> IncrementAsync(
        string key,
        long value = 1)
    {
        return _db.StringIncrementAsync(key, value);
    }

    public Task<long> DecrementAsync(
        string key,
        long value = 1)
    {
        return _db.StringDecrementAsync(key, value);
    }

    // ============================
    // Lock
    // ============================

    public Task<bool> LockTakeAsync(
        string key,
        string value,
        TimeSpan expiry)
    {
        return _db.LockTakeAsync(key, value, expiry);
    }

    public Task<bool> LockReleaseAsync(
        string key,
        string value)
    {
        return _db.LockReleaseAsync(key, value);
    }
}