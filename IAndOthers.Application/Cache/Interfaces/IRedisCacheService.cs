namespace IAndOthers.Application.Cache.Interfaces
{
    public interface IRedisCacheService
    {
        // Basic Key-Value Operations
        Task SetAsync<T>(string key, T value, TimeSpan expiration);
        Task<T> GetAsync<T>(string key);
        Task RemoveAsync(string key);
        Task<bool> ExistsAsync(string key);
        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> valueFactory, TimeSpan expiration);
        Task<T> GetAndRemoveAsync<T>(string key);
        Task<bool> SetIfNotExistsAsync(string key, string value, TimeSpan expiration);

        // List Operations
        Task AddToListAsync<T>(string key, T value);
        Task<List<T>> GetListAsync<T>(string key);
        Task<T> PopFromListAsync<T>(string key);
        Task<long> GetListLengthAsync(string key);
        Task TrimListAsync(string key, int start, int stop);

        // Set Operations (for unique values)
        Task AddToSetAsync<T>(string key, T value);
        Task<List<T>> GetSetAsync<T>(string key);
        Task RemoveFromSetAsync<T>(string key, T value);
        Task<bool> SetContainsAsync<T>(string key, T value);

        // Sorted Set Operations (for tracking with scores like timestamps)
        Task AddToSortedSetAsync<T>(string key, T value, double score);
        Task<List<T>> GetSortedSetRangeByScoreAsync<T>(string key, double minScore, double maxScore);
        Task RemoveFromSortedSetAsync<T>(string key, T value);
        Task<long> GetSortedSetLengthAsync(string key);
    }
}
