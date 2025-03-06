using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using TaskManagement.API.Interfaces.Persistence.Cache;

namespace TaskManagement.API.Data.Cache
{
    internal class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        public CacheService(IDistributedCache cache) => _cache = cache;

        public async Task<T> GetOrCreateAsync<T>(
        string cacheKey,
        Func<Task<T>> retrieveDataFunc,
        TimeSpan? slidingExpiration = null)
        {
            // Try to get the data from the cache
            var cachedDataString = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedDataString))
            {
                return JsonSerializer.Deserialize<T>(cachedDataString);
            }

            // Data not in cache, retrieve it
            T cachedData = await retrieveDataFunc();

            // Serialize the data
            var serializedData = JsonSerializer.Serialize(cachedData);

            // Set cache options
            var cacheEntryOptions = new DistributedCacheEntryOptions
            {
                SlidingExpiration = slidingExpiration ?? TimeSpan.FromMinutes(60)
            };

            // Save data in cache
            await _cache.SetStringAsync(cacheKey, serializedData, cacheEntryOptions);

            return cachedData;
        }
    }
}
