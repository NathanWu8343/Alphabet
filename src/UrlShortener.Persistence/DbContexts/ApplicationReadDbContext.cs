using Microsoft.EntityFrameworkCore;
using UrlShortener.Application.Abstractions.Data;
using UrlShortener.Application.ReadModels;
using UrlShortener.Persistence.Extensions;

namespace UrlShortener.Persistence.DbContexts
{
    public sealed class ApplicationReadDbContext(DbContextOptions<ApplicationReadDbContext> options)
        : DbContext(options), IReadDbContext

    {
        public DbSet<ShortenUrlReadModel> ShortendUrls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) =>
            modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly, ReadConfigurationsFilter)
                        .ApplyUtcDateTimeConverter();

        private static bool ReadConfigurationsFilter(Type type) =>
                type.FullName?.Contains(nameof(Configurations.Read)) ?? false;
    }
}