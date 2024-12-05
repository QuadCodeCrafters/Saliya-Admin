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

        private List<Employee> _employees;
        public EmployeeDetails()
        {
            InitializeComponent();
            LoadEmployeeData();
        }
        private void LoadEmployeeData()
        {
            _employees = new List<Employee>
            {
                new Employee { Id = 1, Name = "John Doe", Department = "HR", Position = "Manager", Status = "Active" },
                new Employee { Id = 2, Name = "Jane Smith", Department = "IT", Position = "Developer", Status = "Active" },
                new Employee { Id = 3, Name = "Tom Brown", Department = "Finance", Position = "Analyst", Status = "On Leave" }
            };

            EmployeeDataGrid.ItemsSource = _employees;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchQuery = SearchBox.Text.ToLower();
            var filteredEmployees = _employees.Where(emp => emp.Name.ToLower().Contains(searchQuery)).ToList();
            EmployeeDataGrid.ItemsSource = filteredEmployees;
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
        public int Id { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string Status { get; set; }
    }
}