using Microsoft.EntityFrameworkCore;
using SharedKernel.Maybe;
using SharedKernel.Messaging;
using SharedKernel.Pagination;
using UrlShortener.Application.Abstractions;

namespace UrlShortener.Application.Features.UrlShorteners.Queries
{
    public sealed record GetShortenUrlListQuery(int Page, int PageSize) : IQuery<Maybe<PagedList<ShortenUrlResponse>>>
    {
    }

    public sealed record ShortenUrlResponse(string Code, string ShorUrl) { }

    internal sealed class GetShortenUrlListQueryHandeler : IQueryHandler<GetShortenUrlListQuery, Maybe<PagedList<ShortenUrlResponse>>>
    {
        private readonly IDbContext _dbContext;

        public GetShortenUrlListQueryHandeler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Maybe<PagedList<ShortenUrlResponse>>> Handle(GetShortenUrlListQuery request, CancellationToken cancellationToken)
        {
            var data = await _dbContext.ShortendUrls
                  .OrderBy(x => x.CreatedAtUtc)
                  .Skip((request.Page - 1) * request.PageSize)
                  .Take(request.PageSize)
                  .Select(x => new ShortenUrlResponse(x.Code, x.ShortUrl))
                  .ToListAsync(cancellationToken);

            var totalCount = await _dbContext.ShortendUrls.CountAsync(cancellationToken);

            return Maybe<PagedList<ShortenUrlResponse>>
                .From(new PagedList<ShortenUrlResponse>(data, request.Page, request.PageSize, totalCount));
        }
    }
}