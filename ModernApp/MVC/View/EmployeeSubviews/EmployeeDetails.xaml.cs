using MaterialDesignThemes.Wpf;
using ModernApp.Core;
using ModernApp.MVC.Controller;
using ModernApp.MVC.Model;
using ModernApp.MVC.View.InventorySubviews;
using ModernApp.MVVM.View;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
        private readonly DBconnection dBconnection;
        private List<Employee> employees;
        public EmployeeDetails()
        {
            InitializeComponent();
            employeeController = new EmployeeController();
           dBconnection = new DBconnection();
            LoadEmployeeData();

           
           
        }
        private void LoadEmployeeData()
        {
            // Ensure employeeController is initialized
            if (employeeController == null)
            {
                MessageBox.Show("Employee controller is not initialized.");
                return;
            }

            employees = employeeController.GetAllEmployees(); // Ensure GetEmployees() returns a valid list
            if (employees == null)
            {
                MessageBox.Show("No employees found.");
                employees = new List<Employee>(); // Prevent null reference
            }

            EmployeeDataGrid.ItemsSource = employees;
        }



        public ObservableCollection<Employee> Employees { get; set; } = new ObservableCollection<Employee>();

        // Property to hold the parent reference
       



        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            
                // Ensure you have a DataGrid named "EmployeeDataGrid"
                var selectedEmployee = EmployeeDataGrid.SelectedItem as Employee;

                if (selectedEmployee == null)
                {
                    MessageBox.Show("No employee selected!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Find the EmployeeView parent
                var parentEmployeeView = FindParent<MainWindow>(this);

                if (parentEmployeeView != null)
                {
                EmployeeDataGrid.ItemsSource = null;

                


                // Pass the selected employee to EmployeeView
                parentEmployeeView.OpenEditEmployeeView(selectedEmployee);

               
            }
                else
                {
                    MessageBox.Show("Parent view not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            


          
        }


        private void TerminateButton_Click(object sender, RoutedEventArgs e)
        {
            var employee = ((Button)sender).DataContext as Employee;
            if (employee == null)
                return;

            // Confirmation before terminating
            var result = MessageBox.Show($"Are you sure you want to terminate {employee.Name}?",
                                         "Confirm Termination", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Update Employee Status to 'Inactive' in the database
                    string query = "UPDATE Employee SET Status = 'Inactive' WHERE EmployeeID = @EmployeeID";

                    using (var connection = dBconnection.GetConnection())  // Assuming GetConnection() returns an open MySQL connection
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EmployeeID", employee.EmployeeID);
                        command.ExecuteNonQuery();
                    }

                    // Update the local employee object
                    employee.Status = "Inactive";

                    MessageBox.Show($"Employee {employee.Name} has been terminated.", "Termination Successful", MessageBoxButton.OK, MessageBoxImage.Information);

                   
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error terminating employee: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
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

        private void btnclear_Click(object sender, RoutedEventArgs e)
        {
            ClearHelper.ClearTextBoxesAndComboBoxes(this);
            LoadEmployeeData();
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
        }

       


        private void ApplyFilters()
        {
            if (employees == null || employees.Count == 0)
            {
                MessageBox.Show("No employee data available.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string nameFilter = EmployeeNameTextBox.Text?.Trim().ToLower();
            string jobTitleFilter = JobTitleComboBox.SelectedItem?.ToString(); // Get SelectedItem directly
            string statusFilter = StatusComboBox.SelectedItem?.ToString();
            DateTime? startDateFilter = StartDatePicker.SelectedDate;
            DateTime? endDateFilter = EndDatePicker.SelectedDate;

            var filteredEmployees = employees.Where(emp =>
                (string.IsNullOrWhiteSpace(nameFilter) || emp.Name.ToLower().Contains(nameFilter)) &&
                (JobTitleComboBox.SelectedIndex == -1 || emp.Position.Equals(jobTitleFilter, StringComparison.OrdinalIgnoreCase)) &&
                (StatusComboBox.SelectedIndex == -1 || emp.Status.Equals(statusFilter, StringComparison.OrdinalIgnoreCase)) &&
                (!startDateFilter.HasValue || emp.HireDate.Date >= startDateFilter.Value.Date) &&
                (!endDateFilter.HasValue || emp.HireDate.Date <= endDateFilter.Value.Date)
            ).ToList();

            EmployeeDataGrid.ItemsSource = null;  // Refresh the DataGrid
            EmployeeDataGrid.ItemsSource = filteredEmployees;
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