using MassTransit;
using SharedKernel.Messaging;

namespace SharedKernel.Messaging.Base
{
    public abstract class BaseQueryHandler<TRequest, TResponse> :
        IQueryHandler<TRequest, TResponse>,
        IConsumer<TRequest>
     where TRequest : class, IQuery<TResponse>
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