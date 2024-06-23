using SharedKernel.Messaging.Base;
using UrlShortener.Domain.Repositories;

namespace UrlShortener.Application.Features.UrlShorteners.Events
{
    internal class CountVistorWhenShortUrlVisitedEventHandeler :
            BaseEventHandler<ShortUrlVisitedEvent>
    {
        private readonly IVistorCounterRespository _vistorCounterRespository;

        public CountVistorWhenShortUrlVisitedEventHandeler(IVistorCounterRespository vistorCounterRespository)
        {
            _vistorCounterRespository = vistorCounterRespository;
        }

        public override async Task Handle(ShortUrlVisitedEvent notification, CancellationToken cancellationToken)
        {
            await _vistorCounterRespository.AddAsync(notification.Code, 1);
        }
    }
}