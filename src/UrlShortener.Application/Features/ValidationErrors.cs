using SharedKernel.Errors;

namespace UrlShortener.Application.Features
{
    internal static class ValidationErrors
    {
        internal static class CreateShortUrl
        {
            internal static Error UrlRequired => new("CreateShortUrl.UrlRequired", "This Url is required.", ErrorType.Validation);
            internal static Error UrlInvalid => new("CreateShortUrl.UrlInvalid", "This Url is invalid.", ErrorType.Validation);
        }
    }
}