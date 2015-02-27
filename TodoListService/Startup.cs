using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.AspNet.Security.OAuthBearer;
using Microsoft.AspNet.Security;
using System.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Security.Claims;

namespace TodoListService
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Setup configuration sources.
            Configuration = new Configuration()
                .AddJsonFile("config.json");
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by a runtime.
        // Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Configure the app to use OAuth Bearer Authentication
            app.UseOAuthBearerAuthentication(options =>
            {
                options.Audience = Configuration.Get("AzureAd:Audience");
                options.Authority = String.Format(Configuration.Get("AzureAd:AadInstance"), Configuration.Get("AzureAd:Tenant"));
            });

            app.UseStaticFiles();
            // Add MVC to the request pipeline.
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
