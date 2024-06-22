using Microsoft.Extensions.DependencyInjection;
using UrlShortener.Application.Abstractions;
using UrlShortener.IntegrationTests.Fixtures;
using UrlShortener.Persistence;

namespace UrlShortener.IntegrationTests.Extensions
{
    internal static class ApplicationDbContextExtensions
    {
        public static ApplicationDbContextFixture GetShortendUrlsDbContext(this IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var uow = serviceProvider.GetRequiredService<IUnitOfWork>();
            return new ApplicationDbContextFixture(dbContext, uow);
        }
    }
}