using ModernApp.MVC.Controller;
using ModernApp.MVVM.View;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for EditEmployeeView.xaml
    /// </summary>
    public partial class EditEmployeeView : UserControl
    {
        private readonly EmployeeController employeeController;
        public string imagePath;


        public EditEmployeeView()
        {
            InitializeComponent();
            employeeController = new EmployeeController(); // Initialize the controller
           

        }


        private void btnBackEmployeeDashboard_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to the Employee Dashboard or previous page
            // Assuming MyFrame is accessible and hosts the content
            var parentFrame = Window.GetWindow(this)?.FindName("fEmployeeDetailsContainer") as Frame;
            if (parentFrame != null)
            {
                parentFrame.GoBack();
            }
        }

        public void LoadEmployeeData(Employee employee)
        {
            if (employee == null) return;

            // Set the values of controls
            txtFullname.Text = employee.Name;
            txtPhone.Text = employee.Phone;
            txtEmail.Text = employee.Mail;
            txtAddress.Text = employee.Address;
            txtNICnumber.Text = employee.NationalIdentificationNumber;
            CBXPosition.SelectedItem = CBXPosition.Items
                .Cast<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString() == employee.Position);
            txtSalary.Text = employee.Salary.ToString("F2"); // Format as currency
            DPDOB.SelectedDate = employee.DOB;
            imagePath = employee.ProfilePicPath;

            // Load profile picture
            if (!string.IsNullOrEmpty(employee.ProfilePicPath) && File.Exists(employee.ProfilePicPath))
            {
                imgEmployee.Source = new BitmapImage(new Uri(employee.ProfilePicPath));
            }
            else
            {
                imgEmployee.Source = null; // Or set a default image
            }
        }

        private void BtnchangePhoto_Click(object sender, RoutedEventArgs e)
        {
            DeleteUpdateImage();
            employeeController.OpenFileDialog();
        }

        public void DeleteUpdateImage()
        {
            try
            {
                

                // Combine ImageFolderPath and the file name from relativePath

                // Check if the image exists
                if (File.Exists(imagePath))
                {
                    // Delete the image
                    File.Delete(imagePath);
                    MessageBox.Show("Image deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Image not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEmployeeUpdate_Click(object sender, RoutedEventArgs e)
        {
           
                // Collect updated data from the UI
                var updatedEmployee = new Employee
                {
                    EmployeeID = int.Parse(txtNICnumber.Text), // Assuming EmployeeID is entered or already populated
                    Name = txtFullname.Text,
                    Phone = txtPhone.Text,
                    Mail = txtEmail.Text,
                    Address = txtAddress.Text,
                    NationalIdentificationNumber = txtNICnumber.Text,
                    Position = (CBXPosition.SelectedItem as ComboBoxItem)?.Content.ToString(),
                    Salary = decimal.TryParse(txtSalary.Text, out var salary) ? salary : 0,
                    DOB = DPDOB.SelectedDate ?? DateTime.MinValue,
                    ProfilePicPath = imgEmployee.Source is BitmapImage bitmapImage ? bitmapImage.UriSource.ToString() : null
                };

                // Call the EmployeeController to update the employee
                var employeeController = new EmployeeController();
                bool isUpdated = employeeController.UpdateEmployee(updatedEmployee);

                if (isUpdated)
                {
                    MessageBox.Show("Employee details updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Navigate back to Employee Dashboard or previous view
                    var parentFrame = Window.GetWindow(this)?.FindName("MyFrame") as Frame;
                    parentFrame?.GoBack();
                }
                else
                {
                    MessageBox.Show("Failed to update employee details. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            

        }
    }
    }
