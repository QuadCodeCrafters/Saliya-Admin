using ModernApp.MVC.Controller;
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
    /// Interaction logic for AddNewEmployeeView.xaml
    /// </summary>
    public partial class AddNewEmployeeView : UserControl
    {
        private readonly EmployeeController employeeController;
        public AddNewEmployeeView()
        {
            InitializeComponent();
            employeeController = new EmployeeController(); // Initialize the controller
            employeeController.ImageUpdated += UpdateProfileImage;
        }
        private void UpdateProfileImage(BitmapImage image)
        {
            imgEmployee.Source = image; // Assuming imgEmployee is your Image control
        }

        private void ProfileImage_DragEnter(object sender, DragEventArgs e)
        {
            employeeController.HandleDragEnter(e);
        }

        private void ProfileImage_Drop(object sender, DragEventArgs e)
        {
            employeeController.HandleDrop(e);
        }

        private void UploadImage_Click(object sender, RoutedEventArgs e)
        {
            employeeController.OpenFileDialog();
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
        private void btnEmployeeRegister_Click(object sender, RoutedEventArgs e)
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


            // Call the controller's method to add the employee
            employeeController.AddEmployee(fullName, email, dob, address, nic, position, salary, phone, this);


        }

        private void txtNICnumber_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


        private void ChangeImage_Click(object sender, RoutedEventArgs e)
        {
            employeeController.OpenFileDialog();
            employeeController.DeleteImage();
        }
       






        //public class EmployeeDetails
        //{
        //    public string Name { get; set; }
        //    public string Position { get; set; }
        //    public decimal Salary { get; set; }
        //    public DateTime DOB { get; set; }
        //    public string Status { get; set; } = "Active"; // Default value for Status
        //    public string Mail { get; set; }
        //    public string Address { get; set; }
        //    public string Phone { get; set; }
        //    public DateTime HireDate { get; set; } = DateTime.Now; // Default to current date
        //    public string NationalIdentificationNumber { get; set; }
        //}


    }
}
