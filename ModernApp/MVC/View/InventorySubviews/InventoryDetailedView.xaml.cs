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


namespace ModernApp.MVC.View.InventorySubviews
{
    public partial class InventoryDetailedView : UserControl
    {
        public ObservableCollection<InventoryItem> InventoryItems { get; set; }

        public InventoryDetailedView()
        {
            InitializeComponent();
            // Initialize sample data
            InventoryItems = new ObservableCollection<InventoryItem>
            {
                new InventoryItem { ID = 1, ItemName = "iPhone 15", Category = "Electronics", Price = 999, Quantity = 50 },
                new InventoryItem { ID = 2, ItemName = "MacBook Pro", Category = "Electronics", Price = 2399, Quantity = 30 },
            };

            // Bind the DataGrid to InventoryItems collection
            InventoryDataGrid.ItemsSource = InventoryItems;
        }

        // Event handler for adding new item (opens a form or dialog in a real app)
        private void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Open the Add Item dialog here!");
        }

        // Command to Edit an inventory item (e.g., opens an Edit form)
        private void EditItem(InventoryItem item)
        {
            MessageBox.Show($"Edit Item: {item.ItemName}");
        }

        // Command to Delete an inventory item
        private void DeleteItem(InventoryItem item)
        {
            InventoryItems.Remove(item);
        }
    }

    // Sample inventory item model
    public class InventoryItem
    {
        public int ID { get; set; }
        public string ItemName { get; set; }
        public string Category { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }
}
