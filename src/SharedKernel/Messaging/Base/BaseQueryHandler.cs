using MassTransit;
using Microsoft.Extensions.Logging;

namespace SharedKernel.Messaging.Base
{
    public abstract class BaseQueryHandler<TRequest, TResponse> :
        IQueryHandler<TRequest, TResponse>,
        IConsumer<TRequest>
     where TRequest : class, IQuery<TResponse>
     where TResponse : class
    {
        protected readonly ILogger Logger;

        protected BaseQueryHandler(ILogger logger)
        {
            Logger = logger;
        }

        public async Task Consume(ConsumeContext<TRequest> context)
        {
            //LOG
            var requestName = typeof(TRequest).Name;

            try
            {
                Logger.LogInformation("Request received: {RequestName} - {Request}"
                       , requestName
                       , context.Message);

                var response = await Handle(context.Message, context.CancellationToken);
                context.Respond(response);

                if (response != null)
                    Logger.LogInformation("Response for {RequestName}: {Response}"
                        , requestName
                        , context.Message);
                else
                    Logger.LogWarning("No response payload found for {RequestName}"
                        , requestName);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Request: {RequestName} failed with error: {Error}"
                     , requestName
                     , ex.Message);
            }
        }

        public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }
}