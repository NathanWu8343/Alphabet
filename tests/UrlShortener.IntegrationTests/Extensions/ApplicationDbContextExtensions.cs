using Microsoft.Extensions.DependencyInjection;
using UrlShortener.Application.Abstractions.Data;
using UrlShortener.IntegrationTests.Fixtures;
using UrlShortener.Persistence.DbContexts;

namespace UrlShortener.IntegrationTests.Extensions
{
    internal static class ApplicationDbContextExtensions
    {
        public static ApplicationDbContextFixture GetShortendUrlsDbContext(this IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService<ApplicationWriteDbContext>();
            var uow = serviceProvider.GetRequiredService<IUnitOfWork>();
            return new ApplicationDbContextFixture(dbContext, uow);
        }
    }
}