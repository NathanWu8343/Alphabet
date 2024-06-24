using FluentValidation;
using FluentValidation.Results;
using MassTransit;
using SharedKernel.Errors;
using SharedKernel.Results;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace UrlShortener.Application.Behaviors
{
    internal sealed class ValidationBehaviorFilter<TRequest>
        : IFilter<ConsumeContext<TRequest>>

            where TRequest : class
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehaviorFilter(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope(nameof(ValidationBehaviorFilter<TRequest>));
        }

        public async Task Send(ConsumeContext<TRequest> context, IPipe<ConsumeContext<TRequest>> next)
        {
            ValidationFailure[] validationFailures = await ValidateAsync(context.Message);

            if (validationFailures.Length > 0)
            {
                //throw new ValidationException(validationFailures);
                return;
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
    }
}