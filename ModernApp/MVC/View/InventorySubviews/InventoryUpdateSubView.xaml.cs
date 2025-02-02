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
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;
using System.Data.Common;
using ModernApp.MVC.Model;
using Microsoft.Win32;
using System.IO;
using Path = System.IO.Path;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using ModernApp.Core;
using ModernApp.Events;
using Prism.Events;
namespace ModernApp.MVC.View.InventorySubviews
{

    
    /// <summary>
    /// Interaction logic for InventoryUpdateSubView.xaml
    /// </summary>
    public partial class InventoryUpdateSubView : UserControl
    {
        private readonly DBconnection dBconnection;
        public event Action<BitmapImage> ImageUpdated;
        private readonly string ImageFolderPath = "D:\\personal\\InventoryPictures";
        private string relativePath;
        public InventoryUpdateSubView()
        {
            InitializeComponent();
            dBconnection = new DBconnection();
            ImageUpdated += UpdateProfileImage;
            // Subscribe to ClearFormEvent
            App.EventAggregator.GetEvent<ClearFormEvent>().Subscribe(ClearForm);
        }


        private void UpdateProfileImage(BitmapImage image)
        {
            imgInventory.Source = image; // Assuming imgEmployee is your Image control
        }

        private void ProfileImage_DragEnter(object sender, DragEventArgs e)
        {
            HandleDragEnter(e);
        }

        private void ProfileImage_Drop(object sender, DragEventArgs e)
        {
            HandleDrop(e);
        }

        private void UploadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog();
        }


        private void ChangeImage_Click(object sender, RoutedEventArgs e)
        {

            DeleteImage();
            OpenFileDialog();
        }
        // Handles drag-and-drop logic
        public void HandleDragEnter(DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
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

                // Generate a unique file name to avoid conflicts
                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(filePath)}";
                string destinationPath = Path.Combine(ImageFolderPath, fileName);

                // Copy the image to the folder
                File.Copy(filePath, destinationPath, overwrite: true);

                // Store the relative path
                relativePath = destinationPath;

                MessageBox.Show("Image saved successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Consolidated method to delete image
        public void DeleteImage()
        {
            try
            {
                if (string.IsNullOrEmpty(relativePath) || string.IsNullOrEmpty(ImageFolderPath))
                {
                    MessageBox.Show("Invalid file path.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string imagePath = relativePath;

                // Check if the image exists and delete it
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
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

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
            string extension = System.IO.Path.GetExtension(filePath)?.ToLower();
            return Array.Exists(validExtensions, ext => ext == extension);
        }


        private void btnBackInventoryDashboard_Click(object sender, RoutedEventArgs e)
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

        public void LoadInventoryItem(string modelNumber)
        {
            try
            {
                using (var connection = dBconnection.GetConnection()) // Use existing DB connection
                {
                    connection.Open();
                    string query = @"
                SELECT ItemName, ItemPrice, Quantity, Category, Description, 
                       Manufacturer, ModelNumber, Warranty, StorageLocation, PicLocation
                FROM Inventory 
                WHERE ModelNumber = @ModelNumber";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ModelNumber", modelNumber);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtItemName.Text = reader["ItemName"].ToString();
                                txtItemPrice.Text = reader["ItemPrice"].ToString();
                                txtQuantity.Text = reader["Quantity"].ToString();
                                cmbCategory.SelectedItem = reader["Category"].ToString();
                                txtDescription.Text = reader["Description"].ToString();
                                txtManufacturer.Text = reader["Manufacturer"].ToString();
                                txtModelNumber.Text = reader["ModelNumber"].ToString();
                                txtWarranty.Text = reader["Warranty"].ToString();
                                txtStorageLocation.Text = reader["StorageLocation"].ToString();
                               
                                string imagePath = reader["PicLocation"]?.ToString();
                                if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                                {
                                    LoadImage(imagePath);
                                    relativePath = imagePath;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Item not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading item: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public void UpdateInventoryItem()
        {
            try
            {
                using (var connection = dBconnection.GetConnection()) // Use existing connection
                {
                    connection.Open();
                    string query = @"
                UPDATE inventory 
                SET ItemName = @ItemName, ItemPrice = @ItemPrice, Quantity = @Quantity, 
                    Category = @Category, Description = @Description, Manufacturer = @Manufacturer, 
                    Warranty = @Warranty, StorageLocation = @StorageLocation, PicLocation = @PicLocation 
                WHERE ModelNumber = @ModelNumber";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ItemName", txtItemName.Text);
                        command.Parameters.AddWithValue("@ItemPrice", Convert.ToDecimal(txtItemPrice.Text));
                        command.Parameters.AddWithValue("@Quantity", Convert.ToInt32(txtQuantity.Text));
                        command.Parameters.AddWithValue("@Category", cmbCategory.Text);
                        command.Parameters.AddWithValue("@Description", txtDescription.Text);
                        command.Parameters.AddWithValue("@Manufacturer", txtManufacturer.Text);
                        command.Parameters.AddWithValue("@ModelNumber", txtModelNumber.Text);
                        command.Parameters.AddWithValue("@Warranty", txtWarranty.Text);
                        command.Parameters.AddWithValue("@StorageLocation", txtStorageLocation.Text);
                        command.Parameters.AddWithValue("@PicLocation", relativePath ?? (object)DBNull.Value); // Handle null image path

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Item updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            ClearHelper.ClearTextBoxesAndComboBoxes(this);
                        }
                        else
                        {
                            MessageBox.Show("No item found with the provided Model Number.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating item: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
      
        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            LoadInventoryItem(ItemNameTextBox.Text);
        }

        private void btnUpdateItem_Click(object sender, RoutedEventArgs e)
        {
            UpdateInventoryItem();
        }

        public void ClearForm()
        {

            ClearHelper.ClearTextBoxesAndComboBoxes(this);
        }

        private void btnCancelInventroyUp_Click(object sender, RoutedEventArgs e)
        {
            App.EventAggregator.GetEvent<DialogOpenEvent>().Publish("OpenCancelDialog");
        }
    }
}
