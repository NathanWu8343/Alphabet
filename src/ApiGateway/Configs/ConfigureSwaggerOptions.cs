using ApiGateway.Extensions;
using ApiGateway.Swagger;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ApiGateway.Configs
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly ReverseProxyDocumentFilterConfig _reverseProxyDocumentFilterConfig;

        public ConfigureSwaggerOptions(IOptionsMonitor<ReverseProxyDocumentFilterConfig> reverseProxyDocumentFilterConfigOptions)
        {
            _reverseProxyDocumentFilterConfig = reverseProxyDocumentFilterConfigOptions.CurrentValue;
        }

        public void Configure(SwaggerGenOptions options)
        {
            var filterDescriptors = new List<FilterDescriptor>();

            options.ConfigureSwaggerDocs(_reverseProxyDocumentFilterConfig);

            filterDescriptors.Add(new FilterDescriptor
            {
                Type = typeof(ReverseProxyDocumentFilter),
                Arguments = Array.Empty<object>()
            });

            options.DocumentFilterDescriptors = filterDescriptors;
        }
    }
}