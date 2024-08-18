using Microsoft.EntityFrameworkCore;
using UrlShortener.Persistence.DbContexts;

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
            optionsBuilder.UseMySql(connectionString, serverVersion, options =>
            {
                //options.MigrationsHistoryTable("__MyMigrationsHistory", "devtips_audit_trails");
            });

            return optionsBuilder;
        }
    }
}