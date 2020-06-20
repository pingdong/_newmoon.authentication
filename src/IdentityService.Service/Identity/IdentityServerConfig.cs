using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace PingDong.Newmoon.IdentityService.Infrastructure
{
    public class IdentityServerConfig
    {
        public static IEnumerable<ApiScope> GetApiScopes()
        {
            // https://docs.identityserver.io/en/latest/reference/api_scope.html
            return new List<ApiScope>
            {
                new ApiScope (name: "events", displayName: "Events API"),
                new ApiScope (name: "places", displayName: "Places API")
            };
        }

        // http://docs.identityserver.io/en/release/reference/client.html
        public static IEnumerable<Client> GetClients(IConfiguration config)
        {
            // client credentials client
            return new List<Client>
                {
                    new Client
                    {
                        ClientId = "events_api",
                        ClientSecrets = { new Secret("events_api-seCrEt".Sha256()) },
                        AllowedGrantTypes = GrantTypes.ClientCredentials,
                        AllowedScopes = { "events" }
                    },
                    new Client
                    {
                        ClientId = "places_api",
                        ClientSecrets = { new Secret("places_api-seCrEt".Sha256()) },
                        AllowedGrantTypes = GrantTypes.ClientCredentials,
                        AllowedScopes = { "places" }
                    }
                };
        }
    }
}
