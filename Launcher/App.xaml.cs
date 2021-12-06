using Launcher.Windows;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Dynastio.Api;
using Launcher.Services;
using Launcher.Extensions;
using Launcher.Models;

namespace Launcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                StartupAsync().GetAwaiter().GetResult();
            }
            catch
            {
                MessageBox.Show("An error occurred, Make sure that you are connected to the internet, if not fixed then report this bug to develoeprs.");

            }
            finally
            {
                Environment.Exit(0);
            }
        }
        public async Task StartupAsync()
        {
            using (var services = ConfigureServices())
            {
                services.GetRequiredService<FileManager>();
                services.GetRequiredService<ConnectionManager>();
                services.GetRequiredService<DynastClient>().WithDatabases("api.json".ResourcesPath());

                services.GetRequiredService<RpcManager>().Intialize();

                services.GetRequiredService<ProfileManager>();
                var game = services.GetRequiredService<GameManager>();
                var app = services.GetRequiredService<AppManager>();
                await app.InitializeAsync();

                AppManager.CreateDirectoriesIfNotExist();

                await game.CheckGameUpdatesAsync();

                var window = new LauncherWindow(services);
                window.ShowDialog();

                Environment.Exit(0);

            }
        }
        ServiceProvider ConfigureServices() => new ServiceCollection()
            .AddSingleton<FileManager>()
            .AddSingleton<GameManager>()
            .AddSingleton<ConnectionManager>()
            .AddSingleton<AppManager>()
            .AddSingleton<DynastClient>()
            .AddSingleton<ProfileManager>()
            .AddSingleton<RpcManager>()

            .BuildServiceProvider();
    }
}
