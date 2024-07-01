using MassTransit;
using Microsoft.Extensions.Logging;

namespace SharedKernel.Messaging.Base
{
    public abstract class BaseEventHandler<TRequest> :
        IEventHandler<TRequest>,
        IConsumer<TRequest>
     where TRequest : class, IEvent
    {
        protected readonly ILogger Logger;

        protected BaseEventHandler(ILogger logger)
        {
            Logger = logger;
        }

        public Task Consume(ConsumeContext<TRequest> context)
        {
            //LOG
            var requestName = typeof(TRequest).Name;

            try
            {
                Logger.LogInformation("Request received: {RequestName} : {Request}"
                      , requestName
                      , context.Message);

                return Handle(context.Message, context.CancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "equest: {RequestName} failed with error: {Error}"
                         , requestName
                         , ex.Message);

                return Task.CompletedTask;
            }
        }

        public abstract Task Handle(TRequest request, CancellationToken cancellationToken);
    }
}