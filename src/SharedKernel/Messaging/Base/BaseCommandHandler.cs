using MassTransit;
using Microsoft.Extensions.Logging;

namespace SharedKernel.Messaging.Base
{
    public abstract class BaseCommandHandler<TRequest, TResponse> :
        ICommandHandler<TRequest, TResponse>,
        IConsumer<TRequest>
     where TRequest : class, ICommand<TResponse>
     where TResponse : class
    {
        protected readonly ILogger Logger;

        protected BaseCommandHandler(ILogger logger)
        {
            Logger = logger;
        }

        public async Task Consume(ConsumeContext<TRequest> context)
        {
            //LOG
            var requestName = typeof(TRequest).Name;

            try
            {
                Logger.LogInformation("Request received: {RequestName} - {Request}."
                        , requestName
                        , context.Message);

                var response = await Handle(context.Message, context.CancellationToken);
                context.Respond(response);

                if (response != null)
                    Logger.LogInformation("Response for {RequestName}: {Response}."
                        , requestName
                        , context.Message);
                else
                {
                    Logger.LogInformation("No response payload found for {RequestName}."
                                            , requestName);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Request: {RequestName} failed with error: {Error}."
                        , requestName
                        , ex.Message);

                throw;
            }
        }

        public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }
}