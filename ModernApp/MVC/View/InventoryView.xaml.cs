using ModernApp.MVC.Model;
using ModernApp.MVC.View.InventorySubviews;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using LiveCharts;
using LiveCharts.Wpf;
using System.Collections.Generic;
using System.ComponentModel;

namespace ModernApp.MVVM.View
{
    public partial class InventoryView : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<InventoryItem> AllInventoryItems { get; set; } = new ObservableCollection<InventoryItem>(); // Stores full data
        public ObservableCollection<InventoryItem> InventoryItems { get; set; } = new ObservableCollection<InventoryItem>(); // Stores filtered data

        private readonly DBconnection dbconnection;

        // Bar Chart Series Collection
        private SeriesCollection _barSeries;
        public SeriesCollection BarSeries
        {
            get { return _barSeries; }
            set { _barSeries = value; OnPropertyChanged(nameof(BarSeries)); }
        }

        // Pie Chart Series Collection
        private SeriesCollection _pieSeries;
        public SeriesCollection PieSeries
        {
            get { return _pieSeries; }
            set { _pieSeries = value; OnPropertyChanged(nameof(PieSeries)); }
        }

        // X Labels for Bar Chart
        public List<string> XLabels { get; set; } = new List<string>();

        public InventoryView()
        {
            InitializeComponent();
            dbconnection = new DBconnection();
            this.DataContext = this;
            LoadInventoryData(); // Load all data on startup
        }

        private void LoadInventoryData()
        {
            try
            {
                using (var connection = dbconnection.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT ItemName, ModelNumber, SKU, Quantity, Category, ItemPrice FROM inventory";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        AllInventoryItems.Clear(); // Load full inventory
                        InventoryItems.Clear();
                        var categoryCounts = new Dictionary<string, int>();

                        while (reader.Read())
                        {
                            var item = new InventoryItem
                            {
                                ItemName = reader["ItemName"]?.ToString() ?? "N/A",
                                SKU = reader["SKU"]?.ToString() ?? "N/A",
                                StockLevel = reader["Quantity"] != DBNull.Value ? Convert.ToInt32(reader["Quantity"]) : 0,
                                Category = reader["Category"]?.ToString() ?? "Uncategorized",
                                ModelNumber = reader["ModelNumber"]?.ToString() ?? "N/A",
                                Price = reader["ItemPrice"]?.ToString() ?? "0.00"
                            };

                            AllInventoryItems.Add(item);
                            InventoryItems.Add(item); // Initially show full data

                            // Count items per category
                            if (categoryCounts.ContainsKey(item.Category))
                                categoryCounts[item.Category] += 1;
                            else
                                categoryCounts[item.Category] = 1;
                        }

                        OnPropertyChanged(nameof(InventoryItems));
                        UpdateCharts(categoryCounts);
                        UpdateInventoryDashboard(); // Now uses AllInventoryItems
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading inventory: {ex.Message}");
            }
        }



        private void UpdateInventoryDashboard()
        {
            int totalItems = AllInventoryItems.Count; // Use full dataset
            int outOfStock = AllInventoryItems.Count(i => i.StockLevel == 0);
            int reordersNeeded = AllInventoryItems.Count(i => i.StockLevel < 5);
            int lowStock = AllInventoryItems.Count(i => i.StockLevel < 10 && i.StockLevel > 0);

            TotalItemsTextBlock.Text = totalItems.ToString();
            OutOfStockTextBlock.Text = outOfStock.ToString();
            ReorderNeededTextBlock.Text = reordersNeeded.ToString();
            LowStockTextBlock.Text = lowStock.ToString();
        }

        private void UpdateCharts(Dictionary<string, int> categoryCounts)
        {
            // Update on the UI thread if required
            Application.Current.Dispatcher.Invoke(() =>
            {
                BarSeries = new SeriesCollection
    {
        new ColumnSeries
        {
            Title = "Stock Levels",
            Values = new ChartValues<int>(InventoryItems.Select(i => i.StockLevel))
        }
    };

                PieSeries = new SeriesCollection();
                foreach (var category in categoryCounts)
                {
                    PieSeries.Add(new PieSeries
                    {
                        Title = category.Key,
                        Values = new ChartValues<int> { category.Value }
                    });
                }
            });

        }


        private void btnAddItemInventory_Click(object sender, RoutedEventArgs e)
        {
            NavigateTo(new InventryAddSubView());
        }

        private void btnInventoryDetails_Click(object sender, RoutedEventArgs e)
        {
            NavigateTo(new InventoryDetailedView());
        }

        private void btnUpdateInventory_Click(object sender, RoutedEventArgs e)
        {
            NavigateTo(new InventoryUpdateSubView());
        }

        private void NavigateTo(UserControl page)
        {
            try
            {
                fInventoryContainer?.Navigate(page);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating: {ex.Message}");
            }
        }

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        // Filtering method
        private void ApplyFilter(Func<InventoryItem, bool> filter = null)
        {
            InventoryItems.Clear();
            foreach (var item in filter == null ? AllInventoryItems : AllInventoryItems.Where(filter))
            {
                InventoryItems.Add(item);
            }
            OnPropertyChanged(nameof(InventoryItems));
        }

        // Click Events
        private void borderTotalItems_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ApplyFilter(null); // Show all items
            ScrollToLastItem();
        }

        private void borderOutOfStock_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ApplyFilter(i => i.StockLevel == 0);
            ScrollToLastItem();
        }

        private void borderReorderStock_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ApplyFilter(i => i.StockLevel < 5);
            ScrollToLastItem();
        }

        private void borderLowStock_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ApplyFilter(i => i.StockLevel < 10 && i.StockLevel > 0);
            ScrollToLastItem();
        }

        private void ScrollToLastItem()
        {
            // Ensure the DataGrid is not empty
            if (InventoryItems.Count > 0)
            {
                // Scroll to the last item in the DataGrid
                var lastItem = InventoryItems.Last();
                InventoryDataGrid.ScrollIntoView(lastItem);
            }
        }







    }

    public class InventoryItem
    {
        public string ItemName { get; set; }
        public string SKU { get; set; }
        public int StockLevel { get; set; }
        public string Category { get; set; }
        public string Price { get; set; }
        public string ModelNumber { get; set; }
        public double ItemPrice { get; internal set; }
    }
}
