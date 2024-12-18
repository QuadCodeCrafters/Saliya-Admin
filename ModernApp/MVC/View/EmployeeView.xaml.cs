using LiveCharts.Definitions.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using ModernApp;
using Saliya_auto_care_Cashier.Notifications;
using ModernApp.MVC.View.InventorySubviews;
using ModernApp.MVC.View.EmployeeSubviews;
using ModernApp.MVC;
using MaterialDesignThemes.Wpf;
using Mysqlx.Notice;


namespace ModernApp.MVVM.View
{
    /// <summary>
    /// Interaction logic for EmployeeView.xaml
    /// </summary>
    public partial class EmployeeView : UserControl
    {
        public EmployeeView()
        {
            InitializeComponent();
            DataContext = this;

            // Find the EmployeeDetails UserControl
            //EmployeeDetails employeeDetails = VisualTreeHelperExtensions.FindChild<EmployeeDetails>(this);
            //if (employeeDetails != null)
            //{
            //    // Subscribe to the EditEmployeeRequested event
            //    employeeDetails.EditEmployeeRequested += OnEditEmployeeRequested;
            //}

        }

        //public static class VisualTreeHelperExtensions
        //{
        //    // Method to find a child control of a specific type
        //    public static T FindChild<T>(DependencyObject parent) where T : DependencyObject
        //    {
        //        if (parent == null) return null;

        //        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        //        {
        //            var child = VisualTreeHelper.GetChild(parent, i);
        //            if (child is T t) return t;

        //            // Recursively search for the child in the tree
        //            var childOfChild = FindChild<T>(child);
        //            if (childOfChild != null) return childOfChild;
        //        }

        //        return null;
        //    }
        //}

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




        private int currentNewEmployees = 500;

 

       



        //Change colors in the mployeeCount accordign to increment and decrement 
        private void UpdateEmployeeCount(int newCount)
        {
            // Determine if the value is increasing or decreasing
            //var isIncreasing = newCount > currentNewEmployees;

            //// Update the TextBlock's value with the correct sign
            //totalNewEmployeesBlock.Text = (isIncreasing ? "+" : "-") + newCount;

            //// Change color based on the change
            //totalNewEmployeesBlock.Foreground = new SolidColorBrush(isIncreasing ? Colors.LightGreen : Colors.Red);

            //// Update the current value
            //currentNewEmployees = newCount;
        }


        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Simulate a change in the employee count
            Random random = new Random();
            int newCount = random.Next(-100, 1000); // Random new value
            UpdateEmployeeCount(newCount);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Notificationbox.ShowSuccess();
            try
            {
                // Ensure fInventoryContainer is a Frame control
                if (fEmployeeDetailsContainer != null)
                {
                    // Navigate to the InventoryDetailedView UserControl
                    fEmployeeDetailsContainer.Navigate(new AddNewEmployeeView());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }



        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Notificationbox.ShowError();
            try
            {
                // Ensure fInventoryContainer is a Frame control
                if (fEmployeeDetailsContainer != null)
                {
                    // Navigate to the InventoryDetailedView UserControl
                    fEmployeeDetailsContainer.Navigate(new EmployeeDetails());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Notificationbox.ShowInfo();
        }

        private void fEmployeeDetailsContainer_Navigated(object sender, NavigationEventArgs e)
        {

        }



        public void OpenEditEmployeeView(Employee employee)
        {
            var editEmployeeView = new EditEmployeeView();
            editEmployeeView.LoadEmployeeData(employee);

            // Assuming MyFrame is the navigation frame
            fEmployeeEditContainer.Content = editEmployeeView;
        }

    }
}
