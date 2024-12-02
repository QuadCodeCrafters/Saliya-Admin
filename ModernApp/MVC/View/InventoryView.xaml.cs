using ModernApp.MVC.View.InventorySubviews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public InventoryView()
        {
            InitializeComponent();
            // Bind sample data
            InventoryDataGrid.ItemsSource = GetInventoryItems();
        }

        // Create sample dataset
        private ObservableCollection<InventoryItem> GetInventoryItems()
        {
            return new ObservableCollection<InventoryItem>
            {
                new InventoryItem { ItemName = "Engine Oil", SKU = "ENG123", StockLevel = 50, Supplier = "ABC Supplies", Category = "Auto Parts", ReorderLevel = 10, Price = "$25" },
                new InventoryItem { ItemName = "Brake Pads", SKU = "BRK456", StockLevel = 30, Supplier = "XYZ Motors", Category = "Auto Parts", ReorderLevel = 5, Price = "$15" },
                new InventoryItem { ItemName = "Tires", SKU = "TIR789", StockLevel = 20, Supplier = "TireCo", Category = "Auto Parts", ReorderLevel = 8, Price = "$50" }
            };
        }

       

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // Ensure fInventoryContainer is a Frame control
                if (fInventoryContainer != null)
                {
                    // Navigate to the InventoryDetailedView UserControl
                    fInventoryContainer.Navigate(new InventoryDetailedView());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }
        }
    }

    public class InventoryItem
    {
        public string ItemName { get; set; }
        public string SKU { get; set; }
        public int StockLevel { get; set; }
        public string Supplier { get; set; }
        public string Category { get; set; }
        public int ReorderLevel { get; set; }
        public string Price { get; set; }
    }
}
