using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Threading.Tasks;
using IdentityServer3.Core.Extensions;
using System.Security.Claims;
using Microsoft.Owin.Security;
using System.Linq;

[assembly: OwinStartup(typeof(MVC_OWIN_Client.Startup))]

namespace MVC_OWIN_Client
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies"
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = "azure",
                Authority = "http://localhost:1080/",
                RedirectUri = "https://localhost:44302/",
                ResponseType = "id_token",
                Scope = "openid email",

                UseTokenLifetime = false,
                SignInAsAuthenticationType = "Cookies",
                PostLogoutRedirectUri= "https://localhost:44302/",
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    SecurityTokenValidated = SecurityTokenValidated,
                    RedirectToIdentityProvider = RedirectToIdentityProvider
                },
            });
        }
        private Task SecurityTokenValidated(
           SecurityTokenValidatedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> arg)
        {
            id_token = arg.ProtocolMessage.IdToken;

            return Task.FromResult(0);
        }
        private string id_token;
        private Task RedirectToIdentityProvider(
            RedirectToIdentityProviderNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> arg)
        {
            if (arg.ProtocolMessage.RequestType == OpenIdConnectRequestType.LogoutRequest)
            {

                if (id_token != null)
                {
                    arg.ProtocolMessage.IdTokenHint = id_token;
                }

                
                var signOutMessageId = arg.OwinContext.Environment.GetSignOutMessageId();
                if (signOutMessageId != null)
                {
                    arg.OwinContext.Response.Cookies.Append("state", signOutMessageId);
                }
            }
            return Task.FromResult(0);
        }

    }
}