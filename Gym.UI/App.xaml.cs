using DotNetEnv;
using Gym.Infrastructure;
using Gym.UI.Services;
using Gym.UI.Services.Dialogs;
using Gym.UI.Services.Reports;
using Gym.UI.ViewModels;
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
            try
            {
                base.OnStartup(e);

                // Capture unhandled UI exceptions to diagnose crashes
                this.DispatcherUnhandledException += (sender, args) =>
                {
                    try
                    {
                        var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.log");
                        File.AppendAllText(logPath, $"===== {DateTime.Now} =====\n{args.Exception}\n\n");
                    }
                    catch { }
                    MessageBox.Show($"Unhandled exception: {args.Exception.Message}\n(Logged to error.log)", "Unhandled Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    args.Handled = true; // prevent silent crash so we can continue diagnosis
                };

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

                        // Register localization service
                        services.AddSingleton<ILocalizationService, LocalizationService>();

                        // Register dialog service
                        services.AddSingleton<IDialogService, DialogService>();
                        
                        // Register report service
                        services.AddTransient<IReportService, ReportService>();

                        // Register ViewModels
                        services.AddTransient<MainViewModel>();
                        
                        // Register UI services
                        services.AddTransient<MainWindow>();
                    })
                    .Build();

                // Start the host
                _host.Start();

                // Get MainWindow from DI container and set up DataContext
                var mainWindow = _host.Services.GetRequiredService<MainWindow>();
                var mainViewModel = _host.Services.GetRequiredService<MainViewModel>();

                // Initialize data sequentially to avoid "second operation" DbContext errors
                // (cannot 'await' directly in OnStartup, so use async void pattern via dispatcher)
                mainWindow.Loaded += async (_, __) =>
                {
                    try
                    {
                        // Use ConfigureAwait(false) to help prevent UI thread deadlocks during initialization
                        await Task.Run(async () =>
                        {
                            await mainViewModel.InitializeAsync().ConfigureAwait(false);
                        }).ConfigureAwait(false);
                    }
                    catch (Exception initEx)
                    {
                        await mainWindow.Dispatcher.InvokeAsync(() =>
                        {
                            MessageBox.Show($"Initialization failed: {initEx.Message}", "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        });
                    }
                };

                mainWindow.DataContext = mainViewModel;
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Application startup failed: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}", 
                               "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _host?.Dispose();
            base.OnExit(e);
        }
    }
}
