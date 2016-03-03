using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TodoListWebApp
{
    public partial class Startup
    {
        public Startup()
        {
            // Setup configuration sources.
            Configuration = new ConfigurationBuilder()
               .AddJsonFile("config.json")
               .AddEnvironmentVariables()
               .AddUserSecrets()
               .Build();
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add MVC services to the services container.
            services.AddMvc();

            // Add Session Middleware
            services.AddCaching();
            services.AddSession();

            // Add Authentication services.
            services.AddAuthentication(sharedOptions => sharedOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme);
        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerfactory)
        {
            // Add the console logger.
            loggerfactory.AddConsole(LogLevel.Debug);

            // Add static files to the request pipeline.
            app.UseStaticFiles();

            // Configure error handling middleware.
            app.UseExceptionHandler("/Home/Error");

            // Configure the OpenIdConnect pipeline and required services.
            ConfigureAuth(app);

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
