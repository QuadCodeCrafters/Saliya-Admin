using MaterialDesignThemes.Wpf;
using ModernApp.MVC.Controller;
using ModernApp.MVC.View.InventorySubviews;
using ModernApp.MVVM.View;
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

        //private void EditButton_Click(object sender, RoutedEventArgs e)
        //{
        //    string profilepic = Employee.ProfielPicPath;
        //    imgEmployee.Source = Employee.profilepic;
        //    if (EmployeeDataGrid.SelectedItem is Employee selectedEmployee)
        //    {
        //        PermissionDialogHost.DataContext = selectedEmployee;

        //        // Debug: Ensure Position matches ComboBoxItem.Content
        //        MessageBox.Show($"Selected Employee Position: {selectedEmployee.Position}");
        //        // Explicitly set ComboBox selection after setting DataContext
        //        var position = selectedEmployee.Position;
        //        foreach (ComboBoxItem item in ComboBoxPosiiton.Items)
        //        {
        //            if (item.Content.ToString() == position)
        //            {
        //                ComboBoxPosiiton.SelectedItem = item;
        //                break;
        //            }
        //        }

        //        // Open the dialog
        //        DialogHost.OpenDialogCommand.Execute(null, PermissionDialogHost);
        //    }   
        //    //if (EmployeeDataGrid.SelectedItem is Employee selectedEmployee)
        //    //{

        //    //    // Assign the selected employee to the dialog host's data context
        //    //    //PermissionDialogHost.DataContext = selectedEmployee;

        //    //    //ComboBoxPosiiton.SelectedIndex = EmployeeDataGrid.SelectedIndex;
        //    //    //// Pre-fill text fields directly (assuming they are inside the DialogHost)
        //    //    txtname.Text = selectedEmployee.Name;
        //    //    txtSalary.Text = selectedEmployee.Salary.ToString();
        //    //    txtEmail.Text = selectedEmployee.Mail;
        //    //    txtAddress.Text = selectedEmployee.Address;
        //    //    txtPhoneNum.Text = selectedEmployee.Phone;
        //    //    txtNIC.Text = selectedEmployee.NationalIdentificationNumber;
        //    //    ComboBoxPosiiton.SelectedItem = selectedEmployee.Position;
        //    //    MessageBox.Show(selectedEmployee.Position);

        //    //    // Open the dialog (if using MaterialDesign's DialogHost.ShowDialog method)
        //    //    DialogHost.OpenDialogCommand.Execute(null, PermissionDialogHost);
        //    //}
        //}


        //        public static class VisualTreeHelperExtensions
        //{
        //    // Method to find a child control of a specific type
        //    //public static T FindChild<T>(DependencyObject parent) where T : DependencyObject
        //    //{
        //    //    if (parent == null) return null;

        //    //    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        //    //    {
        //    //        var child = VisualTreeHelper.GetChild(parent, i);
        //    //        if (child is T t) return t;

        //    //        // Recursively search for the child in the tree
        //    //        var childOfChild = FindChild<T>(child);
        //    //        if (childOfChild != null) return childOfChild;
        //    //    }

        //    //    return null;
        //    //}

        //    //// Method to find a parent control of a specific type
        //    //public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        //    //{
        //    //    DependencyObject parentObject = VisualTreeHelper.GetParent(child);
        //    //    if (parentObject == null) return null;
        //    //    if (parentObject is T parent) return parent;
        //    //    return FindParent<T>(parentObject);
        //    //}
        //}

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
                var parentEmployeeView = FindParent<EmployeeView>(this);

                if (parentEmployeeView != null)
                {
                    // Pass the selected employee to EmployeeView
                    parentEmployeeView.OpenEditEmployeeView(selectedEmployee);
               
            }
                else
                {
                    MessageBox.Show("Parent view not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            


            //// Find the EmployeeView parent
            //var parentEmployeeView = FindParent<EmployeeView>(this);

            //if (parentEmployeeView != null)
            //{
            //    parentEmployeeView.OpenEditEmployeeView(); // Call a method in EmployeeView
            //}
            //else
            //{
            //    MessageBox.Show("Parent view not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //}



            //if (EmployeeDataGrid.SelectedItem is Employee selectedEmployee)
            //{
            //    // Locate the parent EmployeeView UserControl
            //    var parent = FindParent<EmployeeView>(this);
            //    if (parent != null)
            //    {
            //        parent.SelectedEmployee = selectedEmployee;
            //        DialogHost.OpenDialogCommand.Execute(null, parent.PermissionDialogHost);


            //    }
            //}
            //else
            //{
            //    MessageBox.Show("No employee selected!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            //}
            //if (EmployeeDataGrid.SelectedItem is Employee selectedEmployee)
            //{
            //    EditEmployeeRequested?.Invoke(this, selectedEmployee);
            //}
            //else
            //{
            //    MessageBox.Show("No employee selected!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            //}
            //if (EmployeeDataGrid.SelectedItem is Employee selectedEmployee)
            //{
            //    // Set the DataContext for the dialog
            //    PermissionDialogHost.DataContext = selectedEmployee;

            //    // Load the profile picture
            //    if (!string.IsNullOrEmpty(selectedEmployee.ProfilePicPath))
            //    {
            //        try
            //        {
            //            var bitmap = new BitmapImage();
            //            bitmap.BeginInit();
            //            bitmap.UriSource = new Uri(selectedEmployee.ProfilePicPath, UriKind.Absolute);
            //            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            //            bitmap.EndInit();
            //            imgEmployee.Source = bitmap;

            //        }
            //        catch (Exception ex)
            //        {
            //            MessageBox.Show($"Error loading profile picture: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //            //imgEmployee.Source = null; // Reset to default or blank
            //        }
            //    }
            //    else
            //    {
            //        imgEmployee.Source = null; // Reset if no profile picture is available
            //    }

            //    // Pre-fill other fields
            //    var position = selectedEmployee.Position;
            //    foreach (ComboBoxItem item in ComboBoxPosiiton.Items)
            //    {
            //        if (item.Content.ToString() == position)
            //        {
            //            ComboBoxPosiiton.SelectedItem = item;
            //            break;
            //        }
            //    }

            //    // Open the dialog
            //    DialogHost.OpenDialogCommand.Execute(null, PermissionDialogHost);
            //}
            //else
            //{
            //    MessageBox.Show("No employee selected!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
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