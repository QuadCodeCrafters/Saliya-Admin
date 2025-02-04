using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Windows.Controls;
using ModernApp.MVC.Model;
using ModernApp.Core;
using Prism.Events;
using ModernApp.Events;
using System.Windows.Media;
namespace ModernApp.MVC.View.ServicesSubviews
{
    public partial class UpdateServicesSubviews : UserControl
    {
        private int selectedServiceId = -1; // To store the selected service ID
        private readonly DBconnection dBconnection;
        public UpdateServicesSubviews()
        {
            InitializeComponent();
            dBconnection = new DBconnection();
            App.EventAggregator.GetEvent<ClearFormEvent>().Subscribe(ClearForm);
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            string serviceName = ServiceNameTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(serviceName))
            {
                FetchServiceDetails(serviceName);
            }
            else
            {
                MessageBox.Show("Please enter a service name to search.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void FetchServiceDetails(string serviceName)
        {
            string query = @"SELECT id, name, price FROM (
                SELECT id, name, price FROM paintservices 
                UNION ALL 
                SELECT id, name, price FROM services 
                UNION ALL 
                SELECT id, name, price FROM repairs
            ) AS combined WHERE name = @name LIMIT 1;";

            using (var connection = dBconnection.GetConnection())
            {
                try
                {
                    connection.Open();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", serviceName);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                selectedServiceId = reader.GetInt32("id");
                                txtItemPrice.Text = reader.GetDecimal("price").ToString("F2");
                            }
                            else
                            {
                                MessageBox.Show("Service not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnUpdateService_Click(object sender, RoutedEventArgs e)
        {
            if (selectedServiceId == -1)
            {
                MessageBox.Show("No service selected for update.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtItemPrice.Text.Trim(), out decimal newPrice))
            {
                MessageBox.Show("Please enter a valid price.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string query = @"UPDATE paintservices SET price = @price WHERE id = @id;
                             UPDATE services SET price = @price WHERE id = @id;
                             UPDATE repairs SET price = @price WHERE id = @id;";

            using (var connection = dBconnection.GetConnection())
            {
                try
                {
                    connection.Open();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@price", newPrice);
                        command.Parameters.AddWithValue("@id", selectedServiceId);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Service price updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            ClearForm();
                        }
                        else
                        {
                            MessageBox.Show("Failed to update service price.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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

        private void btnBackServicesDashboard_Click(object sender, RoutedEventArgs e)
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
        }
}