using MassTransit;

namespace SharedKernel.Messaging.Base
{
    public abstract class BaseCommandHandler<TRequest, TResponse> :
        ICommandHandler<TRequest, TResponse>,
        IConsumer<TRequest>
     where TRequest : class, ICommand<TResponse>
     where TResponse : class
    {
        public async Task Consume(ConsumeContext<TRequest> context)
        {
            var response = await Handle(context.Message, context.CancellationToken);
            context.Respond(response);
        }

        public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }
}