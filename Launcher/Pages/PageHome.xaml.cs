using Dynastio.Api;
using Launcher.Models;
using Launcher.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Launcher.Pages
{
    /// <summary>
    /// Interaction logic for PageHome.xaml
    /// </summary>
    public partial class PageHome : Page
    {
        private readonly IServiceProvider _services;
        private readonly AppManager _appManager;
        private readonly DynastClient _dynastClient;
        private readonly GameManager _gameManager;
        private readonly RpcManager _rpcManager;
        public PageHome(IServiceProvider service)
        {
            this._services = service;
            this._appManager = service.GetRequiredService<AppManager>();
            this._dynastClient = _services.GetRequiredService<DynastClient>();
            this._gameManager = _services.GetRequiredService<GameManager>();
            this._rpcManager = _services.GetRequiredService<RpcManager>();

            InitializeComponent();
            Initialize();
            UpdateSelectMenu();
        }
        void Initialize()
        {
            try
            {
                this._rpcManager.UpdateActivityToLauncher("On Launcher", "Play Window");
                TextBoxNickname.Text = GameManager.GetPlayerNickname();
            }
            catch { };
        }
        void UpdateSelectMenu()
        {
            var game = _appManager.Configuration.DynastioGames.Where(a => a.IsSelected).FirstOrDefault();
            if (game != null)
            {
                LabelNotice.Text = $"Private-Servers are {(game.IncludePrivateServers ? "On" : "Off")}, You can change it from Manager.";

                var servers = ServerViewModel.ConvertForSelectMenu(_dynastClient.Main.GetOnlineServers(ServerType.All).ToList());
                servers = servers.Where(a => game.Version.Contains(a.Version)).ToList();

                if (!game.IncludePrivateServers) servers = servers.Where(a => a.IsPrivate == false).ToList();
                if (servers == null || servers.Count < 1)
                {
                    BtnPlay.IsEnabled = false;
                    LabelNotice.Text = "No any online server found for this game, change your selected game from Manager.";
                    return;
                }

                var data = new ListCollectionView(servers);
                data.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
                this.ComboBoxServers.ItemsSource = data;

                BtnPlay.IsEnabled = true;
                return;
            }
            BtnPlay.IsEnabled = false;
            LabelNotice.Text = "Install or select a game from Manager first.";
        }

        private void BtnRefreshServers_Click(object sender, RoutedEventArgs e)
        {
            UpdateSelectMenu();
        }

        private async void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var server = (ComboBoxServers.SelectedItem as ServerViewModel).ServerName;
                this._rpcManager.UpdateActivityToDynastio($"Playing {_gameManager.GetSelectedGame()}", $"Server {server}");

                GameManager.ChnagePlayerNickname(TextBoxNickname.Text);
                GameManager.ChnagePlayerSelectedServer(server);
                await _gameManager.OpenSelectedGameAsync();

                this._rpcManager.UpdateActivityToLauncher("On Launcher", "Play Window");

            }
            catch { }
        }
    }
}
