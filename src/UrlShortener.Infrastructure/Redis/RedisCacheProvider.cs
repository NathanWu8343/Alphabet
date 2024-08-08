using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;
using UrlShortener.Application.Abstractions.Caching;

namespace UrlShortener.Infrastructure.Redis
{
    public class RedisCacheProvider : ICacheProvider
    {
        private readonly string DefaultKey;

        private readonly IRedisConnectionFactory _connectionFactory;
        private readonly ILogger<RedisCacheProvider> _logger;

        public RedisCacheProvider(IRedisConnectionFactory connectionFactory, ILogger<RedisCacheProvider> logger)
        {
            DefaultKey = "US";
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        /// <summary>
        /// 获取数据库
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public IDatabase GetDatabase(int? db = null)
        {
            _logger.LogInformation("get redis");
            return _connectionFactory.GetConnection().GetDatabase(db ?? -1);
        }

        /// <summary>
        /// 删除指定数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<bool> RemoveAsync(string key)
        {
            key = AddKeyPrefix(key);
            return GetDatabase().KeyDeleteAsync(key);
        }

        /// <summary>
        /// 通过键获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T?> GetAsync<T>(string key)
        {
            key = AddKeyPrefix(key);
            RedisValue value = await GetDatabase().StringGetAsync(key);
            return value.HasValue ?
                 JsonSerializer.Deserialize<T?>(value!) : default;
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public Task<bool> SetAsync<T>(string key, T data, TimeSpan? expiry)
        {
            key = AddKeyPrefix(key);
            var value = JsonSerializer.Serialize(data);
            return GetDatabase().StringSetAsync(key, value, expiry);
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task<bool> SetAsync<T>(string key, T data)
        {
            return SetAsync(key, data, null);
        }

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public Task<double> StringIncrementAsync(string key, double val = 1)
        {
            key = AddKeyPrefix(key);
            return GetDatabase().StringIncrementAsync(key, val, CommandFlags.None);
        }

        /// <summary>
        /// 为数字递减val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public Task<double> StringDecrementAsync(string key, double val = 1)
        {
            key = AddKeyPrefix(key);
            return GetDatabase().StringDecrementAsync(key, val, CommandFlags.None);
        }

        /// <summary>
        /// 通过键 哈希键获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="itemKey"></param>
        /// <returns></returns>
        public async Task<T?> HashGetAsync<T>(string key, string itemKey)
        {
            key = AddKeyPrefix(key);
            RedisValue value = await GetDatabase().HashGetAsync(key, itemKey);
            return value.HasValue ?
                 JsonSerializer.Deserialize<T?>(value!) : default;
        }

        public Task<bool> HashSetAsync<T>(string key, string itemKey, T data)
        {
            key = AddKeyPrefix(key);
            var value = JsonSerializer.Serialize(data);
            return GetDatabase().HashSetAsync(key, itemKey, value);
        }

        public Task<bool> HashRemoveAsync(string key, string itemKey)
        {
            key = AddKeyPrefix(key);
            return GetDatabase().HashDeleteAsync(key, itemKey);
        }

        #region key 操作

        /// <summary>
        /// 移除指定 Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<bool> KeyDeleteAsync(string key)
        {
            var redisKey = AddKeyPrefix(key);
            return GetDatabase().KeyDeleteAsync(redisKey);
        }

        /// <summary>
        /// 移除指定 Key
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public Task<long> KeyDeleteAsync(IEnumerable<string> keys)
        {
            var redisKeys = keys.Select(x => (RedisKey)AddKeyPrefix(x));
            return GetDatabase().KeyDeleteAsync(redisKeys.ToArray());
        }

        /// <summary>
        /// 设置 Key 的时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public Task<bool> KeyExpireAsync(string key, TimeSpan expiry)
        {
            var redisKey = AddKeyPrefix(key);
            return GetDatabase().KeyExpireAsync(redisKey, expiry);
        }

        #endregion key 操作

        #region private method

        /// <summary>
        /// 添加 Key 的前缀
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string AddKeyPrefix(string key)
        {
            return $"{DefaultKey}:{key}";
        }

        #endregion private method
    }
}