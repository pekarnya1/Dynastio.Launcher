using Dynastio.Api;
using Launcher.Models;
using Launcher.Services;
using Launcher.Windows;
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
        private readonly LauncherWindow _parent;
        public PageHome(IServiceProvider service,LauncherWindow parent)
        {
            this._services = service;
            this._appManager = service.GetRequiredService<AppManager>();
            this._dynastClient = _services.GetRequiredService<DynastClient>();
            this._gameManager = _services.GetRequiredService<GameManager>();
            this._rpcManager = _services.GetRequiredService<RpcManager>();
            this._parent = parent;
            InitializeComponent();
            Initialize();
        }
        void Initialize()
        {
            try
            {
                TextBoxNickname.Text = GameManager.GetPlayerNickname();
            }
            catch { };

            try
            {
                var game = _appManager.Configuration.DynastioGames.Where(a => a.IsSelected).FirstOrDefault();
                if (game != null)
                {
                    if(game.Update != null)
                    {
                        BtnPlay.Content = "Udpate";
                        LabelNotice.Text = "New Update is available";
                        return;
                    }
                    BtnPlay.IsEnabled = true;
                    LabelNotice.Text = $"Private-Servers are {(game.IncludePrivateServers ? "On" : "Off")}, You can change it from Manager.";
                }
                else
                {
                    BtnPlay.Content = "Install";
                    LabelNotice.Text = "Install or select a game from Manager first.";
                }
            }
            catch { }
        }


        private async void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            if(BtnPlay.Content.Equals("Install") || BtnPlay.Content.Equals("Update"))
            {
                _parent.ChangeView(LauncherWindow.Views.BtnManager);
                return;
            }
            try
            {
                try
                {
                    this._rpcManager.UpdateActivityToDynastio($"Playing {_gameManager.GetSelectedGame()}", $"Server {GameManager.GetSelectedServer()}");
                    GameManager.ChnagePlayerNickname(TextBoxNickname.Text);
                }
                catch { }

                await _gameManager.OpenSelectedGameAsync();
                this._rpcManager.UpdateActivityToLauncher("On Launcher", "Play Window");

            }
            catch
            {
                LabelNotice.Text = "Unknown Error";
            }
        }
    }
}
