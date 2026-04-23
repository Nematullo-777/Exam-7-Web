using Application.Interfaces.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class MemoryCacheService(IMemoryCache _cache, ILogger<MemoryCacheService> _logger) : ICacheService
{
    public Task<T?> GetAsync<T>(string key, CancellationToken token = default)
    {
        if (_cache.TryGetValue(key, out T? value))
        {
            _logger.LogInformation("Cache hit: {Key}", key);
            return Task.FromResult(value);
        }

        _logger.LogInformation("Cache miss: {Key}", key);
        return Task.FromResult(default(T));
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow = null, CancellationToken token = default)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow ?? TimeSpan.FromMinutes(10)
        };

        _cache.Set(key, value, options);
        _logger.LogInformation("Cache set: {Key}", key);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken token = default)
    {
        _cache.Remove(key);
        _logger.LogInformation("Cache removed: {Key}", key);
        return Task.CompletedTask;
    }
}
