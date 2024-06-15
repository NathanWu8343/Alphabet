﻿using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UrlShortener.Persistence;

namespace UrlShortener.IntegrationTests.Abstractions
{
    public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
    {
        private readonly IServiceProvider _services;
        protected readonly HttpClient HttpClientStub;

        protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
        {
            // basic Arrange
            var scope = factory.Services.CreateScope();

            // TODO
            _services = scope.ServiceProvider;
            HttpClientStub = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        }

        public virtual Task InitializeAsync()
        {
            // database init
            var dbContext = _services.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.Migrate();

            return Task.CompletedTask;
        }

        public virtual Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
}