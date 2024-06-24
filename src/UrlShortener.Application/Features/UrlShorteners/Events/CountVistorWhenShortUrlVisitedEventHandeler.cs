using SharedKernel.Core;
using UrlShortener.Domain.Repositories;

namespace UrlShortener.Application.Features.UrlShorteners.Events
{
    internal sealed class CountVistorWhenShortUrlVisitedEventHandeler : IEventHandler<ShortUrlVisitedEvent>
    {
        private readonly IVistorCounterRespository _vistorCounterRespository;

        public CountVistorWhenShortUrlVisitedEventHandeler(IVistorCounterRespository vistorCounterRespository)
        {
            _vistorCounterRespository = vistorCounterRespository;
        }

        public async Task Handle(ShortUrlVisitedEvent notification, CancellationToken cancellationToken)
        {
            await _vistorCounterRespository.AddAsync(notification.Code, 1);
        }
    }
}