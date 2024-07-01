using Microsoft.AspNetCore.Mvc;
using SharedKernel.Errors;
using SharedKernel.Messaging;

namespace UrlShortener.Api.Abstractions
{
    [Route("api/v{v:apiVersion}/[controller]")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ApiController]
    public abstract class ApiController : ControllerBase
    {
        private IDispatcher? _dispatcher;

        protected IDispatcher Dispatcher => _dispatcher ??= HttpContext.RequestServices.GetRequiredService<IDispatcher>();

        protected CancellationToken CancellationToken => HttpContext.RequestAborted;

        protected new IActionResult Ok()
        {
            return new ObjectResult(ApiResponse.Success());
        }

        protected IActionResult Ok<TValue>(TValue? value)
        {
            return new ObjectResult(ApiResponse.Success(value));
        }

        protected IActionResult Failure(Error error) =>
           error switch
           {
               ValidationError validationError =>
                   BadRequest(ApiResponse.Failure(validationError)),
               _ =>
                   BadRequest(ApiResponse.Failure(error))
           };
    }
}