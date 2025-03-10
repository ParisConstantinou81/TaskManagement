﻿namespace TaskManagement.API.Interfaces.Persistence.Cache
{
    public interface ICacheService
    {
        Task<T> GetOrCreateAsync<T>(
            string cacheKey,
            Func<Task<T>> retrieveDataFunc,
            TimeSpan? slidingExpiration = null);
    }
}
