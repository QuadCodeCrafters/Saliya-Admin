using ModernApp.MVC.Model;
using ModernApp.MVC.View.EmployeeSubviews;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls; // Assuming this is a WPF application, for DataGrid.

namespace ModernApp.MVC.Controller
{
    internal class EmployeeController
    {
        private readonly EmployeeModel _employeeModel;

        public EmployeeController()
        {
            _employeeModel = new EmployeeModel();
        }

        /// <summary>
        /// Fetches all employees and binds them to the DataGrid in the view.
        /// </summary>
        /// <returns>A List of Employee objects.</returns>
        public List<Employee> GetAllEmployees()
        {
            return _employeeModel.GetAllEmployees();
        }

       

        /// <summary>
        /// Binds the list of employees to the DataGrid.
        /// This method is designed for WPF but can be adapted for WinForms or other environments.
        /// </summary>
        /// <param name="dataGrid">The DataGrid to bind the data to.</param>
        public void BindEmployeesToDataGrid(DataGrid dataGrid)
        {
            // Fetch the employee data from the model
            List<Employee> employees = GetAllEmployees();

            // Set the DataGrid's items source to the employee list
            dataGrid.ItemsSource = employees;
        }

        public void AddEmployee(string fullname, string email, DateTime dob, string address, string nic, string position, decimal salary)
        {
            try
            {
                // Validate the input data before passing to the model
                if (string.IsNullOrWhiteSpace(fullname) || string.IsNullOrWhiteSpace(email) ||
                    string.IsNullOrWhiteSpace(nic) || string.IsNullOrWhiteSpace(position) || salary <= 0)
                {
                    MessageBox.Show("Please ensure all fields are filled correctly.");
                    return;
                }

                // Create an Employee object from the data
                Employee newEmployee = new Employee
                {
                    Name = fullname,
                    Mail = email,
                    DOB = dob,
                    Address = address,
                    NationalIdentificationNumber = nic,
                    Position = position,
                    Salary = salary
                };

                // Pass the Employee object to the model to add to the database
                bool isAdded = _employeeModel.AddEmployee(newEmployee);

                if (isAdded)
                {
                    // Show success message
                    MessageBox.Show("Employee added successfully!");
                }
                else
                {
                    // If something went wrong with the database insertion
                    MessageBox.Show("Failed to add employee.");
                }
            }
            catch (Exception ex)
            {
                // Handle any errors that might occur
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}