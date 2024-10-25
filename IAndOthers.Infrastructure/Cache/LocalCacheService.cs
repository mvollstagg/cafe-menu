using IAndOthers.Application.Cache.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace IAndOthers.Infrastructure.Cache
{
    public class LocalCacheService : ILocalCacheService
    {
        private readonly IMemoryCache _memoryCache;

        public LocalCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void Set<T>(string key, T value, TimeSpan expiration)
        {
            _memoryCache.Set(key, value, expiration);
        }

        public T Get<T>(string key)
        {
            return _memoryCache.TryGetValue(key, out T value) ? value : default;
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }
    }
}
