using System;
using System.Data;
using System.Data.Common;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using Windows.Security.Credentials.UI;
using ModernApp.MVC.Model;

namespace ModernApp.MVC.View.SettingsSubViews
{
    public partial class ChangeAdminPasswordsSubView : UserControl
    {
        private bool isAuthenticated = false;
        private readonly DBconnection dbConnection;
        public ChangeAdminPasswordsSubView()
        {
            InitializeComponent();
            dbConnection = new DBconnection();
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
                MessageBox.Show("Password changed successfully!");
                txtNewpassword.IsEnabled = false;
                txtNewConfirmpassword.IsEnabled = false;
                txtNewpassword.Password = "";
                txtNewConfirmpassword.Password = "";
                txtOldpassword.Password = "";
            }
            else
            {
                MessageBox.Show("Failed to update password!");
            }
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
    }
}
