using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;
using Microsoft.AspNet.Security.Cookies;
using Microsoft.Data.Entity;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Microsoft.Framework.Logging.Console;
using TodoListWebApp.Models;
using Microsoft.AspNet.Security;
using Microsoft.AspNet.Security.OpenIdConnect;
using Microsoft.AspNet.Security.Notifications;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Globalization;
using TodoListWebApp.Utils;

namespace TodoListWebApp
{
    public class Startup
    {
        public static string Authority = String.Empty;
        public static string ClientId = String.Empty;
        public static string AppKey = String.Empty;
        public static string TodoListResourceId = String.Empty;
        public static string TodoListBaseAddress = String.Empty;

        public Startup(IHostingEnvironment env)
        {
            // Setup configuration sources.
            Configuration = new Configuration()
                .AddJsonFile("config.json")
                .AddEnvironmentVariables();

            Authority = String.Format(Configuration.Get("AzureAd:AadInstance"), Configuration.Get("AzureAd:Tenant"));
            ClientId = Configuration.Get("AzureAd:ClientId");
            AppKey = Configuration.Get("AzureAd:AppKey");
            TodoListResourceId = Configuration.Get("AzureAd:TodoListResourceId");
            TodoListBaseAddress = Configuration.Get("AzureAd:TodoListBaseAddress");
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add MVC services to the services container.
            services.AddMvc();
            services.AddCachingServices();
            services.AddSessionServices();

            services.Configure<ExternalAuthenticationOptions>(options =>
            {
                options.SignInAsAuthenticationType = CookieAuthenticationDefaults.AuthenticationType;
            });

        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerfactory)
        {
            // Configure the HTTP request pipeline.
            // Add the console logger.
            loggerfactory.AddConsole();


            // Add the following to the request pipeline only in development environment.
            if (string.Equals(env.EnvironmentName, "Development", StringComparison.OrdinalIgnoreCase))
            {
                app.UseBrowserLink();
                app.UseErrorPage(ErrorPageOptions.ShowAll);
            }
            else
            {
                // Add Error handling middleware which catches all application specific errors and
                // send the request to the following path or controller action.
                app.UseErrorHandler("/Home/Error");
            }

            // Add static files to the request pipeline.
            app.UseStaticFiles();

            // Configure the app with session middleware, which we will use to store tokens.
            app.UseSession();

            // Configure the OWIN Pipeline to use OpenID Connect Authentication
            app.UseCookieAuthentication(options => { });

            app.UseOpenIdConnectAuthentication(options =>
            {
                options.ClientId = Configuration.Get("AzureAd:ClientId");
                options.Authority = Authority;
                options.PostLogoutRedirectUri = Configuration.Get("AzureAd:PostLogoutRedirectUri");
                options.RedirectUri = Configuration.Get("AzureAd:PostLogoutRedirectUri");
                options.Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthorizationCodeReceived = OnAuthorizationCodeReceived
                };
            });

            // Add MVC to the request pipeline.
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });

                // Uncomment the following line to add a route for porting Web API 2 controllers.
                // routes.MapWebApiRoute("DefaultApi", "api/{controller}/{id?}");
            });
        }

        public async Task OnAuthorizationCodeReceived(AuthorizationCodeReceivedNotification notification)
        {
            string userObjectId = notification.AuthenticationTicket.Principal.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
            ClientCredential clientCred = new ClientCredential(ClientId, AppKey);
            AuthenticationContext authContext = new AuthenticationContext(Authority, new NaiveSessionCache(userObjectId, notification.HttpContext.Session));
            AuthenticationResult authResult = await authContext.AcquireTokenByAuthorizationCodeAsync(
                notification.Code, new Uri(notification.RedirectUri), clientCred, Startup.TodoListResourceId);

        }
    }
}
