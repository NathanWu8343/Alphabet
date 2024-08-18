using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SharedKernel.Errors;
using UrlShortener.Api.Abstractions;

namespace UrlShortener.Api.ActionFilters
{
    /// <summary>
    /// ModeState Validateion filter
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ModelStateValidationAttribute : ActionFilterAttribute
    {
        private readonly ILogger<ModelStateValidationAttribute> _logger;

        public ModelStateValidationAttribute(ILogger<ModelStateValidationAttribute> logger)
        {
            _logger = logger;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var response = ApiResponse.Failure(CreateValidationError(context.ModelState));

                _logger.LogInformation("Parameter Invalid: {@Response}", response);
                context.Result = new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(response);
            }

            base.OnActionExecuting(context);
        }

        private static ValidationError CreateValidationError(ModelStateDictionary modelState)
        {
            var errors = new List<Error>();
            foreach (var kvp in modelState)
            {
                var error = modelState[kvp.Key]?.Errors?
                        .Select(e => Error.Problem(kvp.Key, e.ErrorMessage));

                if (error is not null)
                    errors.AddRange(error);
            }
            return new ValidationError([.. errors]);
        }
    }
}