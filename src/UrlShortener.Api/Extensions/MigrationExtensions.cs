using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using UrlShortener.Persistence.DbContexts;

namespace UrlShortener.Api.Extensions
{
    public static class MigrationExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();

            using ApplicationWriteDbContext dbContext =
                scope.ServiceProvider.GetRequiredService<ApplicationWriteDbContext>();

            // crate
            var dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();
            if (!dbCreator.Exists()) dbCreator.Create();

            // migrate
            dbContext.Database.Migrate();
        }
    }
}