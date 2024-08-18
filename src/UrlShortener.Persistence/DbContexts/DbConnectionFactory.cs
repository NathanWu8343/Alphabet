using MySqlConnector;
using System.Data;

namespace UrlShortener.Persistence.DbContexts
{
    internal sealed class DbConnectionFactory(MySqlConnection connection)
    {
        public IDbConnection GetConnection()
        {
            return connection;
        }
    }
}