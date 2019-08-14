using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmeuImporter.Services;
using SmeuImporter.Services.Implementation;

namespace SmeuImporter
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var services = new ServiceCollection();
            ServicesFactory.Configure(services, configuration);
            var serviceProvider = services.BuildServiceProvider();
            using (serviceProvider)
            {
                var main = serviceProvider.GetService<IMain>();
                await main.ExecuteAsync(args, configuration);
            }
        }
    }
}