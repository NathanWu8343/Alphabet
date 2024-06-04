using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Results;

namespace UrlShortener.Application.Behaviors
{
    public class LoggingPipelineBehavior<TRequest, TResponse>(ILogger<TRequest> logger)
   : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger _logger = logger;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;

            _logger.LogInformation("Request: {requestName}  {@Request}",
                requestName, request);

            var response = await next();

            //TODO: 暫時只記錄成功
            if (response is Result result && result.IsSuccess)
                _logger.LogInformation("Response: {requestName} {@Response}", requestName, result);

            return response;
        }
    }
}