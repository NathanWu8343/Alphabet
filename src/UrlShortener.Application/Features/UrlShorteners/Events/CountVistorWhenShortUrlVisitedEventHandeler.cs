using Microsoft.Extensions.Logging;
using SharedKernel.Messaging.Base;
using UrlShortener.Domain.Repositories;

namespace UrlShortener.Application.Features.UrlShorteners.Events
{
    internal class CountVistorWhenShortUrlVisitedEventHandeler :
            BaseEventHandler<ShortUrlVisitedEvent>
    {
        private readonly IVistorCounterRespository _vistorCounterRespository;

        public CountVistorWhenShortUrlVisitedEventHandeler(ILogger<CountVistorWhenShortUrlVisitedEventHandeler> logger, IVistorCounterRespository vistorCounterRespository)
            : base(logger)
        {
            _vistorCounterRespository = vistorCounterRespository;
        }

        public override async Task Handle(ShortUrlVisitedEvent notification, CancellationToken cancellationToken)
        {
            await _vistorCounterRespository.AddAsync(notification.Code, 1);
        }
    }
}