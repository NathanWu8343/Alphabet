using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Alphabet.ServiceDefaults.Extensions
{
    public static class OpenApiExtensions
    {
        public static void AddDefaultOpenApi(this IServiceCollection services)
        {
            services.ConfigureOptions<ConfigureSwaggerGenOptions>();

            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                options.OperationFilter<SecurityRequirementsOperationFilter>(true, "Bearer");

                // integrate xml comments
                options.IncludeXmlComments(XmlCommentsFilePath, true);
            });
        }

        public static void UseDefaultOpenApi(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();

                var descriptions = provider.ApiVersionDescriptions;
                foreach (var desc in descriptions)
                {
                    var url = $"/swagger/{desc.GroupName}/swagger.json";
                    var name = desc.GroupName.ToUpperInvariant();
                    options.SwaggerEndpoint(url, $"{name} Docs.");
                }
            });
        }

        private static string XmlCommentsFilePath
        {
            get
            {
                var basePath = AppContext.BaseDirectory;
                var fileName = $"{Assembly.GetEntryAssembly()!.GetName().Name}.xml";
                return Path.Combine(basePath, fileName);
            }
        }

        private sealed class ConfigureSwaggerGenOptions : IConfigureNamedOptions<SwaggerGenOptions>
        {
            private readonly IApiVersionDescriptionProvider _provider;

            public ConfigureSwaggerGenOptions(IApiVersionDescriptionProvider provider)
            {
                _provider = provider;
            }

            public void Configure(string? name, SwaggerGenOptions options)
            {
                Configure(options);
            }

            public void Configure(SwaggerGenOptions options)
            {
                foreach (var description in _provider.ApiVersionDescriptions)
                {
                    try
                    {
                        options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
                    }
                    catch (Exception)
                    {
                        //TODO
                    }
                }
            }

            private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
            {
                var name = Assembly.GetEntryAssembly()!.GetName().Name!.Replace(".", " ");
                var info = new OpenApiInfo()
                {
                    Title = $"{name}.",
                    Version = description.ApiVersion.ToString()
                };
                if (description.IsDeprecated)
                {
                    info.Description += " This API version has been deprecated.";
                }
                return info;
            }
        }
    }
}