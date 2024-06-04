using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Maybe;
using SharedKernel.Messaging;
using UrlShortener.Application.Abstractions;
using UrlShortener.Application.Features.UrlShorteners.Events;

namespace UrlShortener.Application.Features.UrlShorteners.Queries
{
    public sealed record GetVisitShortenUrlByCodeQuery(string Code) : IQuery<Maybe<string>>
    {
    }

    internal sealed class GetVisitShortenUrlByCodeQueryHandler : IQueryHandler<GetVisitShortenUrlByCodeQuery, Maybe<string>>
    {
        private readonly IDbContext _dbContext;
        private readonly IMediator _mediator;

        public GetVisitShortenUrlByCodeQueryHandler(IMediator mediator, IDbContext dbContext)
        {
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<Maybe<string>> Handle(GetVisitShortenUrlByCodeQuery request, CancellationToken cancellationToken)
        {
            var url = await _dbContext.ShortendUrls
                .Where(x => x.Code == request.Code)
                .Select(x => x.Code)
                .FirstOrDefaultAsync(cancellationToken);

            if (url is null)
                return Maybe<string>.None;

            await _mediator.Publish(new ShortUrlVisitedEvent(request.Code));
            return url;
        }
    }
}