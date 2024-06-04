using IdentityModel;
using IdentityServer4.Models;

namespace IdentityServer.Configs
{
    internal sealed class IdentityConfig
    {
        public static IEnumerable<Client> GetClients()
        {
            return
            [
                new Client()
                {
                     ClientId = "client1",
                     ClientSecrets ={new Secret("secret".Sha256())},
                     AllowedScopes = ["full.access"],
                     AllowedGrantTypes = GrantTypes.ClientCredentials
                 }
            ];
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return
            [
                new ApiScope("full.access","Full Access")
            ];
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return
            [
                new ApiResource("urlshortener-api", "UrlShortener Api")
                {
                    Scopes =  { "full.access"},
                    UserClaims = {JwtClaimTypes.Role }
                },
            ];
        }

        public static IEnumerable<IdentityResource> GetResources()
        {
            return
            [
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            ];
        }
    }
}