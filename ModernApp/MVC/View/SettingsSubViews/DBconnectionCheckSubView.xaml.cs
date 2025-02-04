using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ModernApp.MVC.Model;
using MySql.Data.MySqlClient;
using Saliya_auto_care_Cashier.Notifications;

namespace ModernApp.MVC.View.SettingsSubViews
{
    public partial class DBconnectionCheckSubView : UserControl
    {
        private readonly DBconnection dBconnection;

        public DBconnectionCheckSubView()
        {
            InitializeComponent();
            dBconnection = new DBconnection(); 
            CheckDatabaseStatus();
        }

        // ✅ Check Database Connection
        private void btnCheckDBstatus_Click(object sender, RoutedEventArgs e)
        {
            CheckDatabaseStatus();
        }

        private void CheckDatabaseStatus()
        {
            try
            {
                using (var connection = dBconnection.GetConnection()) // Use existing DB connection
                {
                    connection.Open();

                    // ✅ If connection is successful
                    txtDBStatus.Text = "Connected ✅";
                    txtDBStatus.Foreground = System.Windows.Media.Brushes.Green;
                }
            }
            catch (Exception)
            {
                // ❌ If connection fails
                txtDBStatus.Text = "Disconnected ❌";
                txtDBStatus.Foreground = System.Windows.Media.Brushes.Red;

                Notificationbox.ShowError();
            }
        }

        // 🔄 Reconnect to the Database
        private void btnReconnectDB_Click(object sender, RoutedEventArgs e)
        {
            CheckDatabaseStatus();
        }

        // 🔄 Refresh DB Status
        private void btnRefreshDBStatus_Click(object sender, RoutedEventArgs e)
        {
            CheckDatabaseStatus();
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
