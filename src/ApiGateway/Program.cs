using ApiGateway.Configs;
using ApiGateway.Extensions;
using ApiGateway.Swagger;
using ApiGateway.Swagger.Extensions;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;
using UrlShortener.Infrastructure;

namespace ApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.AddServiceDefaults();

            builder.ConfigureSerilog();
            // Add services to the container.
            //builder.Services.AddControllers();

            // Add YARP Direct Forwarding with Service Discovery support
            //builder.Services.AddHttpForwarderWithServiceDiscovery();

            //TODO: 需要增加middleware來處來對應router才要驗證, 其餘不用

            // 配置 JWT 认证和授权
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = "http://localhost:5136";
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidAudiences = new[] { "urlshortener-api" } //TODO: 需要優化成apiresource取得
                    };
                });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            builder.Services.AddSwaggerGen();

            builder.Services.AddReverseProxy()
                .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
                .AddSwagger(builder.Configuration.GetSection("ReverseProxy"))
                .AddServiceDiscoveryDestinationResolver()     // Add YARP Direct Forwarding with Service Discovery support
                ;

            var app = builder.Build();

            app.MapDefaultEndpoints();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    var config = app.Services.GetRequiredService<IOptionsMonitor<ReverseProxyDocumentFilterConfig>>().CurrentValue;
                    options.ConfigureSwaggerEndpoints(config);
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            //app.MapControllers();

            app.MapReverseProxy();

            app.Run();
        }
    }
}