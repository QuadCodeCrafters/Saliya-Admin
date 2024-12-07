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
    }
}
