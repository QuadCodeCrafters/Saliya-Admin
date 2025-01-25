using ModernApp.MVVM.ViewModel;
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
using System.IO;
using Microsoft.Win32;
using static System.Net.Mime.MediaTypeNames;

namespace ModernApp.MVVM.View
{
    public partial class inventoryAddItemView : Page
    {
        private readonly inventoryViewModel _viewModel;
        private byte[] _selectedImage; // Store the uploaded image data

        public inventoryAddItemView()
        {
            InitializeComponent();
            _viewModel = new inventoryViewModel();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string itemId = txtItemID.Text;
            string itemName = txtItemName.Text;
            string type = txtType.Text;
            string model = txtModel.Text;
            string special = txtSpecial.Text;
            string supplierName = txtSupplierName.Text;
            string address = txtAddress.Text;
            string email = txtEmail.Text;
            string number = txtNumber.Text;

            if (string.IsNullOrWhiteSpace(itemId) ||
                string.IsNullOrWhiteSpace(itemName) ||
                !int.TryParse(txtQTY.Text, out int qty) || qty <= 0 ||
                string.IsNullOrWhiteSpace(type) ||
                string.IsNullOrWhiteSpace(model) ||
                string.IsNullOrWhiteSpace(special) ||
                string.IsNullOrWhiteSpace(supplierName) ||
                string.IsNullOrWhiteSpace(address) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(number) || !number.All(char.IsDigit))
            {
                MessageBox.Show("One or more fields are invalid. Please check your inputs.");
                return;
            }

            if (!email.Contains("@"))
            {
                MessageBox.Show("'@' is missing from your email.");
                return;
            }

            // Check if an image is uploaded
            if (_selectedImage == null)
            {
                MessageBox.Show("Please upload an image.");
                return;
            }

            // Call ViewModel to handle the operation
            _viewModel.AddItem(itemId, itemName, qty, type, model, special, supplierName, address, email, number, _selectedImage);

            // Optionally show a success message
            MessageBox.Show("Item added successfully!");

            // Clear the fields
            txtItemID.Clear();
            txtItemName.Clear();
            txtQTY.Clear();
            txtType.Clear();
            txtModel.Clear();
            txtSpecial.Clear();
            txtSupplierName.Clear();
            txtAddress.Clear();
            txtEmail.Clear();
            txtNumber.Clear();

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri("/images/imageBox.png", UriKind.RelativeOrAbsolute);
            bitmapImage.EndInit();

            // Set the Image control's Source property
            imgItem.Source = bitmapImage;
        }

        private void btnAddImage_Click(object sender, RoutedEventArgs e)
        {
           
        }
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                // Convert image to byte array
                _selectedImage = File.ReadAllBytes(filePath);

                // Display the image in the Image control
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(filePath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                imgItem.Source = bitmap;

                MessageBox.Show("Image added successfully!");
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtItemID.Clear();
            txtItemName.Clear();
            txtQTY.Clear();
            txtType.Clear();
            txtModel.Clear();
            txtSpecial.Clear();
            txtSupplierName.Clear();
            txtAddress.Clear();
            txtEmail.Clear();
            txtNumber.Clear();

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri("/images/imageBox.png", UriKind.RelativeOrAbsolute);
            bitmapImage.EndInit();

            // Set the Image control's Source property
            imgItem.Source = bitmapImage;

        }
    }
}