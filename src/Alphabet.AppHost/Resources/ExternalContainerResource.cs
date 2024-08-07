﻿using Aspire.Hosting.Lifecycle;
using CliWrap;
using CliWrap.Buffered;
using CliWrap.EventStream;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace Alphabet.AppHost.Resources
{
    internal sealed class ExternalContainerResource(string name, string containerNameOrId, string schema, int port)
        : Resource(name), IResourceWithEndpoints, IResourceWithConnectionString
    {
        internal const string Host = "localhost"; // 可以替換

        public string Schema { get; } = schema;
        public string ContainerNameOrId { get; } = containerNameOrId;

        public int Port { get; } = port;

        /// <summary>
        /// Gets the connection string expression for the Seq server.
        /// </summary>
        public ReferenceExpression ConnectionStringExpression =>
            ReferenceExpression.Create($"{Schema}://{Host}:{Port.ToString()}");
    }

    internal static class ExternalContainerResourceExtensions
    {
        public static IResourceBuilder<ExternalContainerResource> AddExternalContainer(
            this IDistributedApplicationBuilder builder,
            string name,
            string containerNameOrId,
            string scheme,
            int port)
        {
            builder.Services.TryAddLifecycleHook<ExternalContainerResourceLifecycleHook>();

            return builder.AddResource(new ExternalContainerResource(name, containerNameOrId, scheme, port))
                .WithInitialState(new CustomResourceSnapshot
                {
                    ResourceType = "External",
                    State = "Starting",
                    Properties = [new ResourcePropertySnapshot(CustomResourceKnownProperties.Source, "Container")],
                })
                .ExcludeFromManifest();
        }
    }

    internal sealed class ExternalContainerResourceLifecycleHook(ResourceNotificationService notificationService, ResourceLoggerService loggerService)
    : IDistributedApplicationLifecycleHook, IAsyncDisposable
    {
        private const string DOCKER_HOST = "localhost";
        private readonly CancellationTokenSource _tokenSource = new();
        private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(false);

        public async Task BeforeStartAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
        {
            foreach (var resource in appModel.Resources.OfType<ExternalContainerResource>())
            {
                StartTrackingExternalContainerLogs(resource, _tokenSource.Token);
                await ShowContainerUrlAsync(resource, _tokenSource.Token);
            }
        }

        /// <summary>
        /// Executes after the orchestrator allocates endpoints for resources in the application model.
        /// </summary>
        /// <param name="appModel">The distributed application model.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task AfterEndpointsAllocatedAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task AfterResourcesCreatedAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
        {
            foreach (var resource in appModel.Resources.OfType<ExternalContainerResource>())
            {
                StartHealthCheck(resource, cancellationToken);
            }

            return Task.CompletedTask;
        }

        private async Task ShowContainerUrlAsync(ExternalContainerResource resource, CancellationToken cancellationToken)
        {
            //
            var uri = new UrlSnapshot(resource.Name, $"{resource.Schema}://{DOCKER_HOST}:{resource.Port}", false);
            await notificationService.PublishUpdateAsync(resource, state => state with
            {
                Urls = [.. state.Urls, uri]
            });
        }

        private Task StartHealthCheck(ExternalContainerResource resource, CancellationToken cancellationToken)
        {
            var logger = loggerService.GetLogger(resource);

            _ = Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    _autoResetEvent.WaitOne(); // 等待檢查
                    var result = await Cli.Wrap("docker")
                                .WithArguments(["inspect", resource.ContainerNameOrId, "--format", "'{{json .State.Status}}'"])
                                .ExecuteBufferedAsync(cancellationToken);
                    var stdOut = result.StandardOutput;

                    if (stdOut.Contains("running"))
                        StartTrackingExternalContainerLogs(resource, cancellationToken);
                    else
                        _autoResetEvent.Reset(); // 持續檢查

                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }, cancellationToken);

            return Task.CompletedTask;
        }

        private void StartTrackingExternalContainerLogs(ExternalContainerResource resource, CancellationToken cancellationToken)
        {
            var logger = loggerService.GetLogger(resource);

            _ = Task.Run(async () =>
            {
                var cmd = Cli.Wrap("docker").WithArguments(["logs", "--follow", resource.ContainerNameOrId]);
                var cmdEvents = cmd.ListenAsync(cancellationToken);

                await foreach (var cmdEvent in cmdEvents)
                {
                    switch (cmdEvent)
                    {
                        case StartedCommandEvent:

                            await notificationService.PublishUpdateAsync(resource, state => state with { State = "Running" });
                            break;

                        case ExitedCommandEvent:
                            await notificationService.PublishUpdateAsync(resource, state => state with { State = "Finished" });
                            break;

                        case StandardOutputCommandEvent stdOut:
                            logger.LogInformation("External container {ResourceName} stdout: {StdOut}", resource.Name, stdOut.Text);
                            break;

                        case StandardErrorCommandEvent stdErr:
                            logger.LogInformation("External container {ResourceName} stderr: {StdErr}", resource.Name, stdErr.Text);
                            break;
                    }
                }

                // 開啟 health check
                _autoResetEvent.Reset();
            }, cancellationToken);
        }

        public ValueTask DisposeAsync()
        {
            _tokenSource.Cancel();
            return default;
        }
    }
}