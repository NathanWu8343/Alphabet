using FluentValidation;
using FluentValidation.Results;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.Extensions.Logging;
using SharedKernel.Errors;
using SharedKernel.Results;
using System.Reflection;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace UrlShortener.Application.Behaviors
{
    internal sealed class ValidationPipelineFilter<TRequest>
        : IFilter<ConsumeContext<TRequest>>
        where TRequest : class
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly ILogger<ValidationPipelineFilter<TRequest>> _logger;

        public ValidationPipelineFilter(ILogger<ValidationPipelineFilter<TRequest>> logger, IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
            _logger = logger;
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope(nameof(ValidationPipelineFilter<TRequest>));
        }

        public async Task Send(ConsumeContext<TRequest> context, IPipe<ConsumeContext<TRequest>> next)
        {
            ValidationFailure[] validationFailures = await ValidateAsync(context.Message);

            if (validationFailures.Length > 0)
            {
                var requestName = typeof(TRequest).Name;
                _logger.LogInformation("Request: {RequestName} failed with error: {Error}."
                    , requestName
                    , validationFailures);

                await context.NotifyConsumed(context.ReceiveContext.ElapsedTime, "ValidationFailure");

                var genericResponseArgument = GetRequestGenericArgumentType(context.Message);

                if (genericResponseArgument != null)
                {
                    var validationError = CreateValidationError(validationFailures);

                    if (genericResponseArgument.IsGenericType &&
                       genericResponseArgument.GetGenericTypeDefinition() == typeof(Result<>))
                    {
                        Type resultType = genericResponseArgument.GetGenericArguments()[0];

                        MethodInfo? failureMethod = typeof(Result<>)
                            .MakeGenericType(resultType)
                            .GetMethod(nameof(Result<object>.ValidationFailure));

                        if (failureMethod is not null)
                        {
                            var result = failureMethod.Invoke(null, new object[] { validationError });
                            await context.RespondAsync(result);
                            return;
                        }
                    }
                    else if (genericResponseArgument == typeof(Result))
                    {
                        var result = Result.Failure(validationError);
                        await context.RespondAsync(result);
                        return;
                    }
                }

                throw new ValidationException(validationFailures);
            }

            await next.Send(context);
        }

        private async Task<ValidationFailure[]> ValidateAsync(TRequest request)
        {
            if (!_validators.Any())
            {
                return [];
            }

            var context = new ValidationContext<TRequest>(request);

            ValidationResult[] validationResults = await Task.WhenAll(
                _validators.Select(validator => validator.ValidateAsync(context)));

            ValidationFailure[] validationFailures = validationResults
                .Where(validationResult => !validationResult.IsValid)
                .SelectMany(validationResult => validationResult.Errors)
                .ToArray();

            return validationFailures;
        }

        private static ValidationError CreateValidationError(ValidationFailure[] validationFailures) =>
            new(validationFailures.Select(f => Error.Problem(f.ErrorCode, f.ErrorMessage)).ToArray());

        private static Type GetRequestGenericArgumentType(object request)
        {
            var requestType = request.GetType();
            var responseType = requestType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(Request<>));

            return responseType?.GetGenericArguments().FirstOrDefault();
        }
    }
}