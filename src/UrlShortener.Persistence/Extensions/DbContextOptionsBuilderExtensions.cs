using Microsoft.EntityFrameworkCore;

namespace UrlShortener.Persistence.Extensions
{
    internal static class DbContextOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder UseMySql(
            this DbContextOptionsBuilder optionsBuilder,
            DbConnectionFactory dbConnectionFactory,
            ServerVersion serverVersion)
        {
            var connectionString = dbConnectionFactory.GetConnection().ConnectionString;
            optionsBuilder.UseMySql(connectionString, serverVersion);

            return optionsBuilder;
        }
    }
}