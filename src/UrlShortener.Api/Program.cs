using Alphabet.ServiceDefaults.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Serilog;
using UrlShortener.Api.Extensions;
using UrlShortener.Api.Filters;
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

            builder.Services.AddVersion();

            //NOTE: 需要搭配 builder.Services.AddVersion()
            builder.Services.AddDefaultOpenApi();

            builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);

            builder.Services.
                AddControllers(options =>
                {
                    options.Filters.Add<ModelStateValidationAttribute>();
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
                //  app.ApplyMigrations();
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