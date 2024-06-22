using Bogus;
using FluentAssertions;
using SharedKernel.Pagination;
using System.Net;
using UrlShortener.Application.Features.UrlShorteners.Queries;
using UrlShortener.Domain.Entities;
using UrlShortener.IntegrationTests.Abstractions;
using UrlShortener.IntegrationTests.Extensions;
using UrlShortener.IntegrationTests.Fixtures;

namespace UrlShortener.IntegrationTests.UrlShortener
{
    public class GetShortenTests : BaseIntegrationTest
    {
        private readonly ApplicationDbContextFixture _fixture;

        public GetShortenTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
            _fixture = _serviceProvider.GetShortendUrlsDbContext();
        }

        [Theory]
        [InlineData(5)]
        public async Task Get_Should_ReturnList_WhenUrlCreate(int expected)
        {
            // Arrange
            var page = 1;
            var pageSize = 5;

            var data = GenerateFakerShortenedUrlData(expected);

            await _fixture
                .AddShortenedUrl(data)
                .SaveInDatabaseAsync();

            // Act
            HttpResponseMessage response = await HttpClientStub.GetAsync(requestUri: $"api/v1/shorten?page={page}&pageSize={pageSize}");

            // Assert(HTTP)
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Assert(HTTP Content Response)
            var result = response.GetApiResponse<PagedList<ShortenUrlResponse>>().Result;
            result.Data.Should().NotBeNull();
            result.Data.Items.Should().HaveCount(expected);
        }

        private IEnumerable<ShortenedUrl> GenerateFakerShortenedUrlData(int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return new Faker<ShortenedUrl>()
                  .CustomInstantiator(faker => ShortenedUrl.Create(
                       faker.Internet.Url(),
                       "http://localhost/xxx/api/xxx",
                       faker.Random.String2(16, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"),
                       DateTime.UtcNow
                      ));
            }
        }
    }
}