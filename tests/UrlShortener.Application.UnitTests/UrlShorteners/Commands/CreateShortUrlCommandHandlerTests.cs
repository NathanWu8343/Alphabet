using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using UrlShortener.Application.Abstractions.Data;
using UrlShortener.Application.Abstractions.Services;
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

        private readonly IShortenedUrlRepository _mockShortenedUrlRepository = Substitute.For<IShortenedUrlRepository>();
        private readonly IUnitOfWork _stubUnitOfWork = Substitute.For<IUnitOfWork>();
        private readonly IShortCodeGenerator _stubShortCodeGenerator = Substitute.For<IShortCodeGenerator>();
        private readonly ILogger<CreateShortUrlCommandHandler> _logger = NullLogger<CreateShortUrlCommandHandler>.Instance;
        private readonly TimeProvider _stubTimeProvider = new FakeTimeProvider();

        private readonly CreateShortUrlCommandHandler _handler;

        public CreateShortUrlCommandHandlerTests()
        {
            _handler = new CreateShortUrlCommandHandler(
                _logger,
                _mockShortenedUrlRepository,
                _stubUnitOfWork,
                _stubShortCodeGenerator,
                _stubTimeProvider);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async void Handle_Should_RasieArgumentException_WhenLongUrlIsEmptyOrNull(string? url)
        {
            // Arrange
            var command = Command with { Url = url };

            // act
            await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, default));
        }

        [Fact]
        public async void Handle_Should_RasieArgumentException_WhenCodeIsEmpty()
        {
            // Arrange
            var command = Command;

            _stubShortCodeGenerator.Generate(Arg.Any<string>()).Returns(string.Empty);

            // act
            await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, default));
        }

        [Fact]
        public async void Handle_Should_ReturnSuceessResult_WhenUrlAndPathAreRight()
        {
            // Arrange
            var command = Command;
            _stubShortCodeGenerator.Generate(Arg.Any<string>()).Returns("xxx");

            // act
            var result = await _handler.Handle(command, default);

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

            // act
            var result = await _handler.Handle(command, default);

            // assert
            _mockShortenedUrlRepository
                .Received(1)
                .Add(Arg.Is<ShortenedUrl>(m => m.ShortUrl == result.Value));
        }
    }
}