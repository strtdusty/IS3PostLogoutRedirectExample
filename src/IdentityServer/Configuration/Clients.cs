using System.Collections.Generic;
using IdentityServer3.Core.Models;

namespace IdentityServer.Configuration
{
    public class Clients
    {
        public static List<Client> Get()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientName = "Test Client",
                    ClientId = "test",
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },

                    // server to server communication
                    Flow = Flows.ClientCredentials,

                    // only allowed to access api1
                    AllowedScopes = new List<string>
                    {
                        "api1"
                    }
                },

                new Client
                       {
                           ClientName = "Azure Client",
                           ClientId = "azure",
                           Enabled = true,
                           AccessTokenType = AccessTokenType.Jwt,
                           IncludeJwtId = false,
                           AllowAccessToAllScopes = true,
                           EnableLocalLogin = false,
                           PostLogoutRedirectUris = new List<string> { "http://localhost:9248/", "https://localhost:44302/" },
                           RedirectUris = new List<string> { "http://localhost:9248/" ,"https://localhost:44302/"},
                           Flow = Flows.Implicit,
                           RequireConsent = false
                       }
            };
        }
    }
}