using Microsoft.Win32;
using ModernApp.MVC.Controller;
using ModernApp.MVC.Model;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ModernApp.MVC.View.InventorySubviews
{
    /// <summary>
    /// Interaction logic for InventryAddSubView.xaml
    /// </summary>
    public partial class InventryAddSubView : UserControl
    {
        private readonly string ImageFolderPath = "D:\\personal\\InventoryPictures";
        private string relativePath;
        public event Action<BitmapImage> ImageUpdated;

        public InventryAddSubView()
        {
            InitializeComponent();
            ImageUpdated += UpdateProfileImage;
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

    }
}
