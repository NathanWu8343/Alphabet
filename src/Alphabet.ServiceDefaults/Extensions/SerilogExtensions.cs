using Aspire.Seq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Configuration;
using Serilog.Enrichers.Span;
using Serilog.Sinks.OpenTelemetry;

namespace Alphabet.ServiceDefaults.Extensions
{
    internal static class SerilogExtensions
    {
        /// <summary>
        /// Configure Serilog to write to the console and OpenTelemetry for Aspire structured logs.
        /// </summary>
        /// <remarks>
        /// ⚠ This method MUST be called before the <see cref="OpenTelemetryLoggingExtensions.AddOpenTelemetry(ILoggingBuilder)"/> method to still send structured logs via OpenTelemetry. ⚠
        /// </remarks>
        public static IHostApplicationBuilder ConfigureSerilog(this IHostApplicationBuilder builder)
        {
            var env = builder.Environment.EnvironmentName;
            var sqlExporter = builder.Configuration.GetConnectionString("seqlog");
            var otlpExporter = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
            var serviceName = builder.Configuration["OTEL_SERVICE_NAME"] ?? "Unknown";
            Log.Logger.Information("App Service name {Name}", serviceName);

            builder.Services.AddSerilog((sp, loggerConfiguration) =>
            {
                //Configure Serilog as desired here for every project (or use IConfiguration for configuration variations between projects)
                loggerConfiguration
                    .ReadFrom.Configuration(builder.Configuration)
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithProcessId()
                    .Enrich.WithProcessName()
                    .Enrich.WithThreadId()
                    .Enrich.WithSpan()
                    .WriteTo.Console();

                if (!string.IsNullOrEmpty(sqlExporter))
                    loggerConfiguration.WriteTo.OpenTelemetryWithSeq(sqlExporter, serviceName, env, builder.Configuration);

                if (!string.IsNullOrEmpty(otlpExporter))
                    loggerConfiguration.WriteTo.OpenTelemetryWithAspire(otlpExporter, serviceName, env, builder.Configuration);
            });

            // Removes the built-in logging providers
            builder.Logging.ClearProviders().AddSerilog();

            return builder;
        }
    }

    internal static class OpenTelemetryExtensions
    {
        public static LoggerConfiguration OpenTelemetryWithSeq(this LoggerSinkConfiguration sinkConfiguration,
            string endpoint, string serviceName, string environment, IConfiguration configuration)
        {
            var settings = new SeqSettings();
            configuration.GetSection("Aspire:Seq").Bind(settings);

            return sinkConfiguration.OpenTelemetry(options =>
            {
                options.IncludedData = IncludedData.TraceIdField | IncludedData.SpanIdField;

                options.Endpoint = $"{endpoint}/ingest/otlp/v1/logs";
                options.Protocol = OtlpProtocol.HttpProtobuf;

                if (string.IsNullOrEmpty(settings.ApiKey) == false)
                    options.Headers.Add("X-Seq-ApiKey", settings.ApiKey!);

                options.ResourceAttributes.Add("service.name", serviceName);
                options.ResourceAttributes.Add("environment.name", environment);
            });
        }

        public static LoggerConfiguration OpenTelemetryWithAspire(this LoggerSinkConfiguration sinkConfiguration,
            string endpoint, string serviceName, string environment, IConfiguration configuration)
        {
            return sinkConfiguration.OpenTelemetry(options =>
            {
                options.IncludedData = IncludedData.TraceIdField | IncludedData.SpanIdField;
                options.Endpoint = endpoint;
                options.ResourceAttributes.Add("service.name", serviceName);
                options.ResourceAttributes.Add("environment.name", environment);

                AddHeaders(options.Headers, configuration["OTEL_EXPORTER_OTLP_HEADERS"]!);
                AddResourceAttributes(options.ResourceAttributes, configuration["OTEL_RESOURCE_ATTRIBUTES"]!);

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
    }
}