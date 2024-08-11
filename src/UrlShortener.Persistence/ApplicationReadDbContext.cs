using Microsoft.EntityFrameworkCore;
using UrlShortener.Application.Abstractions.Data;
using UrlShortener.Application.Models;
using UrlShortener.Persistence.Extensions;

namespace UrlShortener.Persistence
{
    public sealed class ApplicationReadDbContext(DbContextOptions<ApplicationReadDbContext> options)
        : DbContext(options), IReadDbContext

    {
        public DbSet<ShortenUrlReadModel> ShortendUrls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) =>
            modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly, ReadConfigurationsFilter)
                        .ApplyUtcDateTimeConverter();

        private static bool ReadConfigurationsFilter(Type type) =>
                type.FullName?.Contains("Configurations.Read") ?? false;
    }
}