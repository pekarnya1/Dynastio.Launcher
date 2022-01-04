using Dynastio.Api;
using Launcher.Extensions;
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
    /// Interaction logic for PersonalChestControl.xaml
    /// </summary>
    public partial class PersonalChestControl : UserControl
    {
        public PersonalChestControl()
        {
            InitializeComponent();
        }
        Personalchest pchet;
        public Personalchest Pchest
        {
            get
            {
                return pchet;
            }
            set
            {
                pchet = value;
                UpdateView();
            }
        }
        public void SetPersonalChest(Personalchest pchest)
        {
            Pchest = pchest;
        }
        void UpdateView()
        {
            foreach (var item in Pchest.items)
            {
                var TargetElement = (Image)this.FindName($"ImagePchestSlot{(item.index + 1)}");
                var imageUrl = $"Images/Inventory/{item.ItemType}.png".ResourcesPath();
                var image = File.Exists(imageUrl) ? imageUrl : $"Images/Inventory/unknown.png".ResourcesPath();
                TargetElement.Source = new BitmapImage(new Uri(image));
            }
        }

    }
}
