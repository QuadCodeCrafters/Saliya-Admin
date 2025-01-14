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
            fContainer.Navigate(new Uri("pack://application:,,,/MVC/View/HomeView.xaml", UriKind.Absolute));
           


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
    }
}
