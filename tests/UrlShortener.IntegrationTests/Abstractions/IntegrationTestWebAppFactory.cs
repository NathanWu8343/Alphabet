using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MySqlConnector;
using Testcontainers.MariaDb;
using Testcontainers.Redis;
using UrlShortener.Api;
using UrlShortener.Infrastructure.Redis;
using UrlShortener.Persistence;

namespace UrlShortener.IntegrationTests.Abstractions
{
    public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private const int DB_PORT = 3306;

        private readonly MariaDbContainer _dbContainer = new MariaDbBuilder()
            .WithImage("mariadb:10.5.5")
            .WithEnvironment("TZ", "Asia/Shanghai")
            .WithEnvironment("MYSQL_ROOT_PASSWORD", "mariadb")
            .WithUsername("root")
            .WithPassword("mariadb")
            .WithExposedPort(DB_PORT)
            .WithPortBinding(DB_PORT, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(DB_PORT))
            .Build();

        private readonly RedisContainer _redisContainer = new RedisBuilder()
            .WithImage("redis:7")
            .Build();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                // db
                services.RemoveAll(typeof(DbConnectionFactory));
                services.AddSingleton<DbConnectionFactory>(_ =>
                {
                    return new DbConnectionFactory(new MySqlConnection(_dbContainer.GetConnectionString()));
                });

                // reids
                services.RemoveAll(typeof(IRedisConnectionFactory));
                services.AddSingleton<IRedisConnectionFactory, RedisConnectionFactory>(svc =>
                    new RedisConnectionFactory(_redisContainer.GetConnectionString()));
            });

            builder.UseEnvironment("Test");
        }

        public Task InitializeAsync()
        {
            return Task.WhenAll(
                 _dbContainer.StartAsync(),
                 _redisContainer.StartAsync());
        }

        public new Task DisposeAsync()
        {
            return Task.WhenAll(
                 _dbContainer.DisposeAsync().AsTask(),
                 _redisContainer.DisposeAsync().AsTask()
                 );
        }
    }
}