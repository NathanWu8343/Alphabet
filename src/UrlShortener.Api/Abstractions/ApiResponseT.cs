namespace UrlShortener.Api.Abstractions
{
    public class ApiResponse<T> : ApiResponse
    {
        public T? Data { get; init; }
    }
}