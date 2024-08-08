using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel.Maybe;
using SharedKernel.Messaging;
using SharedKernel.Messaging.Base;
using SharedKernel.Pagination;
using UrlShortener.Application.Abstractions.Data;

namespace UrlShortener.Application.Features.UrlShorteners.Queries
{
    public sealed record GetShortenUrlListQuery(int Page, int PageSize) : IQuery<Maybe<PagedList<ShortenUrlResponse>>>
    {
    }

    public sealed record ShortenUrlResponse(string Code, string ShorUrl) { }

    internal sealed class GetShortenUrlListQueryHandeler :
        BaseQueryHandler<GetShortenUrlListQuery, Maybe<PagedList<ShortenUrlResponse>>>
    {
        private readonly IDbContext _dbContext;

        public GetShortenUrlListQueryHandeler(ILogger<GetShortenUrlListQueryHandeler> logger, IDbContext dbContext)
            : base(logger)
        {
            _dbContext = dbContext;
        }

        public override async Task<Maybe<PagedList<ShortenUrlResponse>>> Handle(GetShortenUrlListQuery request, CancellationToken cancellationToken)
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