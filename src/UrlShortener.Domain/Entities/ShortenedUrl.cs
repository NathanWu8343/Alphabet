using SharedKernel.Common;
using SharedKernel.Core;
using UrlShortener.Domain.ValueObjects;

namespace UrlShortener.Domain.Entities
{
    public class ShortenedUrl : AggregateRoot<ShortenedUrlId>
    {
        public string LongUrl { get; private set; } = string.Empty;

        public string ShortUrl { get; private set; } = string.Empty;

        public string Code { get; private set; } = string.Empty;

        public DateTime CreatedAtUtc { get; private set; }

        private ShortenedUrl(ShortenedUrlId id, string longUrl, string shortUrl, string code, DateTime createdAtUtc)
         : base(id)
        {
            LongUrl = longUrl;
            ShortUrl = shortUrl;
            Code = code;
            CreatedAtUtc = createdAtUtc;
        }

        private ShortenedUrl()
        {
        }

        public static ShortenedUrl Create(string longUrl, string shortUrl, string code, DateTime createdAtUtc)
        {
            Ensure.NotEmpty(longUrl, "The long Url is required.");
            Ensure.NotEmpty(shortUrl, "The shor path is required.");
            Ensure.NotEmpty(code, "The code is required.");

            return new ShortenedUrl(new ShortenedUrlId(Guid.NewGuid()), longUrl, shortUrl, code, createdAtUtc);
        }
    }
}