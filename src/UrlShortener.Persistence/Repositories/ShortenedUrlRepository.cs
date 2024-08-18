using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Repositories;
using UrlShortener.Domain.ValueObjects;
using UrlShortener.Persistence.DbContexts;

namespace UrlShortener.Persistence.Repositories
{
    internal sealed class ShortenedUrlRepository :
        Repository<ShortenedUrl, ShortenedUrlId>,
        IShortenedUrlRepository
    {
        public ShortenedUrlRepository(ApplicationWriteDbContext dbContext) : base(dbContext)
        {
        }
    }
}