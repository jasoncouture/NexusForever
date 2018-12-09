using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NexusForever.Shared.Database.Auth.Model;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using NexusForever.StsServer.Api.Controllers;

namespace NexusForever.StsServer.Api
{
    public class ApiStartup
    {
        public IConfiguration Configuration { get; }

        public static void Run(IConfiguration configuration)
        {
            IWebHost host = BuildWebHost(configuration);
            host.Run();
        }
        public static IDisposable Start(IConfiguration configuration)
        {
            IWebHost host = BuildWebHost(configuration);
            host.Start();
            return host;
        }

        private static IWebHost BuildWebHost(IConfiguration configuration)
        {
            var hostBuilder = WebHost.CreateDefaultBuilder<ApiStartup>(Environment.GetCommandLineArgs().Skip(1).ToArray())
                .UseConfiguration(configuration)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Information);
                })
                .UseNLog()
                .UseUrls($"http://localhost:5001")
                .PreferHostingUrls(false);
            return hostBuilder.Build();
        }


        public ApiStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1)
                .ConfigureApplicationPartManager(apm => apm.ApplicationParts.Add(new AssemblyPart(typeof(ApiStartup).Assembly)))
                .AddControllersAsServices();
            services.AddScoped(s => new AuthContext());
            
        }

        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "api/{controller}/{action}/{id?}");

            });
        }
    }
}
