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

namespace ModernApp.MVVM.View.HelpSubViews
{
    /// <summary>
    /// Interaction logic for HelpInventory.xaml
    /// </summary>
    public partial class HelpInventory : UserControl
    {
        public HelpInventory()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Hide the current user control
            this.Visibility = Visibility.Collapsed;
        }
    }
}
