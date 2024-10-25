using IAndOthers.Application.Cache.Interfaces;
using IAndOthers.Core.IoC;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace IAndOthers.Infrastructure.Cache
{
    public class RedisCacheService : IRedisCacheService, IIODependencyScoped<IRedisCacheService>
    {
        private readonly IDatabase _database;

        public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
        {
            _database = connectionMultiplexer.GetDatabase();
        }

        // General Methods
        public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            var jsonData = JsonConvert.SerializeObject(value);
            await _database.StringSetAsync(key, jsonData, expiration);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var jsonData = await _database.StringGetAsync(key);
            return jsonData.HasValue ? JsonConvert.DeserializeObject<T>(jsonData) : default;
        }

        public async Task RemoveAsync(string key)
        {
            await _database.KeyDeleteAsync(key);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            return await _database.KeyExistsAsync(key);
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> valueFactory, TimeSpan expiration)
        {
            var jsonData = await _database.StringGetAsync(key);
            if (jsonData.HasValue)
            {
                return JsonConvert.DeserializeObject<T>(jsonData);
            }

            var value = await valueFactory();
            await SetAsync(key, value, expiration);
            return value;
        }

        public async Task<T> GetAndRemoveAsync<T>(string key)
        {
            var value = await GetAsync<T>(key);
            if (value != null)
            {
                await RemoveAsync(key);
            }
            return value;
        }

        public async Task<bool> SetIfNotExistsAsync(string key, string value, TimeSpan expiration)
        {
            return await _database.StringSetAsync(key, value, expiration, When.NotExists);
        }

        // List Methods (for Redis lists)
        public async Task AddToListAsync<T>(string key, T value)
        {
            var jsonData = JsonConvert.SerializeObject(value);
            await _database.ListRightPushAsync(key, jsonData);
        }

        public async Task<List<T>> GetListAsync<T>(string key)
        {
            var redisValues = await _database.ListRangeAsync(key);
            return redisValues.Select(rv => JsonConvert.DeserializeObject<T>(rv)).ToList();
        }

        public async Task<T> PopFromListAsync<T>(string key)
        {
            var redisValue = await _database.ListRightPopAsync(key);
            return redisValue.HasValue ? JsonConvert.DeserializeObject<T>(redisValue) : default;
        }

        public async Task<long> GetListLengthAsync(string key)
        {
            return await _database.ListLengthAsync(key);
        }

        public async Task TrimListAsync(string key, int start, int stop)
        {
            await _database.ListTrimAsync(key, start, stop);
        }

        // Set Methods (for Redis sets, used for unique values like users)
        public async Task AddToSetAsync<T>(string key, T value)
        {
            var jsonData = JsonConvert.SerializeObject(value);
            await _database.SetAddAsync(key, jsonData);
        }

        public async Task<List<T>> GetSetAsync<T>(string key)
        {
            var redisValues = await _database.SetMembersAsync(key);
            return redisValues.Select(rv => JsonConvert.DeserializeObject<T>(rv)).ToList();
        }

        public async Task RemoveFromSetAsync<T>(string key, T value)
        {
            var jsonData = JsonConvert.SerializeObject(value);
            await _database.SetRemoveAsync(key, jsonData);
        }

        public async Task<bool> SetContainsAsync<T>(string key, T value)
        {
            var jsonData = JsonConvert.SerializeObject(value);
            return await _database.SetContainsAsync(key, jsonData);
        }

        // Sorted Set Methods (e.g., for chat activity tracking)
        public async Task AddToSortedSetAsync<T>(string key, T value, double score)
        {
            var jsonData = JsonConvert.SerializeObject(value);
            await _database.SortedSetAddAsync(key, jsonData, score);
        }

        public async Task<List<T>> GetSortedSetRangeByScoreAsync<T>(string key, double minScore, double maxScore)
        {
            var redisValues = await _database.SortedSetRangeByScoreAsync(key, minScore, maxScore);
            return redisValues.Select(rv => JsonConvert.DeserializeObject<T>(rv)).ToList();
        }

        public async Task RemoveFromSortedSetAsync<T>(string key, T value)
        {
            var jsonData = JsonConvert.SerializeObject(value);
            await _database.SortedSetRemoveAsync(key, jsonData);
        }

        public async Task<long> GetSortedSetLengthAsync(string key)
        {
            return await _database.SortedSetLengthAsync(key);
        }
    }
}
