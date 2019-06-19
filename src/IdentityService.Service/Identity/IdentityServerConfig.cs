using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace PingDong.Newmoon.IdentityService.Infrastructure
{
    public class IdentityServerConfig
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            // http://docs.identityserver.io/en/release/reference/api_resource.html
            return new List<ApiResource>
                {
                    new ApiResource (name: "events", displayName: "Events API"),
                    new ApiResource (name: "places", displayName: "Places API")
                };
        }

        // http://docs.identityserver.io/en/release/reference/client.html
        public static IEnumerable<Client> GetClients(IConfiguration config)
        {
            var eventUri = config["EventsServiceUri"];
            var placeUri = config["PlacesServiceUri"];

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
