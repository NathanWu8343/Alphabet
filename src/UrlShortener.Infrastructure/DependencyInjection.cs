using Elastic.Apm.SerilogEnricher;
using Elastic.Apm.StackExchange.Redis;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Sinks.OpenTelemetry;
using SharedKernel.Common;
using SharedKernel.Messaging;
using StackExchange.Redis;
using UrlShortener.Application.Abstractions;
using UrlShortener.Infrastructure.Common;
using UrlShortener.Infrastructure.Messages;
using UrlShortener.Infrastructure.Redis;

namespace UrlShortener.Infrastructure
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Configure Serilog to write to the console and OpenTelemetry for Aspire structured logs.
        /// </summary>
        /// <remarks>
        /// ⚠ This method MUST be called before the <see cref="OpenTelemetryLoggingExtensions.AddOpenTelemetry(ILoggingBuilder)"/> method to still send structured logs via OpenTelemetry. ⚠
        /// </remarks>
        public static IHostApplicationBuilder ConfigureSerilog(this IHostApplicationBuilder builder)
        {
            var otlpExporter = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
            var serviceName = builder.Configuration["OTEL_SERVICE_NAME"] ?? "Unknown";
            Log.Logger.Information("App Service name {Name}", serviceName);
            builder.Services.AddSerilog((_, loggerConfiguration) =>
            {
                // Configure Serilog as desired here for every project (or use IConfiguration for configuration variations between projects)
                loggerConfiguration
                    .ReadFrom.Configuration(builder.Configuration)
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithProcessId()
                    .Enrich.WithProcessName()
                    .Enrich.WithThreadId()
                    .Enrich.WithSpan()
                    .WriteTo.Console();

                if (!string.IsNullOrEmpty(otlpExporter))
                {
                    loggerConfiguration
                        .WriteTo.OpenTelemetry(options =>
                        {
                            options.IncludedData = IncludedData.TraceIdField | IncludedData.SpanIdField;
                            options.Endpoint = otlpExporter;
                            options.ResourceAttributes.Add("service.name", serviceName);

                            AddHeaders(options.Headers, builder.Configuration["OTEL_EXPORTER_OTLP_HEADERS"]);
                            AddResourceAttributes(options.ResourceAttributes, builder.Configuration["OTEL_RESOURCE_ATTRIBUTES"]);

                            void AddHeaders(IDictionary<string, string> headers, string headerConfig)
                            {
                                if (!string.IsNullOrEmpty(headerConfig))
                                {
                                    foreach (var header in headerConfig.Split(','))
                                    {
                                        var parts = header.Split('=');

                                        if (parts.Length == 2)
                                        {
                                            headers[parts[0]] = parts[1];
                                        }
                                        else
                                        {
                                            throw new InvalidOperationException($"Invalid header format: {header}");
                                        }
                                    }
                                }
                            }

                            void AddResourceAttributes(IDictionary<string, object> attributes, string attributeConfig)
                            {
                                if (!string.IsNullOrEmpty(attributeConfig))
                                {
                                    var parts = attributeConfig.Split('=');

                                    if (parts.Length == 2)
                                    {
                                        attributes[parts[0]] = parts[1];
                                    }
                                    else
                                    {
                                        throw new InvalidOperationException($"Invalid resource attribute format: {attributeConfig}");
                                    }
                                }
                            }
                        });
                }
            });

            // Removes the built-in logging providers
            builder.Logging.ClearProviders().AddSerilog();
            return builder;
        }

        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            //redis
            services.AddSingleton<IRedisConnectionFactory, RedisConnectionFactory>(svc =>
            {
                string? redisString = configuration.GetConnectionString("Redis");
                Ensure.NotNullOrEmpty(redisString);
                return new RedisConnectionFactory(redisString);
            });

            services.AddSingleton<IConnectionMultiplexer>(sp => sp.GetRequiredService<IRedisConnectionFactory>().GetConnection());

            services.AddSingleton<ICacheProvider, RedisCacheProvider>();

            services.AddTransient<IShortCodeGenerator, HashBasedShortCodeGenerator>();

            //services.Decorate(typeof(INotificationHandler<>), typeof(IdempotentDomainEventHandler<>));

            //services.AddAllElasticApm();
            // services.AddSerilogWithElk(configuration);

            //messages
            //services.AddMediator(cfg =>
            //{
            //    //cfg.AddConsumer<CreateShortUrlCommandHandler>();
            //    //cfg.AddConsumers(UrlShortener.Application.AssemblyReference.Assembly);
            //});
            services.AddMassTransit(cfg =>
            {
                cfg.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username("root");
                        h.Password("123123");
                    });

                    cfg.ConfigureEndpoints(context);
                });

                //cfg.AddConsumers(Assembly.GetExecutingAssembly());
            });
            services.AddSingleton<IDispatcher, MassTransitDispatcher>();
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