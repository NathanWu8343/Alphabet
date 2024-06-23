using MassTransit;
using MassTransit.Mediator;
using SharedKernel.Messaging;

namespace UrlShortener.Infrastructure.Messages
{
    internal class MassTransitDispatcher : IDispatcher
    {
        private readonly IMediator _mediator;

        public MassTransitDispatcher(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : IEvent
        {
            return _mediator.Publish(notification, cancellationToken);
        }

        public Task<TResponse> Send<TResponse>(ICommand<TResponse> request, CancellationToken cancellationToken = default)
            where TResponse : class
        {
            return _mediator.SendRequest(request, cancellationToken);
        }

        public Task<TResponse> Send<TResponse>(IQuery<TResponse> request, CancellationToken cancellationToken = default)
              where TResponse : class
        {
            return _mediator.SendRequest(request, cancellationToken);
        }
    }
}