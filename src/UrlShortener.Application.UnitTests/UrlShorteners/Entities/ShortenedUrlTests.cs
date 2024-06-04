using Bogus;
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
            var shortenedUrl = new Faker<ShortenedUrl>()
                .CustomInstantiator(faker => ShortenedUrl.Create(
                     faker.Internet.Url(),
                     "http://localhost/xxx/api/xxx",
                     "xxx",
                     DateTime.UtcNow
                    ));

            var act = shortenedUrl.Generate();

            // Assert
            Assert.NotEmpty(act.DomainEvents.OfType<ShortenedUrlCreatedDomainEvent>());
        }
    }
}