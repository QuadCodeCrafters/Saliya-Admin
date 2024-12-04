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
           
        }



        private int currentNewEmployees = 500;
       

        

        //Change colors in the mployeeCount accordign to increment and decrement 
        private void UpdateEmployeeCount(int newCount)
        {
            // Determine if the value is increasing or decreasing
            var isIncreasing = newCount > currentNewEmployees;

            // Update the TextBlock's value with the correct sign
            totalNewEmployeesBlock.Text = (isIncreasing ? "+" : "-") + newCount;

            // Change color based on the change
            totalNewEmployeesBlock.Foreground = new SolidColorBrush(isIncreasing ? Colors.LightGreen : Colors.Red);

            // Update the current value
            currentNewEmployees = newCount;
        }


        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Simulate a change in the employee count
            Random random = new Random();
            int newCount = random.Next(-100, 1000); // Random new value
            UpdateEmployeeCount(newCount);
        }

        
    }
}
