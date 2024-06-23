using Microsoft.EntityFrameworkCore;
using SharedKernel.Maybe;
using SharedKernel.Messaging;
using SharedKernel.Messaging.Base;
using UrlShortener.Application.Abstractions;
using UrlShortener.Application.Features.UrlShorteners.Events;

namespace UrlShortener.Application.Features.UrlShorteners.Queries
{
    public sealed record GetVisitShortenUrlByCodeQuery(string Code) : IQuery<Maybe<string>>
    {
    }

    internal sealed class GetVisitShortenUrlByCodeQueryHandler :
        BaseQueryHandler<GetVisitShortenUrlByCodeQuery, Maybe<string>>
    {
        private readonly IDbContext _dbContext;
        private readonly IDispatcher _mediator;

        public GetVisitShortenUrlByCodeQueryHandler(IDispatcher mediator, IDbContext dbContext)
        {
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public override async Task<Maybe<string>> Handle(GetVisitShortenUrlByCodeQuery request, CancellationToken cancellationToken)
        {
            var url = await _dbContext.ShortendUrls
                .Where(x => x.Code == request.Code)
                .Select(x => x.LongUrl)
                .FirstOrDefaultAsync(cancellationToken);

            if (url is null)
                return Maybe<string>.None;

            await _mediator.Publish(new ShortUrlVisitedEvent(request.Code));
            return url;
        }
    }
}