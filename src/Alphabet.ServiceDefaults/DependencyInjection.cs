using Alphabet.ServiceDefaults.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Reflection.PortableExecutable;

namespace Alphabet.ServiceDefaults;

// Adds common .NET Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
// This project should be referenced by each service project in your solution.
public static class DependencyInjection
{
    /// <summary>
    /// Adds the services except for making outgoing HTTP calls.
    /// </summary>
    /// <remarks>
    /// This allows for things like Polly to be trimmed out of the app if it isn't used.
    /// </remarks>
    public static IHostApplicationBuilder AddBasicServiceDefaults(this IHostApplicationBuilder builder)
    {
        //TODO:待判斷
        builder.AddSeqEndpoint("seqlog");

        builder.ConfigureSerilog();

        builder.ConfigureOpenTelemetry();

        builder.AddDefaultHealthChecks();

        return builder;
    }

    public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
    {
        builder.AddBasicServiceDefaults();

        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            // Turn on resilience by default
            http.AddStandardResilienceHandler();

            // Turn on service discovery by default
            http.AddServiceDiscovery();
        });

        return builder;
    }

    public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
    {
        var grafanaUrl = builder.Configuration.GetConnectionString("grafana")!;

        // Build a resource configuration action to set service information.
        Action<ResourceBuilder> configureResource = r => r
            .AddService(
                serviceName: "demo",
                serviceVersion: "1.0.0",
                serviceInstanceId: Environment.MachineName)
            .AddTelemetrySdk()
            .AddEnvironmentVariableDetector()
            .AddAttributes(new Dictionary<string, object>
            {
                ["environment.name"] = "dev",
                ["team.name"] = "backend"
            });

        //NOTE: LOG要各別設定
        //builder.Logging.AddOpenTelemetry(logging =>
        //{
        //    var resourceBuilder = ResourceBuilder.CreateDefault();
        //    configureResource(resourceBuilder);
        //    logging.SetResourceBuilder(resourceBuilder);

        //    logging.IncludeFormattedMessage = true;
        //    logging.IncludeScopes = true;
        //});

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(configureResource)
            .WithMetrics(metrics =>
            {
                metrics
                    .AddMeter("Examples.AspNetCore")
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddOtlpExporter(cfg => cfg.Endpoint = new Uri(grafanaUrl));
            })
            .WithTracing(tracing =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    // We want to view all traces in development
                    tracing.SetSampler(new AlwaysOnSampler());
                }

                tracing
                        .AddSource("Examples.AspNetCore")
                        .AddAspNetCoreInstrumentation()
                        // Uncomment the following line to enable gRPC instrumentation (requires the OpenTelemetry.Instrumentation.GrpcNetClient package)
                        //.AddGrpcClientInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddEntityFrameworkCoreInstrumentation()
                        .AddOtlpExporter(cfg => cfg.Endpoint = new Uri(grafanaUrl));

                var redis = builder.Configuration.GetConnectionString("Redis");
                if (!string.IsNullOrEmpty(redis))
                    tracing.AddRedisInstrumentation();
            });

        builder.AddOpenTelemetryExporters();

        return builder;
    }

    private static IHostApplicationBuilder AddOpenTelemetryExporters(this IHostApplicationBuilder builder)
    {
        var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);
        var sequrl = builder.Configuration.GetConnectionString("seqlog");

        if (useOtlpExporter)
        {
            //builder.Services.AddOpenTelemetry().UseOtlpExporter();

            //NOTE: LOG要各別設定
            //builder.Services.Configure<OpenTelemetryLoggerOptions>(
            //    logging => logging.AddOtlpExporter());
            builder.Services.ConfigureOpenTelemetryMeterProvider(
                metrics => metrics.AddOtlpExporter());
            builder.Services.ConfigureOpenTelemetryTracerProvider(
                tracing => tracing.AddOtlpExporter());
        }

        //// Uncomment the following lines to enable the Azure Monitor exporter (requires the Azure.Monitor.OpenTelemetry.AspNetCore package)
        //if (!string.IsNullOrEmpty(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
        //{
        //    builder.Services.AddOpenTelemetry()
        //       .UseAzureMonitor();
        //}

        // Configure alternative exporters
        //builder.Services.AddOpenTelemetry()
        //                .WithMetrics(metrics =>
        //                {
        //                    // Uncomment the following line to enable the Prometheus endpoint
        //                    //metrics.AddPrometheusExporter();
        //                });

        return builder;
    }

    public static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            // Add a default liveness check to ensure app is responsive
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        return builder;
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        // Adding health checks endpoints to applications in non-development environments has security implications.
        // See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
        if (app.Environment.IsDevelopment())
        {
            // All health checks must pass for app to be considered ready to accept traffic after starting
            app.MapHealthChecks("/health");

            // Only health checks tagged with the "live" tag must pass for app to be considered alive
            app.MapHealthChecks("/alive", new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("live")
            });
        }

        return app;
    }
}