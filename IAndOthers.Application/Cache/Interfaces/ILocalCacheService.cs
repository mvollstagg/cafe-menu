namespace IAndOthers.Application.Cache.Interfaces
{
    public interface ILocalCacheService
    {
        void Set<T>(string key, T value, TimeSpan expiration);
        T Get<T>(string key);
        void Remove(string key);
    }
}
