using Microsoft.EntityFrameworkCore;
using UrlShortener.Application.Abstractions.Data;
using UrlShortener.Domain.Entities;
using UrlShortener.Persistence.Extensions;

namespace UrlShortener.Persistence.DbContexts
{
    public sealed class ApplicationWriteDbContext(DbContextOptions<ApplicationWriteDbContext> options)
        : DbContext(options), IUnitOfWork, IWriteDbContext

    {
        public DbSet<ShortenedUrl> ShortendUrls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) =>
            modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly, WriteConfigurationsFilter)
                        .ApplyUtcDateTimeConverter();

        private static bool WriteConfigurationsFilter(Type type) =>
                type.FullName?.Contains(nameof(Configurations.Write)) ?? false;
    }
}