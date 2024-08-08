namespace UrlShortener.Application.Abstractions.Caching
{
    public interface ICacheProvider
    {
        Task<T?> GetAsync<T>(string key);

        Task<T?> HashGetAsync<T>(string key, string itemKey);

        Task<bool> HashRemoveAsync(string key, string itemKey);

        Task<bool> HashSetAsync<T>(string key, string itemKey, T data);

        Task<long> KeyDeleteAsync(IEnumerable<string> keys);

        Task<bool> KeyDeleteAsync(string key);

        Task<bool> KeyExpireAsync(string key, TimeSpan expiry);

        Task<bool> RemoveAsync(string key);

        Task<bool> SetAsync<T>(string key, T data);

        Task<bool> SetAsync<T>(string key, T data, TimeSpan? expiry);

        Task<double> StringDecrementAsync(string key, double val = 1);

        Task<double> StringIncrementAsync(string key, double val = 1);
    }
}