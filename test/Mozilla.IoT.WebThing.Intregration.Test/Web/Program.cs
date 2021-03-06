using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Mozilla.IoT.WebThing.Intregration.Test.Web
{
    public class Program
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logger =>
                {
                    logger.ClearProviders()
                        .AddConsole()
                        .AddFilter("Microsoft.*", LogLevel.Critical);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .UseStartup<Startup>();
                });

        private static IHost s_defaultHost;

        public static ValueTask<IHost> GetHost()
        {
            if (s_defaultHost != null)
            {
                return new ValueTask<IHost>(s_defaultHost);
            }

            return new ValueTask<IHost>(CreateHostBuilderAndStartAsync(null));
        }
        
        private static async Task<IHost> CreateHostBuilderAndStartAsync(string[] args)
        {
            return s_defaultHost = await Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .UseStartup<Startup>();
                }).StartAsync();
        }
    }
}
