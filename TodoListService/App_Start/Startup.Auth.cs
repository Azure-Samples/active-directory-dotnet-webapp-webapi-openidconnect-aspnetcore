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
                options.Audience = Configuration.Get("AzureAd:Audience");
                options.Authority = String.Format(Configuration.Get("AzureAd:AadInstance"), Configuration.Get("AzureAd:Tenant"));
            });
        }
    }
}