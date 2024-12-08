using MaterialDesignThemes.Wpf;
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

            if (EmployeeDataGrid.SelectedItem is Employee selectedEmployee)
            {
                PermissionDialogHost.DataContext = selectedEmployee;

                // Debug: Ensure Position matches ComboBoxItem.Content
                MessageBox.Show($"Selected Employee Position: {selectedEmployee.Position}");
                // Explicitly set ComboBox selection after setting DataContext
                var position = selectedEmployee.Position;
                foreach (ComboBoxItem item in ComboBoxPosiiton.Items)
                {
                    if (item.Content.ToString() == position)
                    {
                        ComboBoxPosiiton.SelectedItem = item;
                        break;
                    }
                }

                // Open the dialog
                DialogHost.OpenDialogCommand.Execute(null, PermissionDialogHost);
            }   
            //if (EmployeeDataGrid.SelectedItem is Employee selectedEmployee)
            //{

            //    // Assign the selected employee to the dialog host's data context
            //    //PermissionDialogHost.DataContext = selectedEmployee;

            //    //ComboBoxPosiiton.SelectedIndex = EmployeeDataGrid.SelectedIndex;
            //    //// Pre-fill text fields directly (assuming they are inside the DialogHost)
            //    txtname.Text = selectedEmployee.Name;
            //    txtSalary.Text = selectedEmployee.Salary.ToString();
            //    txtEmail.Text = selectedEmployee.Mail;
            //    txtAddress.Text = selectedEmployee.Address;
            //    txtPhoneNum.Text = selectedEmployee.Phone;
            //    txtNIC.Text = selectedEmployee.NationalIdentificationNumber;
            //    ComboBoxPosiiton.SelectedItem = selectedEmployee.Position;
            //    MessageBox.Show(selectedEmployee.Position);

            //    // Open the dialog (if using MaterialDesign's DialogHost.ShowDialog method)
            //    DialogHost.OpenDialogCommand.Execute(null, PermissionDialogHost);
            //}
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