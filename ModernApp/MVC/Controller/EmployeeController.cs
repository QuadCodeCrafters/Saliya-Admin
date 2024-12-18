using ModernApp.MVC.Model;
using ModernApp.MVC.View.EmployeeSubviews;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls; // Assuming this is a WPF application, for DataGrid.
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.IO;
using Saliya_auto_care_Cashier.Notifications;
using ModernApp.Core;
using MySql.Data.MySqlClient;


namespace ModernApp.MVC.Controller
{
    internal class EmployeeController
    {
        //Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MVC" , "View" , "EmployeeSubviews" , "EmployeePictures" );
        // Folder path inside the project (Image/profilepics)
        private readonly string ImageFolderPath = "D:\\personal\\Coding\\C#\\ModernApp\\ModernApp\\MVC\\View\\EmployeeSubviews\\EmployeePictures\\";
        private string relativePath;
        private readonly EmployeeModel employeeModel;
        public event Action<BitmapImage> ImageUpdated;


        public EmployeeController()
        {
            employeeModel = new EmployeeModel();
           

        }

        // Handles drag-and-drop logic
        public void HandleDragEnter(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        public void HandleDrop(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0 && IsImageFile(files[0]))
                {
                    LoadImage(files[0]);
                    SaveImageAndStorePath(files[0]);
                }
                else
                {
                    MessageBox.Show("Please drop a valid image file.", "Invalid File", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        public void OpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                Title = "Select a Profile Image"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                LoadImage(openFileDialog.FileName);
                SaveImageAndStorePath(openFileDialog.FileName);
            }
        }





        public void SaveImageAndStorePath(string filePath)
        {
            try
            {
                // Ensure the folder exists; create it if it doesn't
                if (!Directory.Exists(ImageFolderPath))
                {
                    Directory.CreateDirectory(ImageFolderPath);
                    MessageBox.Show("Folder created.");
                }
                else
                {
                    MessageBox.Show("Folder found.");
                }

                // Generate a unique file name to avoid conflicts
                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(filePath)}";
                string destinationPath = Path.Combine(ImageFolderPath, fileName);

                // Copy the image to the folder
                File.Copy(filePath, destinationPath, overwrite: true);

                // Calculate the relative path
                 relativePath = destinationPath;
              
              

                MessageBox.Show("Image saved successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

       

        public void DeleteImage()
        {
            try
            {
                // Ensure that relativePath and ImageFolderPath are not null or empty
                if (string.IsNullOrEmpty(relativePath) || string.IsNullOrEmpty(ImageFolderPath))
                {
                    MessageBox.Show("Invalid file path.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Combine ImageFolderPath and the file name from relativePath
                string imagePath = relativePath;

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
        public void DeleteUpdateImage()
        {
            try
            {
                // Ensure that relativePath and ImageFolderPath are not null or empty
                if (string.IsNullOrEmpty(relativePath) || string.IsNullOrEmpty(ImageFolderPath))
                {
                    MessageBox.Show("Invalid file path.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Combine ImageFolderPath and the file name from relativePath
                string imagePath = relativePath;

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



        //// Example database storage logic
        //private void StoreImagePathInDatabase(string relativePath)
        //{
        //    try
        //    {
        //        // Replace this with your actual database interaction code
        //        using (var connection = new MySqlConnection("YourDatabaseConnectionString"))
        //        {
        //            connection.Open();
        //            string query = "INSERT INTO EmployeeImages (ImagePath) VALUES (@ImagePath)";
        //            using (MySqlCommand command = new MySqlCommand(query, connection))
        //            {
        //                command.Parameters.AddWithValue("@ImagePath", relativePath);
        //                command.ExecuteNonQuery();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error saving image path to database: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        // Loads an image and triggers the ImageUpdated event
        private void LoadImage(string filePath)
        {
            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(filePath);
                bitmap.EndInit();
                ImageUpdated?.Invoke(bitmap); // Notify listeners of the new image
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Validates the file as an image
        private bool IsImageFile(string filePath)
        {
            string[] validExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
            string extension = Path.GetExtension(filePath)?.ToLower();
            return Array.Exists(validExtensions, ext => ext == extension);
        }




        /// <summary>
        /// Fetches all employees and binds them to the DataGrid in the view.
        /// </summary>
        /// <returns>A List of Employee objects.</returns>
        public List<Employee> GetAllEmployees()
        {
            return employeeModel.GetAllEmployees();
        }


        public bool UpdateEmployee(Employee employee)
        {
            return employeeModel.UpdateEmployee(employee);
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

            // Check if employees data is coming in correctly
            if (employees == null || employees.Count == 0)
            {
                MessageBox.Show("No employee data found.");
                return; // Exit if no data
            }

            // Set the DataGrid's items source to the employee list
            dataGrid.ItemsSource = employees;
        }


        public void AddEmployee(string fullname, string email, DateTime dob, string address, string nic, string position, decimal salary, string phone, DependencyObject container)
        {
            try
            {
                // Validate the input data before passing to the model
                if (string.IsNullOrWhiteSpace(fullname) || string.IsNullOrWhiteSpace(email) ||
                    string.IsNullOrWhiteSpace(nic) || string.IsNullOrWhiteSpace(position) || salary <= 0 || string.IsNullOrWhiteSpace(phone))
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
                    Salary = salary,
                    Phone = phone,
                    ProfilePicPath = relativePath

                };

                // Pass the Employee object to the model to add to the database
                bool isAdded = employeeModel.AddEmployee(newEmployee);

                if (isAdded)
                {
                    Notificationbox.ShowSuccess();

                    // Show success message
                    MessageBox.Show("Employee added successfully!");
                    ClearHelper.ClearTextBoxesAndComboBoxes(container);

                }
                else
                {
                    Notificationbox.ShowError();
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