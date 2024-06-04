namespace SharedKernel.Errors
{
    /// <summary>
    /// Represents a concrete domain error.
    /// </summary>
    public record Error
    {
        public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);
        public static readonly Error NullValue = new(
            "General.Null",
            "Null value was provided",
            ErrorType.Failure);

        public Error(string code, string message, ErrorType type)
        {
            Code = code;
            Message = message;
            Type = type;
        }

        public string Code { get; }

        public string Message { get; }

        public ErrorType Type { get; }

        public static Error Failure(string code, string message) =>
            new(code, message, ErrorType.Failure);

        public static Error NotFound(string code, string message) =>
            new(code, message, ErrorType.NotFound);

        public static Error Problem(string code, string message) =>
            new(code, message, ErrorType.Problem);

        public static Error Conflict(string code, string message) =>
            new(code, message, ErrorType.Conflict);
    }
}