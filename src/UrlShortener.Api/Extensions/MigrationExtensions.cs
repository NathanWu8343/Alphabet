using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using UrlShortener.Persistence;

namespace UrlShortener.Api.Extensions
{
    public static class MigrationExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();

            using ApplicationDbContext dbContext =
                scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // crate
            var dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();
            if (!dbCreator.Exists()) dbCreator.Create();

            // migrate
            dbContext.Database.Migrate();
        }
    }
}