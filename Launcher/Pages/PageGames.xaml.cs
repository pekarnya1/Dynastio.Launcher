using Dynastio.Api;
using Launcher.Controls;
using Launcher.Extensions;
using Launcher.Models;
using Launcher.Services;
using Launcher.ViewModels;
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
    /// Interaction logic for PageDynastioGames.xaml
    /// </summary>
    public partial class PageGames : Page
    {
        private readonly IServiceProvider _services;
        private readonly AppManager _appManager;
        private readonly DynastClient _dynastClient;
        private readonly GameManager _gameManager;
        public PageGames(IServiceProvider service)
        {
            this._services = service;
            this._appManager = service.GetRequiredService<AppManager>();
            this._dynastClient = _services.GetRequiredService<DynastClient>();
            this._gameManager = _services.GetRequiredService<GameManager>();

            InitializeComponent();

            _ = Task.Run(async () =>
            {
                try
                {
                    await _gameManager.CheckGameUpdatesAsync();

                    await AddComboBoxVersions();
                }
                catch { }
            });

            ChangeView(this.GridMain);
        }
        public void UpdateGames()
        {
            foreach (var g in _appManager.Configuration.DynastioGames.OrderByDescending(a => a.GetType() == typeof(GameUpdate)))
            {
                if (PanelItems.Children.OfType<GameItemControl>().Where(a => a.GameId == g.Id).Any())
                {

                    continue;
                };
                PanelItems.Children.Add(new GameItemControl(_services, this, g));
            }
            if (PanelItems.Children.OfType<GameItemControl>().Count() == 0)
            {

            }
        }
        public void UnSelectAllItems()
        {
            foreach (var c in PanelItems.Children.OfType<GameItemControl>().Where(a => a.BtnAction.Content.Equals("Launch")))
            {
                c.BtnAction.Content = "Select As Main";
                c.BtnOpenFolder.Content = "Open Folder";
            }
        }
        public void ChangeView(Grid uIElement)
        {
            this.GridMain.Visibility = Visibility.Collapsed;
            this.GridAdd.Visibility = Visibility.Collapsed;
            this.GridModify.Visibility = Visibility.Collapsed;

            switch (uIElement.Name)
            {
                case "GridAdd":
                    {

                    }
                    break;
                case "GridMain":
                    {
                        UpdateGames();
                    }
                    break;
                case "GridModify":
                    {
                        var game = _appManager.Configuration.DynastioGames.Where(a => a.IsSelected).FirstOrDefault();
                        game.IsUpdateAble = CheckBoxModifyUpdateAble.IsChecked.Value;
                        game.IncludePrivateServers = CheckBoxModifyPrivateServers.IsChecked.Value;
                        FileManager.SaveConfiguration(_appManager.Configuration);
                    }
                    break;
            }
            uIElement.Visibility = Visibility.Visible;

        }
        public async Task AddComboBoxVersions()
        {
            var main = await _dynastClient.Main.GetCurrentVersionAsync();
            var nightly = await _dynastClient.GetDatabase("Nightly").GetCurrentVersionAsync();

            var versions = new List<VersionViewModel>()
            {
                new VersionViewModel()
                {
                    url=main.url,
                    version=main.version,
                    details=$"Main {main.version}"
                },
                 new VersionViewModel()
                {
                    url=nightly.url,
                    version=nightly.version,
                    details=$"Nightly {nightly.version}"
                }
            };
            this.Dispatcher.Invoke(() =>
            {
                this.ComboBoxItems.ItemsSource = versions;
            });
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            ChangeView(this.GridAdd);

        }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            ChangeView(this.GridMain);
        }
        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedGame = this.ComboBoxItems.SelectedItem as VersionViewModel;
                _appManager.Configuration.DynastioGames.Add(new Game
                {
                    Id = Guid.NewGuid().ToString(),
                    Tittle = selectedGame.details.Contains("Nightly") ? "Dynastio (Nightly)" : "Dynastio",
                    Type = selectedGame.details.Contains("Nightly") ? GameType.Nightly : GameType.Main,
                    Description = "100 player 2D real-time massive multiplayer survival game! Build, Craft, Fight, Survive!",
                    ImageDirectory = selectedGame.details.Contains("Nightly") ? "Images/dynastio01.jpg" : "Images/dynastio00.jpg",
                    InstalledAt = DateTime.Now,
                    Version = selectedGame.version,
                    IsSelected = false,
                    IsUpdateAble = CheckBoxUpdateAble.IsChecked.Value,
                    IncludePrivateServers = CheckBoxPrivateServers.IsChecked.Value,
                    Update = new GameUpdate()
                    {
                        Version = selectedGame.version,
                        DownloadLink = selectedGame.url,
                        DownloadSize = ConnectionManager.GetFileSize(selectedGame.url),
                    }
                });
            }
            catch { }
            ChangeView(this.GridMain);
        }




        private void BtnModifyConfirm_Click(object sender, RoutedEventArgs e)
        {
            PanelItems.Children.OfType<GameItemControl>().Where(a => a.BtnAction.Content.Equals("Launch")).FirstOrDefault().UpdateControlToInstalledMode();
            ChangeView(GridMain);
        }

        private void BtnReinstall_Click(object sender, RoutedEventArgs e)
        {
            PanelItems.Children.OfType<GameItemControl>().Where(a => a.BtnAction.Content.Equals("Launch")).FirstOrDefault().InstallGame();
            ChangeView(GridMain);
        }
    }
}
