using UrlShortener.Application.Abstractions.Caching;
using UrlShortener.Domain.Repositories;

namespace UrlShortener.Persistence.Repositories.Redis
{
    internal sealed class VistorCounterRespository : IVistorCounterRespository
    {
        private readonly ICacheProvider _cacheProvider;

        public VistorCounterRespository(ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }

        public async Task AddAsync(string code, int count)
        {
            await _cacheProvider.StringIncrementAsync(code, count);
        }

        public async Task<double> TotalAsync(string code)
        {
            return await _cacheProvider.GetAsync<double>(code);
        }
    }
}