using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace FootyApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IConfiguration Configuration { get; private set; }

        public App()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{GetEnvironment()}.json", optional: true);

            Configuration = builder.Build();
        }

        private static string GetEnvironment()
        {
            return System.Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
        }
    }

}
