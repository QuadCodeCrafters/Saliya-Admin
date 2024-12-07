using ModernApp.MVC.Controller;
using ModernApp.MVC.View.InventorySubviews;
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

namespace ModernApp.MVC.View.EmployeeSubviews
{
    /// <summary>
    /// Interaction logic for EmployeeDetails.xaml
    /// </summary>
    public partial class EmployeeDetails : UserControl
    {
        private readonly EmployeeController employeeController;
        private List<Employee> employees;
        public EmployeeDetails()
        {
            InitializeComponent();
            employeeController = new EmployeeController();
           
            LoadEmployeeData();
           
           
        }
        private void LoadEmployeeData()
        {
            
            employeeController.BindEmployeesToDataGrid(EmployeeDataGrid);
        }


        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            //string searchQuery = SearchBox.Text.ToLower();
            //var filteredEmployees = _employees.Where(emp => emp.Name.ToLower().Contains(searchQuery)).ToList();
            //EmployeeDataGrid.ItemsSource = filteredEmployees;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var employee = ((Button)sender).DataContext as Employee;
            MessageBox.Show($"Edit Employee: {employee.Name}", "Edit");
        }

        private void TerminateButton_Click(object sender, RoutedEventArgs e)
        {
            var employee = ((Button)sender).DataContext as Employee;
            MessageBox.Show($"Terminate Employee: {employee.Name}", "Terminate");
        }

       

       

        private void btnBackEmployeeDashboard_click(object sender, RoutedEventArgs e)
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

    public class Employee
    {
        public string Name { get; set; }
        public string Position { get; set; }
        public decimal Salary { get; set; }
        
        public DateTime DOB { get; set; }
        public string Status { get; set; }
        public string Mail { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public int EmployeeID { get; set; }
        public DateTime HireDate { get; set; }
        public string NationalIdentificationNumber { get; set; }
        public string ProfilePicPath { get; set; }
    }
}