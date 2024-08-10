using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using ServiceDefaults.Swagger;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

namespace ServiceDefaults.Extensions
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
                options.DocumentFilter<SwaggerEnumDocumentFilter>();
                options.OperationFilter<SwaggerDefaultValues>();

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
    }
}