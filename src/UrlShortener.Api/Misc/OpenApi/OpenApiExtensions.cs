using Asp.Versioning.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

namespace UrlShortener.Api.Infrastructure.OpenApi
{
    public static class OpenApiExtensions
    {
        public static void AddOpenApi(this IServiceCollection services)
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

        public static void UseOpenApi(this IApplicationBuilder app)
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
    }
}