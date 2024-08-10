using Alphabet.ServiceDefaults;
using Alphabet.ServiceDefaults.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using OpenTelemetry.Trace;
using Serilog;
using System.Reflection.PortableExecutable;
using UrlShortener.Api.Extensions;
using UrlShortener.Api.Filters;
using UrlShortener.Api.Middlewares;
using UrlShortener.Api.Misc;
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

            // Create a service to expose ActivitySource, and Metric Instruments
            // for manual instrumentation
            builder.Services.AddSingleton<Instrumentation>();

            // Add service defaults & Aspire components.
            builder.AddServiceDefaults();

            //builder.ConfigureSerilog();

            builder.Services
                .AddApplication()
                .AddPersistence(builder.Configuration)
                .AddInfrastructure(builder.Configuration);

            builder.Services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });

            builder.Services.AddApiVersion();

            //NOTE: 需要搭配 builder.Services.AddVersion()
            builder.Services.AddDefaultOpenApi();

            builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);

            builder.Services
                .AddControllers(options =>
                {
                    options.Filters.Add<ModelStateValidationAttribute>();
                    options.Conventions.Add(new RouteTokenTransformerConvention(new ToKebabParameterTransformer())); // 將駝峰改為 Tokenbab 形式

                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    // 當您的動作可以從模型驗證錯誤復原時，停用預設行為將會有所幫助。
                    options.SuppressModelStateInvalidFilter = true;
                });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.IncludeErrorDetails = true; // 預設值為 true，有時會特別關閉

                        //IdentityServer地址
                        options.Authority = "http://localhost:5136";
                        //對應Idp中ApiResource的Name
                        options.Audience = "urlshortener-api";
                        //不使用https
                        options.RequireHttpsMetadata = false;
                    });
            ;

            var app = builder.Build();

            app.MapDefaultEndpoints();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDefaultOpenApi();
                app.ApplyMigrations();
            }

            app.UseSerilogRequestLogging();
            app.UseMiddleware<ExceptionHandlerMiddleware>();
            app.UseMiddleware<RequestIdMiddleware>();
            app.UseInfrastructure();

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}