using OpenTelemetry.Trace;
using System.Diagnostics;

namespace UrlShortener.Api.Middlewares
{
    public class RequestIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestIdMiddleware> _logger;

        public RequestIdMiddleware(RequestDelegate next, ILogger<RequestIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.Headers["X-Request-ID"] = Tracer.CurrentSpan.Context.TraceId.ToString();
            await _next(context);
        }
    }
}