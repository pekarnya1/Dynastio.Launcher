using Launcher.Extensions;
using Launcher.Models;
using Launcher.Pages;
using Launcher.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
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

namespace Launcher.Controls
{
    /// <summary>
    /// Interaction logic for GameItemControl.xaml
    /// </summary>
    public partial class GameItemControl : UserControl
    {
        private readonly IServiceProvider _services;
        private readonly PageGames _parent;
        private readonly AppManager _appManager;
        private readonly GameManager _gameManager;

        private readonly IGame _game;
        public GameItemControl(IServiceProvider services, PageGames parent, IGame game)
        {
            _services = services;
            this._parent = parent;
            this._game = game;
            this._appManager = _services.GetRequiredService<AppManager>();
            this._gameManager = _services.GetRequiredService<GameManager>();

            GameId = game.Id;
            InitializeComponent();
            Initialize();

        }
        public string GameId { get; set; }
        public void Initialize()
        {
            if (_game.Update == null) UpdateControlToInstalledMode();
            else UpdateControlToDownloadMode();

        }
        void UpdateControlToDownloadMode()
        {
            this.ProgressBar.Visibility = Visibility.Hidden;
            this.LabelTittle.Text = _game.Tittle;
            this.LabelDetails.Text = _game.Details;
            this.LabelDescription.Text = _game.Description;
            this.LabelAttention.Text = " Update available v" + _game.Version;
            this.ImageAvatar.Source = new BitmapImage(new Uri(_game.ImageDirectory.ResourcesPath()));

            BtnAction.Content = "Download & Install";
            BtnAction.IsEnabled = true;
            BtnOpenFolder.IsEnabled = false;
            BtnUninstall.IsEnabled = true;
        }
        void UpdateControlToFaildDownloadingMode()
        {
            this.ProgressBar.Visibility = Visibility.Hidden;
            this.LabelTittle.Text = _game.Tittle;
            this.LabelDetails.Text = _game.Details;
            this.LabelDescription.Text = _game.Description;
            this.LabelAttention.Text = "";
            this.ImageAvatar.Source = new BitmapImage(new Uri(_game.ImageDirectory.ResourcesPath()));

            BtnAction.Content = "Retry";
            BtnAction.IsEnabled = true;
            BtnOpenFolder.IsEnabled = false;
            BtnUninstall.IsEnabled = true;
        }
        public void UpdateControlToDownloadingMode()
        {
            BtnAction.Content = "Cancel";
            BtnUninstall.IsEnabled = false;
            BtnOpenFolder.IsEnabled = false;
            this.ProgressBar.Visibility = Visibility.Visible;

        }
        public void UpdateControlToInstalledMode()
        {
            this.ProgressBar.Visibility = Visibility.Hidden;
            this.LabelAttention.Text = "";
            this.LabelTittle.Text = _game.Tittle;
            this.LabelDetails.Text = _game.Details;
            this.LabelDescription.Text = _game.Description;
            this.ImageAvatar.Source = new BitmapImage(new Uri(_game.ImageDirectory.ResourcesPath()));

            BtnAction.IsEnabled = true;
            BtnUninstall.IsEnabled = true;
            BtnOpenFolder.IsEnabled = true;

            if (_game.IsSelected)
            {
                this.BtnAction.Content = "Launch";
                this.BtnOpenFolder.Content = "Modify";
            }
            else
            {
                BtnAction.Content = "Select As Main";
                this.BtnOpenFolder.Content = "Open Folder";
            }
        }
        public void UpdateControlToInstallingMode()
        {
            this.Dispatcher.Invoke(() =>
            {
                this.ProgressBar.Visibility = Visibility.Visible;
                this.LabelDetails.Text = "Installing...";
                this.ProgressBar.Value = 50;
                BtnOpenFolder.IsEnabled = false;
                BtnAction.IsEnabled = false;
            });
        }

        public void InstallGame()
        {
            UpdateControlToInstallingMode();
            _ = Task.Run(() =>
            {
                try
                {
                    _gameManager.InstallGame(_game);
                    this.Dispatcher.Invoke(() => UpdateControlToInstalledMode());
                }
                catch
                {
                    StopDownload();
                }
            });
        }
        void StopDownload()
        {
            _client.CancelAsync();
            _client.Dispose();
            this.Dispatcher.Invoke(() =>
            {
                UpdateControlToFaildDownloadingMode();
            });
        }
        public void UpdateDownloadStatus(string details, int progressValue)
        {
            this.LabelDetails.Text = details;
            ProgressBar.Value = progressValue;
        }

        WebClient _client;
        void Download(bool retryMode = false)
        {
            UpdateControlToDownloadingMode();
            _ = Task.Run(async () =>
            {
                FileManager.DeleteGameFiles(_game, true, false);

                _client = new WebClient();
                _client.Headers["User-Agent"] = "User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36";

                _client.DownloadFileCompleted += _client_DownloadFileCompleted;
                void _client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
                {
                    if (e.Error == null || !e.Cancelled)
                    {
                        InstallGame();
                        return;
                    }
                    StopDownload();
                }

                _client.DownloadProgressChanged += _client_DownloadProgressChanged;
                void _client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs ee)
                {
                    double bytesIn = double.Parse(ee.BytesReceived.ToString());
                    double totalBytes = double.Parse(ee.TotalBytesToReceive.ToString());
                    double percentage = bytesIn / totalBytes * 100;

                    this.Dispatcher.Invoke(() =>
                    {
                        var details = string.Format("Downloading {0} of {1}", ee.BytesReceived.SizeSuffix(), ee.TotalBytesToReceive.SizeSuffix());
                        var progressValue = int.Parse(Math.Truncate(percentage).ToString());
                        UpdateDownloadStatus(details, progressValue);
                    });
                }

                await _client.DownloadFileTaskAsync(new Uri(_game.Update.DownloadLink), $@"{FileManager.DownloadGamePath}{_game.Id}.dy");
            });
        }



        private void BtnAction_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Content)
            {
                case "Download & Install": Download(false); break;
                case "Retry": Download(true); break;
                case "Launch": LaunchGame(); break;
                case "Cancel": StopDownload(); break;
                case "Select As Main": SetGameAsMain(); break;
            }
        }
        public void LaunchGame()
        {
            this.BtnAction.IsEnabled = false;
            this.Visibility = Visibility.Hidden;
            GameManager.OpenGameAsync(_game);
            this.Visibility = Visibility.Visible;
            this.BtnAction.IsEnabled = true;
        }
        public void SetGameAsMain()
        {
            this._parent.UnSelectAllItems();
            this.BtnAction.Content = "Launch";
            this.BtnOpenFolder.Content = "Modify";
            _gameManager.UnselectAllGames();
            _game.IsSelected = true;
            FileManager.SaveConfiguration(_appManager.Configuration);
        }
        private void BtnOpenPath_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Content)
            {
                case "Open Folder": Process.Start($@"{FileManager.InstallGamesPath}{_game.Id}\"); break;
                case "Modify": _parent.ChangeView(_parent.GridModify); break;
            }
        }

        private void BtnUninstall_Click(object sender, RoutedEventArgs e)
        {
            _gameManager.RemoveGame(_game);
            _parent.PanelItems.Children.Remove(this);
            FileManager.DeleteGameFiles(_game);
        }

    }
}
