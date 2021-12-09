using Launcher.Controls;
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
    /// Interaction logic for PageAccounts.xaml
    /// </summary>
    public partial class PageAccounts : Page
    {
        private readonly IServiceProvider _services;
        private readonly AppManager _appManager;
        private readonly ProfileManager _profileManager;

        public PageAccounts(IServiceProvider service)
        {
            this._services = service;
            this._appManager = service.GetRequiredService<AppManager>();
            this._profileManager = _services.GetRequiredService<ProfileManager>();
            InitializeComponent();
            UpdateItems();
        }
        public void UpdateItems()
        {
            foreach (var p in _profileManager.GetAll()) AddItem(p);
        }
        public void UpdateControlsView()
        {
            foreach (var c in PanelItems.Children.OfType<ControlUserProfile>())
            {
                c.UpdateControlToProfileView();
            }
        }
        public void RemoveItem(ControlUserProfile ctrl)
        {
            PanelItems.Children.Remove(ctrl);
        }
        public void AddItem(UserProfile profile)
        {
            PanelItems.Children.Add(new ControlUserProfile(this, _profileManager, profile));
        }
        private void BtnCreateAccount_Click(object sender, RoutedEventArgs e)
        {
            var profile = new UserProfile()
            {
                CreatedAt = DateTime.Now,
                Id = Guid.NewGuid().ToString(),
                IsSelected = false,
                Nickname = TextBoxAccountName.Text,
                Personalchest = null,
                PlayerStat = null,
                Profile = null
            };
            _profileManager.AddProfile(profile);
            ProfileManager.SaveProfile(profile);
            _profileManager.SaveChanges();
            AddItem(profile);
            ChangeView(GridMain);
        }
        public void ChangeView(Grid uIElement)
        {
            this.GridMain.Visibility = Visibility.Collapsed;
            this.GridAdd.Visibility = Visibility.Collapsed;
            this.GridEdit.Visibility = Visibility.Collapsed;

            switch (uIElement.Name)
            {
                case "GridAdd":
                    {

                    }
                    break;
                case "GridMain":
                    {
                       
                    }
                    break;
                case "GridEdit":
                    {
                      
                    }
                    break;
            }
            uIElement.Visibility = Visibility.Visible;

        }

        private void BtnCreateAccountBack_Click(object sender, RoutedEventArgs e)=> ChangeView(GridMain);

        private void BtnAdd_Click(object sender, RoutedEventArgs e) => ChangeView(GridAdd);
    }
}
