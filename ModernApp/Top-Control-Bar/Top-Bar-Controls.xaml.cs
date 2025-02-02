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

namespace ModernApp.Top_Control_Bar
{
    /// <summary>
    /// Interaction logic for Top_Bar_Controls.xaml
    /// </summary>
    public partial class Top_Bar_Controls : UserControl
    {
        public Top_Bar_Controls()
        {
            InitializeComponent();

            
            NotificationButton.Click += (s, e) => NotificationPopup.IsOpen = !NotificationPopup.IsOpen;
            ProfileButton.Click += (s, e) => ProfilePopup.IsOpen = !ProfilePopup.IsOpen;
        }


        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            // Action for Search
            MessageBox.Show("Search clicked!");
        }

        private void DarkModeButton_Click(object sender, RoutedEventArgs e)
        {
            // Action for Dark Mode
            MessageBox.Show("Dark mode clicked!");
        }

        private void NotificationButton_Click(object sender, RoutedEventArgs e)
        {
            // Toggle notification popup
            NotificationPopup.IsOpen = !NotificationPopup.IsOpen;
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            // Toggle profile popup
            ProfilePopup.IsOpen = !ProfilePopup.IsOpen;
        }
    }
}
