using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer3.Core.Configuration;
using IdentityServer.Configuration;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Extensions.Logging;
using Serilog;
using Owin;
using System.Collections.Generic;
using IdentityServer3.Core;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security;
using System.IdentityModel.Tokens;
using System.Threading.Tasks;
using IdentityServer3.Core.Extensions;
using System;

namespace IdentityServer
{
    public partial class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection();
        }
        
        public void Configure(IApplicationBuilder app, IApplicationEnvironment env, ILoggerFactory loggerFactory)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole()
                .CreateLogger();

            loggerFactory.AddConsole();
            loggerFactory.AddDebug();

            app.UseIISPlatformHandler();
            

            var certFile = env.ApplicationBasePath + $"{System.IO.Path.DirectorySeparatorChar}idsrv3test.pfx";

            var idsrvOptions = new IdentityServerOptions
            {
                Factory = new IdentityServerServiceFactory()
                                .UseInMemoryUsers(Users.Get())
                                .UseInMemoryClients(Clients.Get())
                                .UseInMemoryScopes(Scopes.Get()),

                SigningCertificate = new X509Certificate2(certFile, "idsrv3test"),
                AuthenticationOptions = new IdentityServer3.Core.Configuration.AuthenticationOptions
                {
                    IdentityProviders = UseOpenIdConnectAuthentications,
                    EnablePostSignOutAutoRedirect = true,
                    CookieOptions = new IdentityServer3.Core.Configuration.CookieOptions() { SlidingExpiration = true }
                },
                RequireSsl = false
            };

            app.UseIdentityServer(idsrvOptions);
        }

        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
