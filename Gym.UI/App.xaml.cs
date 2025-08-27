using DotNetEnv;
using Gym.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace Gym.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost? _host;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Load environment variables
            Env.Load();

            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .Build();

            // Create host and configure services
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Register infrastructure services
                    services.InfrastructureConfiguration(configuration);

                    // Register AutoMapper services
                    services.AddAutoMapper(cfg =>
                    {
                        cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies());
                    });

                    // Register UI services
                    services.AddTransient<MainWindow>();
                })
                .Build();

            // Start the host
            _host.Start();

            // Get MainWindow from DI container
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _host?.Dispose();
            base.OnExit(e);
        }
    }
}
