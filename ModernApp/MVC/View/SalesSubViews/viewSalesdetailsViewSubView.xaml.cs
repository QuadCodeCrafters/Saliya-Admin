using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ModernApp.Core;
using ModernApp.MVC.Model;
using MySql.Data.MySqlClient;

namespace ModernApp.MVC.View.SalesSubViews
{
    /// <summary>
    /// Interaction logic for viewSalesdetailsViewSubView.xaml
    /// </summary>
    public partial class viewSalesdetailsViewSubView : UserControl
    {
        public ObservableCollection<InvoiceModel> Sales { get; set; } = new ObservableCollection<InvoiceModel>();
        private readonly DBconnection dBconnection;
        int totalSalesCount;
        public viewSalesdetailsViewSubView()
        {
            InitializeComponent();
            dBconnection = new DBconnection(); 
            LoadInvoiceData(); // Load data when the view is initialized
            CounttotalSales();
        }

        private void LoadInvoiceData()
        {
            try
            {
                using (var connection = dBconnection.GetConnection()) // Use existing DB connection
                {
                    connection.Open();
                    string query = @"
                SELECT i.InvoiceID, i.CustomerName, i.VehicleType, i.VehicleNumber, i.Date, i.TotalAmount, d.Description
                FROM Invoice i
                JOIN InvoiceDetails d ON i.InvoiceID = d.InvoiceID";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            Sales.Clear(); // Ensure it's not duplicated

                            while (reader.Read())
                            {
                                Sales.Add(new InvoiceModel
                                {
                                    InvoiceID = reader["InvoiceID"].ToString(),
                                    CustomerName = reader["CustomerName"].ToString(),
                                    VehicleModel = reader["VehicleType"].ToString(),
                                    VehicleNumber = reader["VehicleNumber"].ToString(),
                                    Date = Convert.ToDateTime(reader["Date"]),
                                    TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                    Description = reader["Description"].ToString()
                                });
                            }
                        }
                    }
                }

                InvoiceDataGrid.ItemsSource = Sales; // Bind DataGrid to the ObservableCollection
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading invoices: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void CounttotalSales()
        {
            totalSalesCount = InvoiceDataGrid.Items.Count;
            TotalSalesLabel.Content = $"Total Sales : {totalSalesCount}";
        }





        private void btnBackSalesDashboard_Click(object sender, RoutedEventArgs e)
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

        private void btnApplyFilters_Click(object sender, RoutedEventArgs e)
        {
            if (Sales == null || Sales.Count == 0)
            {
                MessageBox.Show("No data available to filter.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string vehicleNumberFilter = txtSearchVehicleNumber.Text?.Trim();
            string vehicleModelFilter = txtSearchVehicleModel.Text?.Trim();
            DateTime? registrationDateFilter = dpRegistrationDateFilter.SelectedDate;

            string selectedProductType = cbProductType.SelectedItem is ComboBoxItem selectedItem ? selectedItem.Content.ToString() : "All";
            decimal minPrice = 0, maxPrice = decimal.MaxValue;

            // Validate Min Price filter
            if (!string.IsNullOrEmpty(txtMinPrice.Text) && decimal.TryParse(txtMinPrice.Text, out decimal parsedMinPrice))
            {
                minPrice = parsedMinPrice;
            }

            // Validate Max Price filter
            if (!string.IsNullOrEmpty(txtMaxPrice.Text) && decimal.TryParse(txtMaxPrice.Text, out decimal parsedMaxPrice))
            {
                maxPrice = parsedMaxPrice;
            }

            // Apply filters
            var filteredData = Sales.Where(invoice =>
                // Filter by vehicle number
                (string.IsNullOrEmpty(vehicleNumberFilter) ||
                 (!string.IsNullOrEmpty(invoice.VehicleNumber) && invoice.VehicleNumber.IndexOf(vehicleNumberFilter, StringComparison.OrdinalIgnoreCase) >= 0)) &&

                // Filter by vehicle model
                (string.IsNullOrEmpty(vehicleModelFilter) ||
                 (!string.IsNullOrEmpty(invoice.VehicleModel) && invoice.VehicleModel.IndexOf(vehicleModelFilter, StringComparison.OrdinalIgnoreCase) >= 0)) &&

                // Registration date filter
                (!registrationDateFilter.HasValue || (invoice.Date.Date == registrationDateFilter.Value.Date)) &&

                // Product type filter (ignore if "All" is selected)
                (selectedProductType == "All" || invoice.Description.Equals(selectedProductType, StringComparison.OrdinalIgnoreCase)) &&

                // Price range filter
                (invoice.TotalAmount >= minPrice && invoice.TotalAmount <= maxPrice)
            ).ToList();

            // Update DataGrid
            InvoiceDataGrid.ItemsSource = filteredData;

            CounttotalSales();
        }


        private void btnClearFilters_Click(object sender, RoutedEventArgs e)
        {
            ClearHelper.ClearTextBoxesAndComboBoxes(this);
            DataContext = this;
            LoadInvoiceData();
            CounttotalSales();
        }
    }

    // Invoice Model Class
    public class InvoiceModel
    {
        public string InvoiceID { get; set; }
        public string CustomerName { get; set; }
        public string VehicleNumber { get; set; }
        public string VehicleModel { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
        public string Description { get; set; }
    }
}
