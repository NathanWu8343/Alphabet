using SharedKernel.Common;
using System.Net.Http.Json;
using UrlShortener.Api.Abstractions;

namespace UrlShortener.IntegrationTests.Extensions
{
    internal static class HttpResponseMessageExtensions
    {
        internal static async Task<ApiResponse> GetApiResponse(this HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<ApiResponse>();
            Ensure.NotNull(content, "The content is null.");

            return content!;
        }

        internal static async Task<ApiResponse<T>> GetApiResponse<T>(this HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
            Ensure.NotNull(content, "The content is null.");

            return content!;
        }
    }
}