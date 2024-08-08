using Microsoft.EntityFrameworkCore;
using UrlShortener.Application.Abstractions.Data;
using UrlShortener.Domain.Entities;
using UrlShortener.Persistence.Extensions;

namespace UrlShortener.Persistence
{
    public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : DbContext(options), IUnitOfWork, IDbContext

    {
        public DbSet<ShortenedUrl> ShortendUrls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) =>
            modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly)
                        .ApplyUtcDateTimeConverter();
    }
}