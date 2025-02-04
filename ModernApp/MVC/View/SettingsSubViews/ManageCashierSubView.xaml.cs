using ModernApp.MVC.Model;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernApp.MVC.View.SettingsSubViews
{
    public partial class ManageCashierSubView : UserControl
    {
        private readonly DBconnection dBconnection;
        

        public ManageCashierSubView()
        {
            InitializeComponent();
            dBconnection = new DBconnection();
            LoadCashiers();
        }

        // Load all Cashiers
        private void LoadCashiers()
        {
            try
            {
                using (var connection = dBconnection.GetConnection()) // Use existing DB connection
                {
                    connection.Open();

                    string query = @"
                SELECT c.EmployeeID, c.Username, c.PasswordHash, e.Name 
                FROM Cashier c 
                JOIN Employee e ON c.EmployeeID = e.EmployeeID";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    using (MySqlDataAdapter da = new MySqlDataAdapter(command))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        CashierGrid.ItemsSource = dt.DefaultView;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading cashiers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Search Cashier by Name
        private void SearchCashier(object sender, RoutedEventArgs e)
        {
            try
            {
                string searchText = txtSearch.Text.Trim();

                using (var connection = dBconnection.GetConnection()) // Use existing DB connection
                {
                    connection.Open();

                    string query = @"
                SELECT c.EmployeeID, c.Username, c.PasswordHash, e.Name 
                FROM Cashier c 
                JOIN Employee e ON c.EmployeeID = e.EmployeeID 
                WHERE e.Name LIKE @name";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", $"%{searchText}%");

                        using (MySqlDataAdapter da = new MySqlDataAdapter(command))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            CashierGrid.ItemsSource = dt.DefaultView;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching cashier: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Delete Cashier
        private void DeleteCashier(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CashierGrid.SelectedItem is DataRowView row)
                {
                    int employeeID = Convert.ToInt32(row["EmployeeID"]);

                    MessageBoxResult result = MessageBox.Show(
                        "Are you sure you want to delete this cashier?", "Confirm Delete",
                        MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        using (var connection = dBconnection.GetConnection()) // Use existing DB connection
                        {
                            connection.Open();

                            string query = "DELETE FROM Cashier WHERE EmployeeID = @id";

                            using (MySqlCommand cmd = new MySqlCommand(query, connection))
                            {
                                cmd.Parameters.AddWithValue("@id", employeeID);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        LoadCashiers(); // Refresh the grid after deletion
                    }
                }
                else
                {
                    MessageBox.Show("Please select a cashier to delete.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting cashier: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnBackSettingsDashboard_Click(object sender, RoutedEventArgs e)
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
