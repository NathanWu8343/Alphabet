using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using UrlShortener.Application.Abstractions;
using UrlShortener.Application.Features.UrlShorteners.Commands;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Repositories;

namespace UrlShortener.Application.UnitTests.UrlShorteners.Commands
{
    public class CreateShortUrlCommandHandlerTests
    {
        private static readonly CreateShortUrlCommand Command = new(
            "http://google.com",
            "http://localhost/api");

        private readonly IShortenedUrlRepository _mockShortenedUrlRepository;
        private readonly IUnitOfWork _stubUnitOfWork;
        private readonly IShortCodeGenerator _stubShortCodeGenerator;
        private readonly TimeProvider _stubTimeProvider;
        private readonly ILogger<CreateShortUrlCommandHandler> _logger;

        public CreateShortUrlCommandHandlerTests()
        {
            _mockShortenedUrlRepository = Substitute.For<IShortenedUrlRepository>();
            _stubUnitOfWork = Substitute.For<IUnitOfWork>();
            _stubShortCodeGenerator = Substitute.For<IShortCodeGenerator>();
            _stubTimeProvider = new FakeTimeProvider();
            _logger = NullLogger<CreateShortUrlCommandHandler>.Instance;
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async void Handle_Should_RasieArgumentException_WhenLongUrlIsEmptyOrNull(string? url)
        {
            // Arrange
            var command = Command with { Url = url };

            var handle = new CreateShortUrlCommandHandler(
                _logger,
                _mockShortenedUrlRepository,
                _stubUnitOfWork,
                _stubShortCodeGenerator,
                _stubTimeProvider);

            // act
            await Assert.ThrowsAsync<ArgumentException>(() => handle.Handle(command, default));
        }

        [Fact]
        public async void Handle_Should_RasieArgumentException_WhenCodeIsEmpty()
        {
            // Arrange
            var command = Command;

            _stubShortCodeGenerator.Generate(Arg.Any<string>()).Returns(string.Empty);

            var handle = new CreateShortUrlCommandHandler(
                _logger,
                _mockShortenedUrlRepository,
                _stubUnitOfWork,
                _stubShortCodeGenerator,
                _stubTimeProvider);

            // act
            await Assert.ThrowsAsync<ArgumentException>(() => handle.Handle(command, default));
        }

        [Fact]
        public async void Handle_Should_ReturnSuceessResult_WhenUrlAndPathAreRight()
        {
            // Arrange
            var command = Command;
            _stubShortCodeGenerator.Generate(Arg.Any<string>()).Returns("xxx");

            var handle = new CreateShortUrlCommandHandler(
                _logger,
                _mockShortenedUrlRepository,
                _stubUnitOfWork,
                _stubShortCodeGenerator,
                _stubTimeProvider);

            // act
            var result = await handle.Handle(command, default);

            // assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeEmpty();
        }

        [Fact]
        public async void Handle_Should_CallAddOnRespositoryOnce_WhenUrlAndPathAreRight()
        {
            // Arrange
            var command = Command;
            _stubShortCodeGenerator.Generate(Arg.Any<string>()).Returns("xxx");

            var handle = new CreateShortUrlCommandHandler(
                _logger,
                _mockShortenedUrlRepository,
                _stubUnitOfWork,
                _stubShortCodeGenerator,
                _stubTimeProvider);

            // act
            var result = await handle.Handle(command, default);

            // assert
            _mockShortenedUrlRepository
                .Received(1)
                .Add(Arg.Is<ShortenedUrl>(m => m.ShortUrl == result.Value));
        }
    }
}