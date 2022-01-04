using Launcher.Services;
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
using System.Windows.Shapes;
using Microsoft.Extensions.DependencyInjection;
using Launcher.Models;
using Launcher.Managers;
using Dynastio.Api;

namespace Launcher.Windows
{
    /// <summary>
    /// Interaction logic for PrivateServerWindow.xaml
    /// </summary>
    public partial class PrivateServerWindow : Window
    {
        private readonly IServiceProvider _services;
        private readonly GameManager _gameManager;
        public PrivateServerWindow(IServiceProvider services)
        {
            this._services = services;
            _gameManager = services.GetRequiredService<GameManager>();
            InitializeComponent();
            Initialize();
        }
        public IGame Game { get; set; }
        public PrivateServerDataManager dataManager { get; set; }
        List<PSAccount> pSAccounts { get; set; }
        public void Initialize()
        {
            Game = _gameManager.GetSelectedGame();
            this.LabelNote.Text = $"For Running private server, Open \"{FileManager.InstallGamesPath}{Game.Id}\\run_private.bat\"";
            if (Game == null)
            {
                MessageBox.Show("Install A Game From Game Manager First.");
                this.Close();
                return;
            }
            dataManager = new PrivateServerDataManager($@"Data Source=bin\games\{Game.Id}\data.sqlite");
            dataManager.OpenConnection();
            Refresh();
            DGAccounts.SelectionChanged += DGAccounts_SelectionChanged;
            CBSlot.SelectionChanged += CBSlot_SelectionChanged;
            CBItem.ItemsSource = Enum.GetNames(typeof(Dynastio.Api.ItemType));
        }
        public void Refresh()
        {
            this.GridMain.Visibility = Visibility.Hidden;
            _ = Task.Run(() =>
            {
                pSAccounts = dataManager.GetAllUsers();
                this.Dispatcher.Invoke(() =>
                {
                    this.DGAccounts.ItemsSource = null;
                    this.DGAccounts.ItemsSource = pSAccounts.Take(1000);
                    this.GridMain.Visibility = Visibility.Visible;
                });
            });

        }
        private void CBSlot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int index = int.Parse((CBSlot.SelectedValue as dynamic).Content);
                var item = personalchest.Pchest.items.Where(a => a.index == index).FirstOrDefault();
                TBDurablity.Text = item.Durablity.ToString();
            }
            catch
            {
                MessageBox.Show("error: report this to developers\ninfo:CBSlot_Selection");
            }
        }

        PSAccount selectedAccount { get; set; }
        private void DGAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedAccount = DGAccounts.SelectedValue as PSAccount;
            if (selectedAccount == null) return;
            UpdateView();
        }
        void UpdateView()
        {
            this.LabelName.Content = $"Name {selectedAccount.name}";
            this.LabelId.Content = $"Id {selectedAccount.id}";
            this.TBCoins.Text = selectedAccount.coins.ToString();
            this.LabelDetails.Content = $"Details: Role: {selectedAccount.role}";
            this.BadgeAdministrator.IsChecked = selectedAccount.badges.Contains(Dynastio.Api.BadgeType.Administrator);
            this.BadgeDeveloper.IsChecked = selectedAccount.badges.Contains(Dynastio.Api.BadgeType.Developer);
            this.BadgeFriend.IsChecked = selectedAccount.badges.Contains(Dynastio.Api.BadgeType.Friend);
            this.BadgeMonthly.IsChecked = selectedAccount.badges.Contains(Dynastio.Api.BadgeType.Monthly);
            this.BadgeSupporter.IsChecked = selectedAccount.badges.Contains(Dynastio.Api.BadgeType.Supporter);
            this.BadgeTranslator.IsChecked = selectedAccount.badges.Contains(Dynastio.Api.BadgeType.Translator);
            this.BadgeYoutuber.IsChecked = selectedAccount.badges.Contains(Dynastio.Api.BadgeType.Youtuber);

            var data = dataManager.GetUserChest(selectedAccount.id).FirstOrDefault();
            if (data != null) this.personalchest.SetPersonalChest(data);
        }
        private void BtnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            string name = TBName.Text;
            string region = TBRegion.Text;
            string map = TBMap.Text;
            PrivateServerManager.CreateBatFile(Game, name, region, map);
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            var filter = TBSearch.Text;
            this.DGAccounts.ItemsSource = pSAccounts.Where(a => a.id.Contains(filter.ToLower()) || a.name.ToLower().Trim().Contains(filter.ToLower().Trim())).ToList();
        }

        private void BtnUpdateUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.selectedAccount.badges.Clear();
                if (BadgeAdministrator.IsChecked.Value) this.selectedAccount.badges.Add(Dynastio.Api.BadgeType.Administrator);
                if (BadgeDeveloper.IsChecked.Value) this.selectedAccount.badges.Add(Dynastio.Api.BadgeType.Developer);
                if (BadgeFriend.IsChecked.Value) this.selectedAccount.badges.Add(Dynastio.Api.BadgeType.Friend);
                if (BadgeMonthly.IsChecked.Value) this.selectedAccount.badges.Add(Dynastio.Api.BadgeType.Monthly);
                if (BadgeSupporter.IsChecked.Value) this.selectedAccount.badges.Add(Dynastio.Api.BadgeType.Supporter);
                if (BadgeTranslator.IsChecked.Value) this.selectedAccount.badges.Add(Dynastio.Api.BadgeType.Translator);
                if (BadgeYoutuber.IsChecked.Value) this.selectedAccount.badges.Add(Dynastio.Api.BadgeType.Youtuber);

                selectedAccount.coins = int.Parse(TBCoins.Text.Trim());

                this.dataManager.UpdateUser(selectedAccount);
                this.dataManager.UpdatePersonalChest(this.personalchest.Pchest, selectedAccount.id);
            }
            catch
            {
                MessageBox.Show("report this to developers, can not update user, coins must be number only.");
            }
            MessageBox.Show("done");
        }

        private void BtnSetSlot_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int index = int.Parse((CBSlot.SelectedValue as dynamic).Content);
                string sitem = CBItem.SelectedValue.ToString();
                ItemType _item = (ItemType)Enum.Parse(typeof(ItemType), sitem);

                var pchest = personalchest.Pchest;
                var item = pchest.items.Where(a => a.index == index).FirstOrDefault();
                pchest.items.Remove(item);
                item.ItemType = _item;
                item.Count = 1;
                item.Durablity = int.Parse(TBDurablity.Text);
                pchest.items.Add(item);
                personalchest.Pchest = pchest;
            }
            catch
            {
                MessageBox.Show("error,make sure Durablity is a number if its already, then report this to developers");
            }
        }

        private void BtnRefreshData_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
            MessageBox.Show("Done");
        }

        private void BtnRunPrivateServer_Click(object sender, RoutedEventArgs e)
        {
            PrivateServerManager.OpenBatFile(Game);
        }
    }
}
