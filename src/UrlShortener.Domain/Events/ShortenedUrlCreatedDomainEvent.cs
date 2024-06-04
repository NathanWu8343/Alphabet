using SharedKernel.Core;
using UrlShortener.Domain.ValueObjects;

namespace UrlShortener.Domain.Events
{
    /// <summary>
    ///
    /// </summary>
    public sealed record ShortenedUrlCreatedDomainEvent(ShortenedUrlId ShortenedUrlId) : IDomainEvent
    {
        public Guid Id { get; init; } = Guid.NewGuid();
    }
}