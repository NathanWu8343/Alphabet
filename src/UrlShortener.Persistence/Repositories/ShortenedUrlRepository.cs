using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Repositories;
using UrlShortener.Domain.ValueObjects;

namespace UrlShortener.Persistence.Repositories
{
    internal sealed class ShortenedUrlRepository :
        Repository<ShortenedUrl, ShortenedUrlId>,
        IShortenedUrlRepository
    {
        public ShortenedUrlRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}