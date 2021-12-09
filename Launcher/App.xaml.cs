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
        public const string version = "1.0.3";
        public const string discordurl = "https://discord.gg/x5j4cZtnWR";
        public const string dynastioChangelog = "https://dynast.io/changelog.txt";
        public const string dynastioNightlyChangelog = "https://nightly.dynast.io/changelog.txt";
        public const string appChangesUrl = "https://galalzhaleh.github.io/Dynastio.Launcher/changelog.txt";
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
                services.GetRequiredService<GameManager>();

                var app = services.GetRequiredService<AppManager>();
                await app.InitializeAsync();

                AppManager.CreateDirectoriesIfNotExist();

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
