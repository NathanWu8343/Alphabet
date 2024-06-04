using StackExchange.Redis;

namespace UrlShortener.Infrastructure.Redis
{
    public interface IRedisConnectionFactory
    {
        IConnectionMultiplexer GetConnection();
    }

    internal class RedisConnectionFactory : IRedisConnectionFactory
    {
        private readonly string _connectionString;

        public RedisConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// 锁
        /// </summary>
        private readonly object _lock = new();

        /// <summary>
        /// 连接对象
        /// </summary>
        private IConnectionMultiplexer _connection;

        /// <summary>
        /// 获取连接
        /// </summary>
        /// <returns></returns>
        public IConnectionMultiplexer GetConnection()
        {
            if (_connection != null && _connection.IsConnected)
            {
                return _connection;
            }
            lock (_lock)
            {
                if (_connection != null && _connection.IsConnected)
                {
                    return _connection;
                }

                if (_connection != null)
                {
                    _connection.Dispose();
                }
                _connection = ConnectionMultiplexer.Connect(_connectionString);
            }

            return _connection;
        }
    }
}