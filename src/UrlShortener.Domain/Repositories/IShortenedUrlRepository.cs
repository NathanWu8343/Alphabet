using UrlShortener.Domain.Entities;

namespace UrlShortener.Domain.Repositories
{
    public interface IShortenedUrlRepository
    {
        void Add(ShortenedUrl shrotendUrl);
    }
}