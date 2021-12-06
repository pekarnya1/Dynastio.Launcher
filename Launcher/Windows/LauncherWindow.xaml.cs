using Launcher.Controls;
using Launcher.Extensions;
using Launcher.Pages;
using Launcher.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace Launcher.Windows
{
    /// <summary>
    /// Interaction logic for LauncherWindow.xaml
    /// </summary>
    public partial class LauncherWindow : Window
    {
        private readonly IServiceProvider _services;
        private readonly AppManager _appManager;
        private readonly PageGames _pageDynastio;
        private readonly RpcManager _rpcManager;
        public LauncherWindow(IServiceProvider services)
        {
            this._services = services;
            this._appManager = services.GetRequiredService<AppManager>();
            this._rpcManager = services.GetRequiredService<RpcManager>();
            this._pageDynastio = new PageGames(services);

            InitializeComponent();
            Initialize();
            ChangeView("BtnAppVersion");
        }
        public void Initialize()
        {
            this._rpcManager.UpdateActivityToLauncher("On Launcher", "Not playing yet");
            this.BtnAppVersion.Content = _appManager.Configuration.Version + " v";
            this.Closed += delegate
            {
                this.Visibility = Visibility.Collapsed;
            };
        }
        public void TabItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangeView((sender as TabItem).Header.ToString());
        }
        public void ChangeView(string view)
        {
            switch (view)
            {
                case "BtnPlay": this.FrameMain.Content = new PageHome(_services); break;
                case "BtnManager": this.FrameMain.Content = _pageDynastio; break;
                case "BtnAboutUs": this.FrameMain.Content = new PageAbout(_appManager.Configuration); break;
                case "BtnAccounts": this.FrameMain.Content = new PageAccounts(_services); break;

                case "BtnChangeLogGame": ShowPage(_appManager.Configuration.ChangeLog); break;
                case "BtnChangeLogApp": ShowPage(_appManager.Configuration.AppChangeLog); break;
                case "BtnAppVersion": ShowPage(_appManager.Configuration.SiteUrl); break;
            }
        }
        void ShowPage(string url)
        {
            var web = new WebBrowser();
            web.Navigate(new Uri(url));
            this.FrameMain.Content = web;
        }
        private void LabelAppVersion_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangeView("Main");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ChangeView((sender as Button).Name.ToString());
        }
    }
}
