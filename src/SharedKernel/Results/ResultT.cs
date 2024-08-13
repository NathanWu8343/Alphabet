using SharedKernel.Errors;

namespace SharedKernel.Results
{
    /// <summary>
    /// Represents the result of some operation, with status information and possibly a value and an error.
    /// </summary>
    /// <typeparam name="TValue">The result value type.</typeparam>
    public class Result<TValue> : Result
    {
        private readonly TValue _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Result{TValue}"/> class with the specified parameters.
        /// </summary>
        /// <param name="value">The result value.</param>
        /// <param name="isSuccess">The flag indicating if the result is successful.</param>
        /// <param name="error">The error.</param>
        protected internal Result(TValue value, bool isSuccess, Error error)
            : base(isSuccess, error)
        {
            if (isSuccess && Equals(value, default))
            {
                throw new InvalidOperationException("Success result cannot have a default value.");
            }

            if (!isSuccess && !Equals(value, default))
            {
                throw new InvalidOperationException("Failure result cannot have a non-default value.");
            }

            _value = value;
        }

        /// <summary>
        /// Implicit conversion from a value to a success result.
        /// </summary>
        public static implicit operator Result<TValue>(TValue value) => Success(value);

        /// <summary>
        /// Gets the result value if the result is successful; otherwise, throws an exception.
        /// </summary>
        /// <returns>The result value if the result is successful.</returns>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="Result.IsFailure"/> is true.</exception>
        public TValue Value => IsSuccess
            ? _value
            : throw new InvalidOperationException("Cannot access the value of a failure result.");

        /// <summary>
        /// Returns a validation failure <see cref="Result{TValue}"/> with the specified error.
        /// </summary>
        public static Result<TValue> ValidationFailure(Error error) =>
            new(default!, false, error);
    }
}