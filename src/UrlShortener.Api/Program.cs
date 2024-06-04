using Elastic.Apm.SerilogEnricher;
using Elastic.CommonSchema.Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System.Text;
using UrlShortener.Api.Extensions;
using UrlShortener.Api.Filters;
using UrlShortener.Api.Infrastructure.OpenApi;
using UrlShortener.Api.Middlewares;
using UrlShortener.Application;
using UrlShortener.Infrastructure;
using UrlShortener.Persistence;

namespace UrlShortener.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services
                .AddApplication()
                .AddPersistence(builder.Configuration)
                .AddInfrastructure(builder.Configuration);

            builder.Services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });

            builder.Services.AddVersion();

            builder.Services.AddOpenApi();

            builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);

            builder.Services.
                AddControllers(options =>
                {
                    options.Filters.Add<ModelStateValidationAttribute>();
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    // ��z���ʧ@�i�H�q�ҫ����ҿ��~�_��ɡA���ιw�]�欰�N�|�������U�C
                    options.SuppressModelStateInvalidFilter = true;
                });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.IncludeErrorDetails = true; // �w�]�Ȭ� true�A���ɷ|�S�O����

                        //IdentityServer�a�}
                        options.Authority = "http://localhost:5136";
                        //����Idp��ApiResource��Name
                        options.Audience = "urlshortener-api";
                        //���ϥ�https
                        options.RequireHttpsMetadata = false;
                    });
            ;

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseOpenApi();
                app.ApplyMigrations();
            }

            app.UseMiddleware<ExceptionHandlerMiddleware>();
            app.UseInfrastructure();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSerilogRequestLogging();

            app.MapControllers();

            app.Run();
        }
    }
}