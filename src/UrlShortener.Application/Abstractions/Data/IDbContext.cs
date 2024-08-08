using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Application.Abstractions.Data
{
    /// <summary>
    /// Represents the application database context interface.
    /// </summary>
    public interface IDbContext
    {
        public DbSet<ShortenedUrl> ShortendUrls { get; }
    }
}