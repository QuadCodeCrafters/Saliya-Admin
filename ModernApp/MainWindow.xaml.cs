using System;
using System.Collections.Generic;
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
using System.Windows.Media.Animation;
using LiveCharts.Wpf;
using LiveCharts;
using System.Windows.Markup;
using ModernApp.MVVM.Model;
using ModernApp.MVC.View.EmployeeSubviews;
using ModernApp.MVVM.View;
using ModernApp.MVC.View.InventorySubviews;
using MaterialDesignThemes.Wpf;
using Prism.Events;
using ModernApp.Events;
using MySql.Data.MySqlClient;
using Windows.Security.Credentials.UI;
using ModernApp.MVC.Model;
using System.Security.Cryptography;
using ModernApp.MVC.View;
using ModernApp.MVC.Controller;

namespace ModernApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isAuthenticated = false;
        private readonly DBconnection dbConnection;
        private readonly DashboardView dashboardView;
        public MainWindow()
        {
            InitializeComponent();
            fContainer.Navigate(new Uri("pack://application:,,,/MVC/View/DashboardView.xaml", UriKind.Absolute));
            fTopBoxContainer.Navigate(new Uri("pack://application:,,,/Top-Control-Bar/Top-Bar-Controls.xaml", UriKind.Absolute));
            // Subscribe to DialogOpenEvent
            App.EventAggregator.GetEvent<DialogOpenEvent>().Subscribe(OpenDialog);
            App.EventAggregator.GetEvent<ChangeAdminPasswordEvent>().Subscribe(OpenPasswordChanegDialog);
            App.EventAggregator.GetEvent<logoutConfirmationEvent>().Subscribe(OpenlogoutConfirmDialog);
            dbConnection = new DBconnection();
            dashboardView = new DashboardView();
        }

        private async void CheckPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string oldPassword = txtOldpassword.Password.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(oldPassword))
                return;

            // Hash the entered old password
            string hashedOldPassword = txtOldpassword.Password.Trim();

            try
            {
                using (var connection = dbConnection.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM admin_credentials WHERE username = @username AND password_hash = @passwordHash";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@passwordHash", hashedOldPassword);

                        // Execute the query and check if the username and hashed password match
                        int count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                        if (count > 0)
                        {
                            txtNewpassword.IsEnabled = true;
                            txtNewConfirmpassword.IsEnabled = true;
                            ChangePasswordButton.IsEnabled = true;
                            MessageBox.Show("Old password is correct! You can now enter a new password.");
                        }
                        else
                        {
                            txtNewpassword.IsEnabled = false;
                            txtNewConfirmpassword.IsEnabled = false;
                            MessageBox.Show("Incorrect old password!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }
        }


        private async void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string oldPassword = txtOldpassword.Password.Trim();
            string newPassword = txtNewpassword.Password.Trim();
            string confirmPassword = txtNewConfirmpassword.Password.Trim();

            if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("New password fields cannot be empty!");
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("New passwords do not match!");
                return;
            }

            // Authenticate with Windows Hello before proceeding
            if (!isAuthenticated)
            {
                var result = await AuthenticateWithWindowsHello();
                if (!result)
                {
                    MessageBox.Show("Windows Hello Authentication Failed!");
                    return;
                }
            }

            // If authentication is successful, proceed with password update
            bool isUpdated = await UpdatePasswordAsync(username, newPassword);
            if (isUpdated)
            {
               
                
                txtNewpassword.Password = "";
                txtNewConfirmpassword.Password = "";
                txtOldpassword.Password = "";
                txtUsername.Text = "";
                txtNewpassword.IsEnabled = false;
                txtNewConfirmpassword.IsEnabled = false;
                ChangePasswordButton.IsEnabled = false;
                MessageBox.Show("Password changed successfully!");
            }
            else
            {
                MessageBox.Show("Failed to update password!");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            txtNewpassword.Password = "";
            txtNewConfirmpassword.Password = "";
            txtOldpassword.Password = "";
            txtUsername.Text = "";
            txtNewpassword.IsEnabled = false;
            txtNewConfirmpassword.IsEnabled = false;
            ChangePasswordButton.IsEnabled = false;
        }

        private async Task<bool> AuthenticateWithWindowsHello()
        {
            var availability = await UserConsentVerifier.CheckAvailabilityAsync();
            if (availability == UserConsentVerifierAvailability.Available)
            {
                var result = await UserConsentVerifier.RequestVerificationAsync("Confirm your identity to proceed.");
                return result == UserConsentVerificationResult.Verified;
            }
            return false;
        }

        private async Task<bool> ValidateOldPasswordAsync(string username, string oldPassword)
        {
            try
            {
                using (var connection = dbConnection.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT password_hash FROM admin_credentials WHERE username = @username";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        object result = await cmd.ExecuteScalarAsync();
                        if (result != null)
                        {
                            string storedHash = result.ToString();
                            return VerifyPassword(oldPassword, storedHash);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }
            return false;
        }

        private async Task<bool> UpdatePasswordAsync(string username, string newPassword)
        {
            try
            {
                string hashedPassword = newPassword;
                using (var connection = dbConnection.GetConnection())
                {
                    connection.Open();
                    string query = "UPDATE admin_credentials SET password_hash = @newPassword WHERE username = @username";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@newPassword", hashedPassword);
                        cmd.Parameters.AddWithValue("@username", username);
                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }
            return false;
        }

        private string HashPassword(string password) =>
            Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password)));

        private bool VerifyPassword(string enteredPassword, string storedHash) =>
            HashPassword(enteredPassword) == storedHash;
    

    private void OpenDialog(string message)
        {
            if (message == "OpenCancelDialog")
            {
                // Find the DialogHost in MainWindow
                if (CancelDialogHost != null)
                {
                    CancelDialogHost.IsOpen = true;
                }
                else
                {
                    MessageBox.Show("DialogHost not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OpenPasswordChanegDialog(string message)
        {
            if (message == "OpenChangePasswordDialog")
            {
                // Find the DialogHost in MainWindow
                if (ChangePasswordDialog != null)
                {
                    ChangePasswordDialog.IsOpen = true;
                }
                else
                {
                    MessageBox.Show("DialogHost not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OpenlogoutConfirmDialog(string message)
        {
            if (message == "OpenlogoutConfirmDialog")
            {
                // Find the DialogHost in MainWindow
                if (logoutcallDialog != null)
                {
                    logoutcallDialog.IsOpen = true;
                }
                else
                {
                    MessageBox.Show("DialogHost not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }



        private void ConfirmCancel_Click(object sender, RoutedEventArgs e)
        {
            // Publish event to clear the form
            App.EventAggregator.GetEvent<ClearFormEvent>().Publish();

            CloseDialog(); // Close the dialog
        }

        private void CloseDialog_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog();
        }

        private void CloseDialog()
        {
            if (FindName("CancelDialogHost") is DialogHost dialogHost)
            {
                dialogHost.IsOpen = false;
            }
        }


        private void closeApp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void minimizeApp(object sender, MouseButtonEventArgs e)
        {

            try
            {
                this.WindowState = WindowState.Minimized;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
           
        }   

       

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            try
            {
                // Make sure the URI points to the compiled XAML file location
                fContainer.Navigate(new System.Uri("pack://application:,,,/MVC/View/HomeView.xaml", UriKind.Absolute));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }

        }

        private void RadioButton_Checked_2(object sender, RoutedEventArgs e)
        {
            try
            {
                if (fContainer != null)
                {
                    fContainer.Navigate(new Uri("pack://application:,,,/MVC/View/DiscoveryView.xaml", UriKind.Absolute));
                }
                else
                {
                    MessageBox.Show("Frame container is not initialized.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }
        }

        private void RDBSales_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (fContainer != null)
                {
                    fContainer.Navigate(new Uri("pack://application:,,,/MVC/View/SalesView.xaml", UriKind.Absolute));
                }
                else
                {
                    MessageBox.Show("Frame container is not initialized.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }

        }

        private void RDBhr_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (fContainer != null)
                {
                    fContainer.Navigate(new Uri("pack://application:,,,/MVC/View/EmployeeView.xaml", UriKind.Absolute));
                }
                else
                {
                    MessageBox.Show("Frame container is not initialized.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }
        }

        private void RDBReport_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (fContainer != null)
                {
                    fContainer.Navigate(new Uri("pack://application:,,,/MVC/View/ReportsView.xaml", UriKind.Absolute));
                }
                else
                {
                    MessageBox.Show("Frame container is not initialized.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }
        }

        private void RDBSystem_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (fContainer != null)
                {
                    fContainer.Navigate(new Uri("pack://application:,,,/MVC/View/SystemDiagnosisView.xaml", UriKind.Absolute));
                }
                else
                {
                    MessageBox.Show("Frame container is not initialized.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }
        }

        private void RDBCarrier_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (fContainer != null)
                {
                    fContainer.Navigate(new Uri("pack://application:,,,/MVC/View/CarrierView.xaml", UriKind.Absolute));
                }
                else
                {
                    MessageBox.Show("Frame container is not initialized.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }
        }



        private void RDBSettings_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (fContainer != null)
                {
                    fContainer.Navigate(new Uri("pack://application:,,,/MVC/View/SettingsView.xaml", UriKind.Absolute));
                }
                else
                {
                    MessageBox.Show("Frame container is not initialized.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }
        }

        private void RDBHelp_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (fContainer != null)
                {
                    fContainer.Navigate(new Uri("pack://application:,,,/MVC/View/HelpView.xaml", UriKind.Absolute));
                }
                else
                {
                    MessageBox.Show("Frame container is not initialized.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }
        }

        private void RDBStock_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (fContainer != null)
                {
                    fContainer.Navigate(new Uri("pack://application:,,,/MVC/View/InventoryView.xaml", UriKind.Absolute));
                }
                else
                {
                    MessageBox.Show("Frame container is not initialized.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }
        }

        private void RDBDashbord_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (fContainer != null)
                {
                    fContainer.Navigate(new Uri("pack://application:,,,/MVC/View/DashboardView.xaml", UriKind.Absolute));
                }
                else
                {
                    MessageBox.Show("Frame container is not initialized.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }
        }


        private void RDBServices_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                // Ensure fInventoryContainer is a Frame control
                if (fContainer != null)
                {
                    // Navigate to the InventoryDetailedView UserControl
                    fContainer.Navigate(new ServicesView());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }
        }






        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;
            if (parentObject is T parent) return parent;
            return FindParent<T>(parentObject);
        }

        public static T FindChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T t) return t;
                var childOfChild = FindChild<T>(child);
                if (childOfChild != null) return childOfChild;
            }
            return null;
        }

        public void OpenEditEmployeeView(Employee employee)
        {

            MessageBox.Show("OpeneditEmployeViwe");

            //fEmployeeDetailsContainer.Navigate(new AddNewEmployeeView());

            MessageBox.Show("OpeneditEmployeViwe");
            var editEmployeeView = new EditEmployeeView();
            editEmployeeView.LoadEmployeeData(employee);

            // Assuming MyFrame is the navigation frame
            fEmployeeEditContainer.Content = editEmployeeView;
            editEmployeeView.LoadEmployeeData(null);
        }

        public void LoadEmployeeDetailsView()
        {
            //var employeeView = FindChild<EmployeeView>(this);
            //if (employeeView != null)
            //{
            //    MessageBox.Show("EmployeeView found!");
            //    fEmployeeEditContainer.Content = null;
            //    employeeView.OpenEmployeeDetailsView(); // Assuming this method exists in EmployeeView
            //}
            //else
            //{
            //    MessageBox.Show("EmployeeView not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //}

            var employeeView = FindChild<EmployeeView>(this);
            if (employeeView != null)
            {
                MessageBox.Show("EmployeeView found!");

                // Hide navigation bar if fEmployeeEditContainer is a Frame
                if (fEmployeeEditContainer is Frame frame)
                {
                    frame.NavigationUIVisibility = NavigationUIVisibility.Hidden; // Hide navigation bar
                    while (frame.CanGoBack) // Clear history
                    {
                        frame.RemoveBackEntry();
                    }
                }

                // Clear the content of the container
                fEmployeeEditContainer.Content = null;

                // Open the employee details view
                employeeView.OpenEmployeeDetailsView(); // Assuming this method exists in EmployeeView
            }
            else
            {
                MessageBox.Show("EmployeeView not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }

        private void btnBackDashboard_Click(object sender, RoutedEventArgs e)
        {
            txtNewpassword.Password = "";
            txtNewConfirmpassword.Password = "";
            txtOldpassword.Password = "";
            txtUsername.Text = "";
            txtNewpassword.IsEnabled = false;
            txtNewConfirmpassword.IsEnabled = false;
            ChangePasswordButton.IsEnabled = false;
        }

        private void btnconfirmedYes_Click(object sender, RoutedEventArgs e)
        {
            loginPage loginPage = new loginPage();
            loginPage.Show();
            this.Close();
            dashboardView.stoptimer();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
          
        }
    }
}
