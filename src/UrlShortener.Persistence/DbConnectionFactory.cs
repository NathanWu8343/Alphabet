using MySqlConnector;
using System.Data;

namespace UrlShortener.Persistence
{
    internal sealed class DbConnectionFactory(MySqlConnection connection)
    {
        public IDbConnection OpenConnection()
        {
            return connection;
        }
    }
}