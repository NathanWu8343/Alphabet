﻿using Elastic.Apm.SerilogEnricher;
using Elastic.Apm.StackExchange.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SharedKernel.Common;
using UrlShortener.Application.Abstractions;
using UrlShortener.Infrastructure.Common;
using UrlShortener.Infrastructure.Redis;

namespace UrlShortener.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            //redis
            services.AddSingleton<IRedisConnectionFactory, RedisConnectionFactory>(svc =>
            {
                string? redisString = configuration.GetConnectionString("Redis");
                Ensure.NotNullOrEmpty(redisString);
                return new RedisConnectionFactory(redisString);
            });
            services.AddSingleton<ICacheProvider, RedisCacheProvider>();

            services.AddTransient<IShortCodeGenerator, HashBasedShortCodeGenerator>();

            //services.Decorate(typeof(INotificationHandler<>), typeof(IdempotentDomainEventHandler<>));

            services.AddAllElasticApm();
            services.AddSerilogWithElk(configuration);
        }

        public static void UseInfrastructure(this IApplicationBuilder app)
        {
            //app.UseAllElasticApm(configuration);

            app.ApplicationServices.GetRequiredService<IRedisConnectionFactory>()
                                   .GetConnection()
                                   .UseElasticApm();
        }

        private static void AddSerilogWithElk(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSerilog((svc, lc) =>
            {
                lc.ReadFrom.Configuration(configuration)
                  .Enrich.WithElasticApmCorrelationInfo();
                //.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
                // {
                //     IndexFormat = "elk-log-{0:yyyyMMdd}",
                //     CustomFormatter = new EcsTextFormatter()
                // });
            });
        }

        private static void AddOpenTelemetry(this IServiceCollection services)
        {
            // Build a resource configuration action to set service information.
            //    Action<ResourceBuilder> configureResource = r => r
            //    .AddService(
            //        serviceName: "test",
            //        serviceVersion: "1.0.0",
            //        serviceInstanceId: Environment.MachineName)
            //    .AddTelemetrySdk()
            //    .AddEnvironmentVariableDetector()
            //    .AddAttributes(new Dictionary<string, object>
            //    {
            //        ["environment.name"] = "dev",
            //        ["team.name"] = "backend"
            //    });

            //    //
            //    services.AddOpenTelemetry()
            //         .ConfigureResource(configureResource)
            //        .WithMetrics(metrics =>
            //        {
            //            metrics
            //                .AddMeter("test-metric")
            //                .AddAspNetCoreInstrumentation()
            //                .AddHttpClientInstrumentation()

            //                ;

            //            metrics.AddOtlpExporter(options =>
            //            {
            //                options.Endpoint = new Uri("http://localhost:8200");
            //                options.Protocol = OtlpExportProtocol.Grpc;
            //            });
            //        })
            //        .WithTracing(tracing =>
            //        {
            //            tracing
            //                .AddSource("test-tracing")
            //                .SetSampler(new AlwaysOnSampler());

            //            tracing
            //                .AddAspNetCoreInstrumentation()
            //                .AddHttpClientInstrumentation()
            //                .AddEntityFrameworkCoreInstrumentation()
            //                .AddRedisInstrumentation()
            //                ;

            //            tracing.AddOtlpExporter(options =>
            //            {
            //                options.Endpoint = new Uri("http://localhost:8200");
            //                options.Protocol = OtlpExportProtocol.Grpc;
            //            });
            //        });

            //    services.AddLogging(bulder => bulder.AddOpenTelemetry(logging =>
            //    {
            //        var resourceBuilder = ResourceBuilder.CreateDefault();
            //        configureResource(resourceBuilder);
            //        logging.SetResourceBuilder(resourceBuilder);

            //        logging.AddOtlpExporter(options =>
            //        {
            //            options.Endpoint = new Uri("http://localhost:8200");
            //            options.Protocol = OtlpExportProtocol.Grpc;
            //        });
            //    }));
        }
    }
}