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
using MaterialDesignThemes.Wpf;

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
            employeeController.ImageUpdated += UpdateProfileImage;

        }
        private void UpdateProfileImage(BitmapImage image)
        {
            imgEmployee.Source = image; // Assuming imgEmployee is your Image control
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

        int employeeID;

        public void LoadEmployeeData(Employee employee)
        {
            if (employee == null) return;

            // Set the values of controls
            employeeID=employee.EmployeeID;
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
                using (var stream = new FileStream(employee.ProfilePicPath, FileMode.Open, FileAccess.Read))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad; // Fully load the image into memory
                    bitmap.EndInit();
                    imgEmployee.Source = bitmap;

                }

            }
            else
            {
                imgEmployee.Source = null; // Or set a default image
            }
        }



        private void BtnchangePhoto_Click(object sender, RoutedEventArgs e)
        {
            // Ensure the image source is released before moving
            ReleaseImageLock();

            // Move the old image to the target folder
           // MoveUpdateImage();
            DeleteUpdateImage();

            // Open the file dialog to select a new image
            employeeController.OpenFileDialog();
        }


        private void ReleaseImageLock()
        {
            // Check if the image is loaded in an Image control and release the lock
            if (imgEmployee.Source != null)
            {
                imgEmployee.Source = null; // Set source to null to release any file locks
                GC.Collect(); // Force garbage collection
                GC.WaitForPendingFinalizers(); // Ensure finalizers have run
            }
        }

        public void DeleteUpdateImage()
        {
            try
            {


                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                    MessageBox.Show("Image deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Image not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (IOException ioEx)
            {
                MessageBox.Show($"Error deleting image: {ioEx.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        //public void MoveUpdateImage()
        //{
        //    try
        //    {
        //        if (File.Exists(imagePath))
        //        {
        //            // Define the target folder path
        //            string targetFolder = "D:\\VM OS";

        //            // Ensure the target folder exists
        //            if (!Directory.Exists(targetFolder))
        //            {
        //                Directory.CreateDirectory(targetFolder);
        //            }

        //            // Generate a unique file name to avoid conflicts
        //            string newFileName = System.IO.Path.Combine(targetFolder, System.IO.Path.GetFileName(imagePath));

        //            // Move the file to the target folder
        //            File.Move(imagePath, newFileName);

        //            MessageBox.Show($"Image moved successfully to {newFileName}!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

        //            // Update the image path to reflect the new location
        //            imagePath = newFileName;
        //        }
        //        else
        //        {
        //            MessageBox.Show("Image not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //    }
        //    catch (IOException ioEx)
        //    {
        //        MessageBox.Show($"Error moving image: {ioEx.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error moving image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}



       

        private void btnEmployeeUpdate_Click(object sender, RoutedEventArgs e)
        {
            // Collect data from UI controls

            string fullName = txtFullname.Text;
            string email = txtEmail.Text;
            DateTime dob = DPDOB.SelectedDate ?? DateTime.MinValue; // Fallback if no date is selected
            string address = txtAddress.Text;
            string nic = txtNICnumber.Text;
            string position = (CBXPosition.SelectedItem as ComboBoxItem)?.Content as string; // Safely get position
            decimal salary;
            string phone = txtPhone.Text;

            // Try parsing the salary value
            if (!decimal.TryParse(txtSalary.Text, out salary))
            {
                MessageBox.Show("Please enter a valid salary.");

                return;
            }

            // Check if all fields are filled
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(nic) || string.IsNullOrWhiteSpace(nic) ||
                string.IsNullOrWhiteSpace(position) || salary <= 0 || dob == DateTime.MinValue)
            {
                MessageBox.Show("Please ensure all fields are filled correctly.");
                return;
            }

            //MessageBox.Show(Convert.ToString(employeeID));
            // Call the controller's method to add the employee
            employeeController.updateEmployee(employeeID, fullName, email, dob, address, nic, position, salary, phone, this);
        }

        private void btnBackEmployeeDashboard_Click_1(object sender, RoutedEventArgs e)
        {
           
           
          

            // Find the EmployeeView parent
            var parentEmployeeView = FindParent<MainWindow>(this);

            if (parentEmployeeView != null)
            {
            
                // Pass the selected employee to EmployeeView
                parentEmployeeView.LoadEmployeeDetailsView();


            }
            else
            {
                MessageBox.Show("Parent view not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void btnEmployeeCancel_Click(object sender, RoutedEventArgs e)
        {

          
        }

        //// Collect updated data from the UI
        //var updatedEmployee = new Employee
        //{
        //    EmployeeID = employee.EmployeeID, // Assuming EmployeeID is entered or already populated
        //    Name = txtFullname.Text,
        //    Phone = txtPhone.Text,
        //    Mail = txtEmail.Text,
        //    Address = txtAddress.Text,
        //    NationalIdentificationNumber = txtNICnumber.Text,
        //    Position = (CBXPosition.SelectedItem as ComboBoxItem)?.Content.ToString(),
        //    Salary = decimal.TryParse(txtSalary.Text, out var salary) ? salary : 0,
        //    DOB = DPDOB.SelectedDate ?? DateTime.MinValue,
        //    ProfilePicPath = imgEmployee.Source is BitmapImage bitmapImage ? bitmapImage.UriSource.ToString() : null
        //};

        //// Call the EmployeeController to update the employee
        //var employeeController = new EmployeeController();
        //bool isUpdated = employeeController.UpdateEmployee(updatedEmployee);

        //if (isUpdated)
        //{
        //    MessageBox.Show("Employee details updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

        //    // Navigate back to Employee Dashboard or previous view
        //    var parentFrame = Window.GetWindow(this)?.FindName("MyFrame") as Frame;
        //    parentFrame?.GoBack();
        //}
        //else
        //{
        //    MessageBox.Show("Failed to update employee details. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //}


    }
    }
    
