using Alphabet.AppHost.Resources;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace Alphabet.AppHost.Extensions
{
    internal static class ExternalContainerExtensions
    {
        public static IResourceBuilder<ExternalContainerResource> AddRedisLocalContainer(
               this IDistributedApplicationBuilder builder,
               string containerNameOrId,
               int port)
        {
            return builder.AddExternalContainer("redis", containerNameOrId, "tcp", port);
        }
    }
}