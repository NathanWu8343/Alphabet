using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel.Maybe;
using SharedKernel.Messaging;
using SharedKernel.Messaging.Base;
using UrlShortener.Application.Abstractions.Data;
using UrlShortener.Domain.Repositories;

namespace UrlShortener.Application.Features.UrlShorteners.Queries
{
    public sealed record GtUrlVistorCountByCodeQuery(string Code) : IQuery<Maybe<double>>
    {
    }

    internal sealed class GetUrlVistorCountByCodeQueryHandler :
        BaseQueryHandler<GtUrlVistorCountByCodeQuery, Maybe<double>>
    {
        private readonly IWriteDbContext _dbContext;
        private readonly IVistorCounterRespository _vistorCounterRespository;

        public GetUrlVistorCountByCodeQueryHandler(ILogger<GetUrlVistorCountByCodeQueryHandler> logger, IVistorCounterRespository vistorCounterRespository, IWriteDbContext dbContext)
            : base(logger)
        {
            _vistorCounterRespository = vistorCounterRespository;
            _dbContext = dbContext;
        }

        public override async Task<Maybe<double>> Handle(GtUrlVistorCountByCodeQuery request, CancellationToken cancellationToken)
        {
            var bol = await _dbContext.ShortendUrls.AnyAsync(x => x.Code == request.Code, cancellationToken);
            if (!bol) return Maybe<double>.None;

            return await _vistorCounterRespository.TotalAsync(request.Code);
        }
    }
}