using ModernApp.Core;
using ModernApp.MVC.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernApp.MVC.View.SalesSubViews
{
    public partial class CustomerDetailsViewSubView : UserControl
    {
        public ObservableCollection<Customer> Customers { get; set; } = new ObservableCollection<Customer>();
        private readonly DBconnection dBconnection;
        int totalcustomersCount;
        public CustomerDetailsViewSubView()
        {
            InitializeComponent();
            dBconnection = new DBconnection();
            DataContext = this;
            LoadCustomers();
            CounttotalCustomers();
        }

        private void LoadCustomers()
        {
            Customers.Clear();

            using (var connection = dBconnection.GetConnection()) // Use existing DB connection
            {
                connection.Open();
                string query = "SELECT * FROM vehicleregister";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Customers.Add(new Customer
                            {
                                ID = reader.GetInt32("ID"),
                                VehicleNumber = reader.GetString("VehicleNumber"),
                                VehicleCategory = reader.GetString("VehicleCategory"),
                                VehicleModel = reader.GetString("VehicleModel"),
                                VehicleType = reader.GetString("VehicleType"),
                                CustomerName = reader.GetString("CustomerName"),
                                CustomerAddress = reader.IsDBNull(reader.GetOrdinal("CustomerAddress")) ? null : reader.GetString("CustomerAddress"),
                                CustomerNIC = reader.GetString("CustomerNIC"),
                                CustomerEmail = reader.IsDBNull(reader.GetOrdinal("CustomerEmail")) ? null : reader.GetString("CustomerEmail"),
                                CustomerPhone = reader.GetString("CustomerPhone"),
                                SpecialNotes = reader.IsDBNull(reader.GetOrdinal("SpecialNotes")) ? null : reader.GetString("SpecialNotes"),
                                RegistrationDate = reader.GetDateTime("RegistrationDate")
                            });
                        }
                    }
                }
            }

            CustomerDataGrid.ItemsSource = Customers; // Bind data to DataGrid
        }

        public void CounttotalCustomers()
        {
            totalcustomersCount = CustomerDataGrid.Items.Count;
            TotalCustomersLabel.Content = $"Employee no: {totalcustomersCount}";
        }



        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void btnApplyFilters_Click(object sender, RoutedEventArgs e)
        {
            // Get the filter values from the UI controls
            string vehicleNumberFilter = txtSearchVehicleNumber.Text?.Trim();
            string vehicleModelFilter = txtSearchVehicleModel.Text?.Trim();
            DateTime? registrationDateFilter = dpRegistrationDateFilter.SelectedDate;

            // Filter the vehicle data
            var filteredData = Customers.Where(vehicle =>
                // Filter by vehicle number
                (string.IsNullOrEmpty(vehicleNumberFilter) ||
                 (!string.IsNullOrEmpty(vehicle.VehicleNumber) && vehicle.VehicleNumber.IndexOf(vehicleNumberFilter, StringComparison.OrdinalIgnoreCase) >= 0)) &&

                // Filter by vehicle model
                (string.IsNullOrEmpty(vehicleModelFilter) ||
                 (!string.IsNullOrEmpty(vehicle.VehicleModel) && vehicle.VehicleModel.IndexOf(vehicleModelFilter, StringComparison.OrdinalIgnoreCase) >= 0)) &&

                // Filter by registration date
                (!registrationDateFilter.HasValue ||
                 (vehicle.RegistrationDate.Date == registrationDateFilter.Value.Date))
            )
            .ToList();

            // Update the DataGrid view without modifying the original collection
            CustomerDataGrid.ItemsSource = filteredData;

            CounttotalCustomers();

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

        private void btnClearFilters_Click(object sender, RoutedEventArgs e)
        {
            ClearHelper.ClearTextBoxesAndComboBoxes(this);
            DataContext = this;
            LoadCustomers();
        }

        
    }

    public class Customer
    {
        public int ID { get; set; }  // Primary Key

        public string VehicleNumber { get; set; }
        public string VehicleCategory { get; set; }
        public string VehicleModel { get; set; }
        public string VehicleType { get; set; }

        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }  // Nullable
        public string CustomerNIC { get; set; }
        public string CustomerEmail { get; set; }  // Nullable
        public string CustomerPhone { get; set; }
        public string SpecialNotes { get; set; }  // Nullable

        public DateTime RegistrationDate { get; set; }
    }

}
