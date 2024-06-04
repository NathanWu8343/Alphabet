using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Events;

namespace UrlShortener.Application.UnitTests.UrlShorteners.Entities
{
    public class ShortenedUrlTests
    {
        [Fact]
        public void Craete_Should_RasieDomainEvent()
        {
            // Act
            var shortenedUrl = ShortenedUrl.Create(
                "http://google.com",
                "http://localhost/xxx/api/xxx",
                "xxx",
                DateTime.UtcNow);

            // Assert
            Assert.NotEmpty(shortenedUrl.DomainEvents.OfType<ShortenedUrlCreatedDomainEvent>());
        }
    }
}