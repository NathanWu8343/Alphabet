using Microsoft.EntityFrameworkCore;
using SharedKernel.Maybe;
using SharedKernel.Messaging;
using SharedKernel.Messaging.Base;
using UrlShortener.Application.Abstractions;
using UrlShortener.Domain.Repositories;

namespace UrlShortener.Application.Features.UrlShorteners.Queries
{
    public sealed record GtUrlVistorCountQueryByCode(string Code) : IQuery<Maybe<double>>
    {
    }

    internal sealed class GetUrlVistorCountByCodeQueryHandler :
        BaseQueryHandler<GtUrlVistorCountQueryByCode, Maybe<double>>
    {
        private readonly IDbContext _dbContext;
        private readonly IVistorCounterRespository _vistorCounterRespository;

        public GetUrlVistorCountByCodeQueryHandler(IVistorCounterRespository vistorCounterRespository, IDbContext dbContext)
        {
            _vistorCounterRespository = vistorCounterRespository;
            _dbContext = dbContext;
        }

        public override async Task<Maybe<double>> Handle(GtUrlVistorCountQueryByCode request, CancellationToken cancellationToken)
        {
            var bol = await _dbContext.ShortendUrls.AnyAsync(x => x.Code == request.Code, cancellationToken);
            if (!bol) return Maybe<double>.None;

            return await _vistorCounterRespository.TotalAsync(request.Code);
        }
    }
}