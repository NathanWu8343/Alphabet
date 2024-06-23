using MassTransit;
using SharedKernel.Messaging;

namespace SharedKernel.Messaging.Base
{
    public abstract class BaseEventHandler<TRequest> :
        IEventHandler<TRequest>,
        IConsumer<TRequest>
     where TRequest : class, IEvent
    {
        public Task Consume(ConsumeContext<TRequest> context)
        {
            return Handle(context.Message, context.CancellationToken);
        }

        public abstract Task Handle(TRequest request, CancellationToken cancellationToken);
    }
}