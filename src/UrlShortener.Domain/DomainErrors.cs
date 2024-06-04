using SharedKernel.Errors;

namespace UrlShortener.Domain
{
    public static class DomainErrors
    {
        public static class ShortenedUrl
        {
            public static Error NotFound =>
                new("ShortenedUrl.NotFound", "The ShortenedUrl with the code was not found", ErrorType.NotFound);
        }
    }
}