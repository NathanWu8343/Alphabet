using UrlShortener.Domain.ValueObjects;

namespace UrlShortener.Application.Models
{
    public sealed class ShortenUrlReadModel
    {
        public ShortenedUrlId Id { get; set; }
        public string Code { get; set; }

        public string ShortUrl { get; set; }

        public DateTime CreatedAtUtc { get; private set; }
    }
}