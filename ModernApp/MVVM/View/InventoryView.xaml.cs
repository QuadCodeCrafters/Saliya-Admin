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

namespace ModernApp.MVVM.View
{
    /// <summary>
    /// Interaction logic for InventoryView.xaml
    /// </summary>
    public partial class InventoryView : UserControl
    {
        private readonly Page InventoryAddItemView; // Store the page instance
        public InventoryView()
        {
            InitializeComponent();
            InventoryAddItemView = new inventoryAddItemView();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Navigate to the inventoryAddItemView.xaml without losing state on subsequent loads
                fContainer.Navigate(new Uri("pack://application:,,,/MVVM/View/inventoryAddItemView.xaml", UriKind.Absolute));
                fContainer.Visibility = Visibility.Visible; // Ensure the frame is visible when navigating
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }
        }

        private void Inventory_Click(object sender, RoutedEventArgs e)
        {
            // Hide the frame, retaining its state
            fContainer.Visibility = Visibility.Collapsed;
        }

        private void ShowFrame_Click(object sender, RoutedEventArgs e)
        {
            // Show the frame again with the same content state
            fContainer.Visibility = Visibility.Visible;
        }

        private void fContainer_Navigated(object sender, NavigationEventArgs e)
        {

        }
    }
}

