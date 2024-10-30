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


namespace ModernApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            fContainer.Navigate(new Uri("pack://application:,,,/MVVM/View/HomeView.xaml", UriKind.Absolute));
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

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            ProfilePopup.IsOpen = true;
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            try
            {
                // Make sure the URI points to the compiled XAML file location
                fContainer.Navigate(new System.Uri("pack://application:,,,/MVVM/View/HomeView.xaml", UriKind.Absolute));
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
                    fContainer.Navigate(new Uri("pack://application:,,,/MVVM/View/DiscoveryView.xaml", UriKind.Absolute));
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
                    fContainer.Navigate(new Uri("pack://application:,,,/MVVM/View/SalesView.xaml", UriKind.Absolute));
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
                    fContainer.Navigate(new Uri("pack://application:,,,/MVVM/View/EmployeeView.xaml", UriKind.Absolute));
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
                    fContainer.Navigate(new Uri("pack://application:,,,/MVVM/View/ReportsView.xaml", UriKind.Absolute));
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
                    fContainer.Navigate(new Uri("pack://application:,,,/MVVM/View/SystemDiagnosisView.xaml", UriKind.Absolute));
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
                    fContainer.Navigate(new Uri("pack://application:,,,/MVVM/View/SettingsView.xaml", UriKind.Absolute));
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
                    fContainer.Navigate(new Uri("pack://application:,,,/MVVM/View/HelpView.xaml", UriKind.Absolute));
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
                    fContainer.Navigate(new Uri("pack://application:,,,/MVVM/View/InventoryView.xaml", UriKind.Absolute));
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
                    fContainer.Navigate(new Uri("pack://application:,,,/MVVM/View/DashboardView.xaml", UriKind.Absolute));
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
    }
}
