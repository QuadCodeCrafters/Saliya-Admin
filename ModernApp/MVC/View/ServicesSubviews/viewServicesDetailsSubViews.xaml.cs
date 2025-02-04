using ModernApp.Core;
using ModernApp.MVC.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernApp.MVC.View.ServicesSubviews
{
    /// <summary>
    /// Interaction logic for viewServicesDetailsSubViews.xaml
    /// </summary>
    public partial class viewServicesDetailsSubViews : UserControl
    {
        private readonly DBconnection dbconnection;
        public viewServicesDetailsSubViews()
        {
            InitializeComponent();
            dbconnection = new DBconnection();
            LoadData(); // Call the method to populate grids when the view loads
        }

        private void LoadData()
        {
            LoadPaintServices();
            LoadVehicleServices();
            LoadRepairs();
        }

        private void LoadPaintServices()
        {
            try
            {
                using (var connection = dbconnection.GetConnection()) // Ensure dbconnection is configured for MySQL
                {
                    connection.Open();

                    string query = "SELECT ID, Name, Price FROM paintservices";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        PaintServicesGrid.ItemsSource = dataTable.DefaultView;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading paint services: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadVehicleServices()
        {
            try
            {
                using (var connection = dbconnection.GetConnection()) // Ensure it returns MySqlConnection
                {
                    connection.Open();
                    string query = "SELECT ID, Name, Price FROM services";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        VehicleServicesGrid.ItemsSource = dataTable.DefaultView;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading vehicle services: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadRepairs()
        {
            try
            {
                using (var connection = dbconnection.GetConnection()) // Ensure it returns MySqlConnection
                {
                    connection.Open();
                    string query = "SELECT ID, Name, Price FROM repairs";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        RepairsGrid.ItemsSource = dataTable.DefaultView;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading repairs: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearHelper.ClearTextBoxesAndComboBoxes(this);
            LoadData();
        }



        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get filter values from UI controls
                string itemNameFilter = ItemNameTextBox.Text?.Trim();
                decimal? minPrice = string.IsNullOrEmpty(MinPriceTextBox.Text) ? (decimal?)null : decimal.Parse(MinPriceTextBox.Text);
                decimal? maxPrice = string.IsNullOrEmpty(MaxPriceTextBox.Text) ? (decimal?)null : decimal.Parse(MaxPriceTextBox.Text);

                // Apply filters to each data grid separately
                ApplyFilterToDataGrid(PaintServicesGrid, "paintservices", itemNameFilter, minPrice, maxPrice);
                ApplyFilterToDataGrid(VehicleServicesGrid, "services", itemNameFilter, minPrice, maxPrice);
                ApplyFilterToDataGrid(RepairsGrid, "repairs", itemNameFilter, minPrice, maxPrice);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error applying filters: " + ex.Message, "Filter Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Method to apply filters dynamically to any DataGrid
        private void ApplyFilterToDataGrid(DataGrid dataGrid, string tableName, string itemNameFilter, decimal? minPrice, decimal? maxPrice)
        {
            try
            {
                using (var connection = dbconnection.GetConnection()) // Ensure dbconnection is configured for MySQL
                {
                    connection.Open();

                    // Construct the query dynamically with filtering
                    string query = $"SELECT ID, Name, Price FROM {tableName} WHERE 1=1"; // 1=1 allows easy concatenation

                    if (!string.IsNullOrEmpty(itemNameFilter))
                        query += " AND Name LIKE @ItemName";

                    if (minPrice.HasValue)
                        query += " AND Price >= @MinPrice";

                    if (maxPrice.HasValue)
                        query += " AND Price <= @MaxPrice";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        if (!string.IsNullOrEmpty(itemNameFilter))
                            command.Parameters.AddWithValue("@ItemName", $"%{itemNameFilter}%");

                        if (minPrice.HasValue)
                            command.Parameters.AddWithValue("@MinPrice", minPrice.Value);

                        if (maxPrice.HasValue)
                            command.Parameters.AddWithValue("@MaxPrice", maxPrice.Value);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            DataTable dataTable = new DataTable();
                            dataTable.Load(reader);
                            dataGrid.ItemsSource = dataTable.DefaultView;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error filtering {tableName}: {ex.Message}", "Filter Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
