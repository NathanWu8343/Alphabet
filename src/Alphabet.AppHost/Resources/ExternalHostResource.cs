using Aspire.Hosting.Lifecycle;

namespace Alphabet.AppHost.Resources
{
    internal class ExternalHostResource(string name, string host) : Resource(name), IResourceWithEndpoints
    {
        public string Host { get; } = host;
    }

    internal static class ExternalHostResourceBuilderExtensions
    {
        public static IResourceBuilder<ExternalHostResource> AddExternalHost(
            this IDistributedApplicationBuilder builder,
            string name,
            string hostname)
        {
            //builder.Services.TryAddLifecycleHook<ExternalHostResourceLifecycleHook>();

            return builder.AddResource(new ExternalHostResource(name, hostname))
                    .WithInitialState(new CustomResourceSnapshot
                    {
                        ResourceType = "Endpoint",
                        State = "Running",
                        Properties = [
                            new (CustomResourceKnownProperties.Source, hostname),
                        new ("Running", KnownResourceStateStyles.Success)
                        ]
                    });
        }

        public static IResourceBuilder<ExternalHostResource> AddExternalHost(
            this IDistributedApplicationBuilder builder,
            string name,
            Uri uri,
            bool isProxied = false,
            bool isExternal = true)
            => builder.AddExternalHost(name, uri.Host)
                .WithEndpoint(targetPort: uri.Port, scheme: uri.Scheme, name: uri.Scheme, isProxied: isProxied, isExternal: isExternal);
    }

    internal sealed class ExternalHostResourceLifecycleHook(ResourceNotificationService notificationService, ResourceLoggerService loggerService) : IDistributedApplicationLifecycleHook
    {
        private readonly CancellationTokenSource _tokenSource = new();

        public async Task BeforeStartAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
        {
            foreach (var resource in appModel.Resources)
            {
                if (resource is ExternalHostResource externalEndpointResource)
                {
                    var uris = new List<UrlSnapshot>();

                    foreach (var annotation in externalEndpointResource.Annotations)
                    {
                        if (annotation is EndpointAnnotation endpointAnnotation)
                        {
                            endpointAnnotation.IsProxied = false;
                            endpointAnnotation.IsExternal = true;

                            if (endpointAnnotation.AllocatedEndpoint is null)
                            {
                                endpointAnnotation.AllocatedEndpoint = new(
                                    endpointAnnotation,
                                    externalEndpointResource.Host,
                                    endpointAnnotation.TargetPort.HasValue
                                        ? endpointAnnotation.TargetPort.GetValueOrDefault()
                                        : new UriBuilder(endpointAnnotation.UriScheme, externalEndpointResource.Host).Uri.Port);
                            }

                            uris.Add(new UrlSnapshot(endpointAnnotation.Name, externalEndpointResource.GetEndpoint(endpointAnnotation.Name).Url, false));
                        }
                    }

                    if (uris.Count != 0)
                    {
                        await notificationService.PublishUpdateAsync(resource, state => state with
                        {
                            Urls = [.. state.Urls, .. uris]
                        });
                    }
                }
            }
        }
    }
}