using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using TodoListWebApp.Utils;
using Microsoft.AspNet.Authentication.OpenIdConnect;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace TodoListWebApp
{
    public partial class Startup
    {
        public static string Authority = String.Empty;
        public static string ClientId = String.Empty;
        public static string AppKey = String.Empty;
        public static string TodoListResourceId = String.Empty;
        public static string TodoListBaseAddress = String.Empty;
        public static string GraphResourceId = String.Empty;

        public void ConfigureAuth(IApplicationBuilder app)
        {
            // Populate AzureAd Configuration Values
            Authority = String.Format(Configuration["AzureAd:AadInstance"], Configuration["AzureAd:Tenant"]);
            ClientId = Configuration["AzureAd:ClientId"];
            AppKey = Configuration["AzureAd:AppKey"];
            TodoListResourceId = Configuration["AzureAd:TodoListResourceId"];
            TodoListBaseAddress = Configuration["AzureAd:TodoListBaseAddress"];
            GraphResourceId = Configuration["AzureAd:GraphResourceId"];

            // Configure the session middleware used for storing tokens.
            app.UseSession();

            // Configure the OWIN pipeline to use cookie auth.
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AutomaticAuthenticate = true
            });

            // Configure the OWIN pipeline to use OpenID Connect auth.
            app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            {
                ClientId = Configuration["AzureAD:ClientId"],
                Authority = String.Format(Configuration["AzureAd:AadInstance"], Configuration["AzureAd:Tenant"]),
                PostLogoutRedirectUri = Configuration["AzureAd:PostLogoutRedirectUri"],
                Events = new OpenIdConnectEvents
                {
                    OnAuthenticationFailed = OnAuthenticationFailed,
                    OnAuthorizationCodeReceived = OnAuthorizationCodeReceived
                }
            });
        }

        private async Task OnAuthorizationCodeReceived(AuthorizationCodeReceivedContext context)
        {
            // Acquire a Token for the Graph API and cache it.  In the TodoListController, we'll use the cache to acquire a token to the Todo List API
            string userObjectId = (context.AuthenticationTicket.Principal.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier"))?.Value;
            ClientCredential clientCred = new ClientCredential(ClientId, AppKey);
            AuthenticationContext authContext = new AuthenticationContext(Authority, new NaiveSessionCache(userObjectId, context.HttpContext.Session));
            AuthenticationResult authResult = await authContext.AcquireTokenByAuthorizationCodeAsync(
                context.Code, new Uri(context.RedirectUri), clientCred, Startup.GraphResourceId);
        }

        private Task OnAuthenticationFailed(AuthenticationFailedContext context)
        {
            context.HandleResponse();
            context.Response.Redirect("/Home/Error?message=" + context.Exception.Message);
            return Task.FromResult(0);
        }
    }
}