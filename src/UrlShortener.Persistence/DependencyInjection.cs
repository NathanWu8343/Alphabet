using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using SharedKernel.Common;
using UrlShortener.Application.Abstractions.Data;
using UrlShortener.Domain.Repositories;
using UrlShortener.Persistence.Extensions;
using UrlShortener.Persistence.Interceptors;
using UrlShortener.Persistence.Repositories;
using UrlShortener.Persistence.Repositories.Redis;

namespace UrlShortener.Persistence
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Registers the necessary services with the DI framework.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The same service collection.</returns>
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<DbConnectionFactory>(_ =>
            {
                string? connectionString = configuration.GetConnectionString("Database");
                Ensure.NotNullOrEmpty(connectionString);
                return new DbConnectionFactory(new MySqlConnection(connectionString));
            });

            services.AddSingleton<ConvertDomainEventsToOutboxMessagesInterceptor>();
            services.AddSingleton<UpdateAuditableEntitiesInterceptor>();

            var serverVersion = new MariaDbServerVersion(new Version(10, 5, 5));

            // Write DB
            services.AddDbContext<ApplicationWriteDbContext>(
            (sp, options) => options
                 //.UseModel(ApplicationWriteDbContextModel.Instance)
                 .UseMySql(sp.GetRequiredService<DbConnectionFactory>(), serverVersion)
                 .LogTo(Console.WriteLine, LogLevel.Information)
                 .AddInterceptors(
                        sp.GetRequiredService<ConvertDomainEventsToOutboxMessagesInterceptor>(),
                        sp.GetRequiredService<UpdateAuditableEntitiesInterceptor>())
                 );
            //.UseSnakeCaseNamingConvention();
            //.AddInterceptors(sp.GetRequiredService<InsertOutboxMessagesInterceptor>()));

            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationWriteDbContext>());
            services.AddScoped<IWriteDbContext>(sp => sp.GetRequiredService<ApplicationWriteDbContext>());

            // Read DB
            services.AddDbContext<ApplicationReadDbContext>(
              (sp, options) => options
                    .UseMySql(sp.GetRequiredService<DbConnectionFactory>(), serverVersion)
                    //.UseSnakeCaseNamingConvention()
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
            services.AddScoped<IReadDbContext>(sp => sp.GetRequiredService<ApplicationReadDbContext>());

            //repositoryTransient
            services.AddScoped<IShortenedUrlRepository, ShortenedUrlRepository>();

            //
            services.AddScoped<IVistorCounterRespository, VistorCounterRespository>();

            return services;
        }
    }
}