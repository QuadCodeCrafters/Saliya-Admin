using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ModernApp.Core;
using ModernApp.Events;
using ModernApp.MVC.Model;
using MySql.Data.MySqlClient;

namespace ModernApp.MVC.View.ServicesSubviews
{
    public partial class AddServicesSubViews : UserControl
    {
       
        private readonly DBconnection dBconnection;
        public AddServicesSubViews()
        {
            InitializeComponent();
            dBconnection = new DBconnection();
            App.EventAggregator.GetEvent<ClearFormEvent>().Subscribe(ClearForm);
        }

        private void btnRegisterService_Click(object sender, RoutedEventArgs e)
        {
            string serviceName = txtServiceName.Text.Trim();
            string servicePriceText = txtServicePrice.Text.Trim();
            string selectedTable = cmbServiceType.Text;

            if (string.IsNullOrEmpty(serviceName) || string.IsNullOrEmpty(servicePriceText) || string.IsNullOrEmpty(selectedTable))
            {
                MessageBox.Show("Please fill all fields and select a service type.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(servicePriceText, out decimal servicePrice))
            {
                MessageBox.Show("Please enter a valid price.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string tableName = "";

            switch (selectedTable)
            {
                case "Paint Services":
                    tableName = "paintservices";
                    break;
                case "General Services":
                    tableName = "services";
                    break;
                case "Repairs":
                    tableName = "repairs";
                    break;
                default:
                    MessageBox.Show("Invalid service type selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
            }

            string query = $"INSERT INTO {tableName} (name, price) VALUES (@name, @price)";

            try
            {
                using (var connection = dBconnection.GetConnection())
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        // Add parameters before execution
                        cmd.Parameters.AddWithValue("@name", serviceName);
                        cmd.Parameters.AddWithValue("@price", servicePrice);

                        // ExecuteNonQuery is correct for INSERT operations
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Service added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearForm();


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void btnCancelServiceReg_Click(object sender, RoutedEventArgs e)
        {
            txtServiceName.Clear();
            txtServicePrice.Clear();
            cmbServiceType.SelectedIndex = -1;
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
        private void btnCancelServiceUpdate_Click(object sender, RoutedEventArgs e)
        {
            App.EventAggregator.GetEvent<DialogOpenEvent>().Publish("OpenCancelDialog");

        }

        public void ClearForm()
        {

            ClearHelper.ClearTextBoxesAndComboBoxes(this);
        }
    }
}
