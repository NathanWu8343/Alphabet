using SharedKernel.Errors;
using System.Text.Json.Serialization;

namespace UrlShortener.Api.Abstractions
{
    /// <summary>
    /// 延生 ProblemDetails
    /// </summary>
    public class ApiResponse
    {
        [JsonConstructor]
        public ApiResponse()
        { }

        public int Status { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Type { get; init; }

        public string Title { get; init; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Detail { get; init; }

        public long TimeStamp { get; init; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Instance { get; init; }

        [JsonExtensionData]
        public IDictionary<string, object?> Extensions { get; init; } = new Dictionary<string, object?>(StringComparer.Ordinal);

        public static ApiResponse Success()
        {
            return new ApiResponse()
            {
                Status = StatusCodes.Status200OK,
                Title = "操作成功",
            };
        }

        public static ApiResponse Success<T>(T value)
        {
            return new ApiResponse<T>()
            {
                Status = StatusCodes.Status200OK,
                Title = "操作成功",
                Data = value
            };
        }

        public static ApiResponse Failure(ValidationError error)
        {
            return new ApiResponse()
            {
                Title = GetTitle(error),
                Type = GetType(error.Type),
                Detail = GetDetail(error),
                Status = GetStatusCode(error.Type),
                Extensions = { { "errors", error.Errors } }
            };
        }

        public static ApiResponse Failure(Error error)
        {
            return new ApiResponse()
            {
                Title = GetTitle(error),
                Type = GetType(error.Type),
                Detail = GetDetail(error),
                Status = GetStatusCode(error.Type),
            };
        }

        private static string GetTitle(Error error) =>
             error.Type switch
             {
                 ErrorType.Validation => error.Code,
                 ErrorType.Problem => error.Code,
                 ErrorType.NotFound => error.Code,
                 ErrorType.Conflict => error.Code,
                 _ => "Server failure"
             };

        private static string GetDetail(Error error) =>
          error.Type switch
          {
              ErrorType.Validation => error.Message,
              ErrorType.Problem => error.Message,
              ErrorType.NotFound => error.Message,
              ErrorType.Conflict => error.Message,
              _ => "An unexpected error occurred"
          };

        private static int GetStatusCode(ErrorType errorType) =>
         errorType switch
         {
             ErrorType.Validation => StatusCodes.Status400BadRequest,
             ErrorType.NotFound => StatusCodes.Status404NotFound,
             ErrorType.Conflict => StatusCodes.Status409Conflict,
             _ => StatusCodes.Status500InternalServerError
         };

        private static string GetType(ErrorType errorType) =>
          errorType switch
          {
              ErrorType.Validation => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
              ErrorType.Problem => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
              ErrorType.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
              ErrorType.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
              _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
          };
    }
}