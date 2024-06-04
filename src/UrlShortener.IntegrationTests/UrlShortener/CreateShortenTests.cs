﻿using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using UrlShortener.Api.Contracts;
using UrlShortener.IntegrationTests.Abstractions;
using UrlShortener.IntegrationTests.Extensions;

namespace UrlShortener.IntegrationTests.UrlShortener
{
    public class CreateShortenTests : BaseIntegrationTest
    {
        private static readonly CreateShortenUrlRequest Request = new()
        {
            Url = "http://google.com"
        };

        public CreateShortenTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Create_Should_ReturnO_WhenRequesIsValid()
        {
            // Arrange
            var request = Request;

            // Act
            HttpResponseMessage response = await HttpClientStub.PostAsJsonAsync(requestUri: "api/v1/shorten/create", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.GetApiResponse<string>().Should().NotBeNull();
        }
    }
}