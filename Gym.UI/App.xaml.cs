using DotNetEnv;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Gym.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Load variables from .env
            Env.Load();
        }

    }
}
