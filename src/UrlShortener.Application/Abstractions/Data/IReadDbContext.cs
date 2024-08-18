using Microsoft.EntityFrameworkCore;
using UrlShortener.Application.ReadModels;

namespace UrlShortener.Application.Abstractions.Data
{
    /// <summary>
    /// Represents the application database context interface.
    /// </summary>
    public interface IReadDbContext
    {
        public DbSet<ShortenUrlReadModel> ShortendUrls { get; }
    }
}