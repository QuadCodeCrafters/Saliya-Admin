using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ModernApp.MVC.Model;
using System.Linq;

namespace ModernApp.MVC.View.InventorySubviews
{
    public partial class InventoryDetailedView : UserControl
    {
        public ObservableCollection<InventoryItem> InventoryItems { get; set; }
        private readonly DBconnection dbConnection;
        public InventoryDetailedView()
        {
            InitializeComponent();
            InventoryItems = new ObservableCollection<InventoryItem>();
            dbConnection = new DBconnection();
            LoadInventoryData();
            DataContext = this;  // Bind DataGrid to this class
        }

        // Load data from the MySQL database
        private void LoadInventoryData()
        {


            try
            {
                
                using (var connection = dbConnection.GetConnection())
                {
                    connection.Open();
                    string query = @"
                SELECT ID, Availability, ItemName, SKU, ItemPrice, Category, 
                       Description, Manufacturer, ModelNumber, Warranty, 
                       StorageLocation, PicLocation 
                FROM inventory";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        InventoryItems.Clear();
                        while (reader.Read())
                        {
                            InventoryItems.Add(new InventoryItem
                            {
                                ID = reader.GetInt32("ID"),
                                Availability = reader.GetString("Availability"),
                                ItemName = reader.GetString("ItemName"),
                                SKU = reader.GetString("SKU"),
                                ItemPrice = reader.GetDouble("ItemPrice"),
                                Category = reader.GetString("Category"),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString("Description"),
                                Manufacturer = reader.IsDBNull(reader.GetOrdinal("Manufacturer")) ? null : reader.GetString("Manufacturer"),
                                ModelNumber = reader.IsDBNull(reader.GetOrdinal("ModelNumber")) ? null : reader.GetString("ModelNumber"),
                                Warranty = reader.IsDBNull(reader.GetOrdinal("Warranty")) ? null : reader.GetString("Warranty"),
                                StorageLocation = reader.IsDBNull(reader.GetOrdinal("StorageLocation")) ? null : reader.GetString("StorageLocation"),
                                PicLocation = reader.IsDBNull(reader.GetOrdinal("PicLocation")) ? null : reader.GetString("PicLocation")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading inventory: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Refresh Button Click - Reloads the data
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadInventoryData();
        }

        // Add Item Button - Opens an Add Item dialog (To be implemented)
        private void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Open the Add Item dialog here!");
        }

        // Edit an inventory item
        private void EditItem(InventoryItem item)
        {
            MessageBox.Show($"Edit Item: {item.ItemName}");
        }

        // Delete an inventory item
        private void DeleteItem(InventoryItem item)
        {
            InventoryItems.Remove(item);
        }

        private void btnBackInventoryDashboard_Click(object sender, RoutedEventArgs e)
        {
            var parentFrame = FindParent<Frame>(this); // Find the parent Frame
            if (parentFrame != null)
            {
                parentFrame.Content = null; // Remove the UserControl (close it)
            }
        }

        // Helper function to find the parent Frame of the UserControl
        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null) return null;

            if (parentObject is T parent)
            {
                return parent;
            }
            else
            {
                return FindParent<T>(parentObject);
            }
        }


        private void btnClear_Click_1(object sender, RoutedEventArgs e)
        {
            // Clear the filter inputs
            ItemNameTextBox.Clear();
            MinPriceTextBox.Clear();
            MaxPriceTextBox.Clear();
            CategoryComboBox.SelectedIndex = -1;

            // Reload the inventory data and reset the filter
            LoadInventoryData();

            // Ensure the DataGrid is bound to the updated inventory data
            InventoryDataGrid.ItemsSource = InventoryItems;
        }


        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            // Get the filter values from the UI controls
            string itemNameFilter = ItemNameTextBox.Text?.Trim();
            var selectedCategoryItem = CategoryComboBox.SelectedItem as ComboBoxItem;
            string selectedCategory = selectedCategoryItem?.Content.ToString();
            decimal? minPrice = string.IsNullOrEmpty(MinPriceTextBox.Text) ? (decimal?)null : decimal.Parse(MinPriceTextBox.Text);
            decimal? maxPrice = string.IsNullOrEmpty(MaxPriceTextBox.Text) ? (decimal?)null : decimal.Parse(MaxPriceTextBox.Text);

            // Filter the inventory data
            var filteredData = InventoryItems.Where(item =>
                // Filter by item name
                (string.IsNullOrEmpty(itemNameFilter) ||
                (!string.IsNullOrEmpty(item.ItemName) && item.ItemName.IndexOf(itemNameFilter, StringComparison.OrdinalIgnoreCase) >= 0)) &&

                // Filter by category
                (string.IsNullOrEmpty(selectedCategory) || item.Category == selectedCategory) &&

        // Filter by price range - Convert double to decimal for comparison
        (minPrice.HasValue ? (decimal)item.ItemPrice >= minPrice.Value : true) &&
        (maxPrice.HasValue ? (decimal)item.ItemPrice <= maxPrice.Value : true)
    

            )
            .ToList();

            // Update the DataGrid view without modifying the original collection
            InventoryDataGrid.ItemsSource = filteredData;

            // Update the total count label
            int totalItemCount = filteredData.Count();
            TotalItemslabel.Content = $"Total items: {totalItemCount}";
        }

    }

    // Inventory Item Model
    public class InventoryItem
    {
        public int ID { get; set; }
        public string Availability { get; set; }  // New field
        public string ItemName { get; set; }
        public string SKU { get; set; }  // New field
        public double ItemPrice { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }  // New field
        public string Manufacturer { get; set; }  // New field
        public string ModelNumber { get; set; }  // New field
        public string Warranty { get; set; }  // New field
        public string StorageLocation { get; set; }  // New field
        public string PicLocation { get; set; }  // New field
    }

}
