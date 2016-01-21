using Microsoft.AspNet.Builder;
using System;

namespace TodoListService
{
    public partial class Startup
    {
        public void ConfigureAuth(IApplicationBuilder app)
        {
            // Configure the app to use OAuth Bearer Authentication
            app.UseOAuthBearerAuthentication(options =>
            {
                options.Audience = Configuration["AzureAd:Audience"];
                options.Authority = String.Format(Configuration["AzureAd:AadInstance"], Configuration["AzureAd:Tenant"]);
            });
        }
    }
}