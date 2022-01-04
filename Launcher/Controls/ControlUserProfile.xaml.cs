using Dynastio.Api;
using Launcher.Extensions;
using Launcher.Models;
using Launcher.Pages;
using Launcher.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Launcher.Controls
{
    /// <summary>
    /// Interaction logic for ControlUserProfile.xaml
    /// </summary>
    public partial class ControlUserProfile : UserControl
    {
        public readonly UserProfile _userProfile;
        private readonly ProfileManager _profileManager;
        private readonly PageAccounts _parent;

        public ControlUserProfile(PageAccounts parent, ProfileManager profileManager, UserProfile userProfile)
        {
            this._parent = parent;
            this._profileManager = profileManager;
            _userProfile = userProfile;
            InitializeComponent();
            UpdateControlToProfileView();
        }
        public void UpdateControlToProfileView()
        {
            this.LabelTittle.Content = _userProfile.Nickname;
            this.LabelDescription.Text = "its a dynast.io user profile description";
            this.LabelDetails.Text = _userProfile.Details;
            if (_userProfile.IsSelected)
            {
                this.BtnAction.Content = "Modify";
                this.BtnAction.IsEnabled = false;
                this.BorderMain.BorderBrush = Brushes.Green;
            }
            else
            {
                this.BtnAction.Content = "Select as Main";
                this.BtnAction.IsEnabled = true;
                this.BorderMain.BorderBrush = Brushes.Orange;
            }
            if (_userProfile.Profile != null)
            {
                try
                {
                    SetBadges();
                    this.LabelCoin.Content = _userProfile.Profile.Coins;
                    this.LabelLevel.Content = _userProfile.Profile.Details.Level;
                }
                catch
                {
                    _userProfile.Profile = null;
                }
            }
            if (_userProfile.Personalchest != null)
            {
                try
                {
                    Pchest.SetPersonalChest(_userProfile.Personalchest);
                }
                catch
                {
                    _userProfile.Personalchest = null;
                }
            }
        }
        private void BtnSet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var data = TextBoxData.Text;
                TextBoxData.Text = "";
                var bdata = LauncherTransferDataModel.ToTransferDataModel(data);

                _userProfile.Profile = bdata.Profile;
                _userProfile.Personalchest = bdata.Personalchest;

                UpdateControlToProfileView();
                _profileManager.SaveChanges();
            }
            catch
            {
                TextBoxData.Text = $"incorrect data, make sure you got version {App.version}.";
            }
        }
        public void SetBadges()
        {
            this.PBadministrator.Source = new BitmapImage(new Uri(_userProfile.Profile.Badges.Where(a => a == BadgeType.Administrator).Any() ? "Images/badges/administrator.png".ResourcesPath() : "Images/_badges/administrator.png".ResourcesPath()));
            this.PBdeveloper.Source = new BitmapImage(new Uri(_userProfile.Profile.Badges.Where(a => a == BadgeType.Developer).Any() ? "Images/badges/developer.png".ResourcesPath() : "Images/_badges/developer.png".ResourcesPath()));
            this.PBfriend.Source = new BitmapImage(new Uri(_userProfile.Profile.Badges.Where(a => a == BadgeType.Friend).Any() ? "Images/badges/friend.png".ResourcesPath() : "Images/_badges/friend.png".ResourcesPath()));
            this.PBmonthly.Source = new BitmapImage(new Uri(_userProfile.Profile.Badges.Where(a => a == BadgeType.Monthly).Any() ? "Images/badges/monthly.png".ResourcesPath() : "Images/_badges/monthly.png".ResourcesPath()));
            this.PBsupporter.Source = new BitmapImage(new Uri(_userProfile.Profile.Badges.Where(a => a == BadgeType.Supporter).Any() ? "Images/badges/supporter.png".ResourcesPath() : "Images/_badges/supporter.png".ResourcesPath()));
            this.PBtranslator.Source = new BitmapImage(new Uri(_userProfile.Profile.Badges.Where(a => a == BadgeType.Translator).Any() ? "Images/badges/translator.png".ResourcesPath() : "Images/_badges/translator.png".ResourcesPath()));
            this.PByoutuber.Source = new BitmapImage(new Uri(_userProfile.Profile.Badges.Where(a => a == BadgeType.Youtuber).Any() ? "Images/badges/youtuber.png".ResourcesPath() : "Images/_badges/youtuber.png".ResourcesPath()));
        }

        private void BtnAction_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Content)
            {
                case "Modify": break;
                case "Select as Main": SetProfileAsMain(); break;
            }
        }
        void SetProfileAsMain()
        {
            _profileManager.UnselectAllProfiles();
            _userProfile.IsSelected = true;
            _profileManager.SaveChanges();

            _parent.UpdateControlsView();
            ProfileManager.SetProfile(_userProfile);
        }
        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            _profileManager.RemoveProfile(_userProfile);
            _profileManager.SaveChanges();
            _parent.RemoveItem(this);
        }


    }
}
