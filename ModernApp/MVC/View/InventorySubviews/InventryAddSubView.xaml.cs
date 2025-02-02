using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using ModernApp.Events;
using ModernApp.MVC.Controller;
using ModernApp.MVC.Model;
using MySql.Data.MySqlClient;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Prism.Events;
using ModernApp.Core;
using System.ComponentModel;
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;

namespace ModernApp.MVC.View.InventorySubviews
{
    /// <summary>
    /// Interaction logic for InventryAddSubView.xaml
    /// </summary>
    public partial class InventryAddSubView : UserControl
    {
        private readonly string ImageFolderPath = "D:\\personal\\InventoryPictures";
        private string relativePath;
        private VideoCaptureDevice videoSource;
        private BarcodeReader barcodeReader;
        private readonly DBconnection dbConnection;
        public event Action<BitmapImage> ImageUpdated;

        public InventryAddSubView()
        {
            InitializeComponent();
            dbConnection = new DBconnection();
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
            string extension = Path.GetExtension(filePath)?.ToLower();
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

        public void SaveInventoryItem()
        {
            try
            {
                string itemName = txtItemName.Text;
                decimal itemPrice;
                if (!decimal.TryParse(txtItemPrice.Text, out itemPrice))
                {
                    MessageBox.Show("Please enter a valid price.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int quantityValue = string.IsNullOrWhiteSpace(txtQuantity.Text) ? 0 : Convert.ToInt32(txtQuantity.Text);
                string category = cmbCategory.SelectedItem?.ToString();
                string description = txtDescription.Text ?? string.Empty; // Ensure it's not null
                string manufacturer = txtManufacturer.Text;
                string modelNumber = txtModelNumber.Text;
                string warranty = txtWarranty.Text;
                string storageLocation = txtStorageLocation.Text;
                string imagePath = relativePath;

                if (string.IsNullOrWhiteSpace(itemName) || string.IsNullOrWhiteSpace(category))
                {
                    MessageBox.Show("Please fill in all required fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (var connection = dbConnection.GetConnection())
                {
                    connection.Open();
                    string query = @"
                INSERT INTO Inventory 
                (ItemName, ItemPrice, Quantity, Category, Description, Manufacturer, ModelNumber, Warranty, StorageLocation, PicLocation) 
                VALUES (@ItemName, @ItemPrice, @Quantity, @Category, @Description, @Manufacturer, @ModelNumber, @Warranty, @StorageLocation, @PicLocation)";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.Add("@ItemName", MySqlDbType.VarChar).Value = itemName;
                        cmd.Parameters.Add("@ItemPrice", MySqlDbType.Decimal).Value = itemPrice; // Proper decimal
                        cmd.Parameters.Add("@Quantity", MySqlDbType.Int32).Value = quantityValue;
                        cmd.Parameters.Add("@Category", MySqlDbType.VarChar).Value = category;
                        cmd.Parameters.Add("@Description", MySqlDbType.Text).Value = description; // Proper text handling
                        cmd.Parameters.Add("@Manufacturer", MySqlDbType.VarChar).Value = manufacturer;
                        cmd.Parameters.Add("@ModelNumber", MySqlDbType.VarChar).Value = modelNumber;
                        cmd.Parameters.Add("@Warranty", MySqlDbType.VarChar).Value = warranty;
                        cmd.Parameters.Add("@StorageLocation", MySqlDbType.VarChar).Value = storageLocation;
                        cmd.Parameters.Add("@PicLocation", MySqlDbType.VarChar).Value = imagePath;

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Item successfully added to inventory!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        // Clear input fields
                        //txtItemName.Clear();
                        //cmbCategory.SelectedIndex = -1; // Reset dropdown selection
                        //txtItemPrice.Clear();
                        //txtQuantity.Clear();
                        //txtDescription.Clear();
                        ClearHelper.ClearTextBoxesAndComboBoxes(this);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving item: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRegisterItem_Click(object sender, RoutedEventArgs e)
        {
            SaveInventoryItem();

        }

        private void btnCancelInventroyReg_Click(object sender, RoutedEventArgs e)
        {
            App.EventAggregator.GetEvent<DialogOpenEvent>().Publish("OpenCancelDialog");
        }


        public void ClearForm()
        {
          
            ClearHelper.ClearTextBoxesAndComboBoxes(this);
        }

    }
}
