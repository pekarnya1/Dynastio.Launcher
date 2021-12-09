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
                TextBoxNickname.Text = GameManager.GetPlayerNickname();
                this._rpcManager.UpdateActivityToLauncher("On Launcher", "Play Window");
            }
            catch { };
        }
        void UpdateSelectMenu()
        {
            var game = _appManager.Configuration.DynastioGames.Where(a => a.IsSelected).FirstOrDefault();
            if (game != null)
            {
                LabelNotice.Text = $"Private-Servers are {(game.IncludePrivateServers ? "On" : "Off")}, You can change it from Manager.";

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
                try
                {
                    this._rpcManager.UpdateActivityToDynastio($"Playing {_gameManager.GetSelectedGame()}", $"Server {GameManager.GetSelectedServer()}");
                }
                catch { }
                GameManager.ChnagePlayerNickname(TextBoxNickname.Text);
                await _gameManager.OpenSelectedGameAsync();

                this._rpcManager.UpdateActivityToLauncher("On Launcher", "Play Window");

            }
            catch { }
        }
    }
}
