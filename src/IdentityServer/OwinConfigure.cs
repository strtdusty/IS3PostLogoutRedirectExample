using IdentityServer3.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using Serilog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Threading.Tasks;
using static IdentityServer3.Core.Constants;

namespace IdentityServer
{
    public partial class Startup
    {
        public static void UseOpenIdConnectAuthentications(IAppBuilder app, string signInAsType)
        {

            var options = new OpenIdConnectAuthenticationOptions
            {
                AuthenticationType = "icad",
                Caption = "Sign In",
                Scope = "openid email",
                ClientId = ,
                Authority = "https://login.windows.net/common/",
                PostLogoutRedirectUri = "http://localhost:1080/signoutcallback",
                RedirectUri = "http://localhost:1080/identity/signin-icad",
                AuthenticationMode = AuthenticationMode.Passive,
                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false//Validation logic handled in SecurityTokenValidated event below
                },
                Notifications = new OpenIdConnectAuthenticationNotifications()
                {
                    RedirectToIdentityProvider = n =>
                    {

                        if (n.ProtocolMessage.RequestType == Microsoft.IdentityModel.Protocols.OpenIdConnectRequestType.LogoutRequest)
                        {
                            var signOutMessageId = n.OwinContext.Environment.GetSignOutMessageId();
                            if (signOutMessageId != null)
                            {
                                n.OwinContext.Response.Cookies.Append("aad.signout.state", signOutMessageId);
                            }
                        }
                        return Task.FromResult(0);
                    }
                },

                SignInAsAuthenticationType = signInAsType // this MUST come after TokenValidationParameters
            };
            app.Map("/signoutcallback", cleanup =>
            {
                cleanup.Run(async ctx =>
                {
                    var state = ctx.Request.Cookies["aad.signout.state"];
                    ctx.Response.Cookies.Append("aad.signout.state", ".", new Microsoft.Owin.CookieOptions { Expires = DateTime.UtcNow.AddYears(-1) });
                    await ctx.Environment.RenderLoggedOutViewAsync(state);
                });
            });
            Log.Debug("External IdP {@Options}", options);
            app.UseOpenIdConnectAuthentication(options);
        }
    }
}