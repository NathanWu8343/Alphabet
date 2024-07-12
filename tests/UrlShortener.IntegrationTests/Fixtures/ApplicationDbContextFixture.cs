using UrlShortener.Application.Abstractions;
using UrlShortener.Domain.Entities;
using UrlShortener.Persistence;

namespace UrlShortener.IntegrationTests.Fixtures
{
    internal class ApplicationDbContextFixture
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IUnitOfWork _unitOfWork;

        public ApplicationDbContextFixture(ApplicationDbContext dbContext, IUnitOfWork unitOfWork)
        {
            _dbContext = dbContext;
            _unitOfWork = unitOfWork;
        }

        public ApplicationDbContextFixture AddShortenedUrl(IEnumerable<ShortenedUrl> data)
        {
            var context = _dbContext.ShortendUrls;
            context.AddRange(data);
            return this;
        }

        public Task SaveInDatabaseAsync()
        {
            return _unitOfWork.SaveChangesAsync(default);
        }
    }
}