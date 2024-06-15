using Alphabet.AppHost.Resources;

namespace Alphabet.AppHost.Extensions
{
    internal static class ExternalContainerExtensions
    {
        public static IResourceBuilder<ExternalContainerResource> AddRedisLocalContainer(
               this IDistributedApplicationBuilder builder,
               string containerNameOrId)
        {
            return builder.AddExternalContainer("redis", containerNameOrId, "tcp");
        }
    }
}