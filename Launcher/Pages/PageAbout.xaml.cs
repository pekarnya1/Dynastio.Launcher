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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Launcher.Pages
{
    /// <summary>
    /// Interaction logic for PageAbout.xaml
    /// </summary>
    public partial class PageAbout : Page
    {
        private readonly Configuration _configuration;
        public PageAbout(Configuration configuration)
        {
            _configuration = configuration;
            InitializeComponent();
        }


        private void BtnDiscord_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(_configuration.DiscorUrl);
        }

        private void BtnSite_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(_configuration.SiteUrl);
        }

        private void BtnSource_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(_configuration.SourceUrl);
        }
    }
}
