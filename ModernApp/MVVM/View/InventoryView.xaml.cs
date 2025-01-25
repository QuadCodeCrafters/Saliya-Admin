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
using Microsoft.Win32;
using ModernApp.MVVM.ViewModel;

namespace ModernApp.MVVM.View
{

    public partial class InventoryView : UserControl
    {
        private readonly Page InventoryAddItemView; // Store the page instance
        private readonly inventoryViewModel _viewModel;
        private byte[] _selectedImage;
        string cqty = "QTY";
        public InventoryView()
        {
            InitializeComponent();
            InventoryAddItemView = new inventoryAddItemView();
            _viewModel = new inventoryViewModel();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Navigate to the inventoryAddItemView.xaml without losing state on subsequent loads
                fContainer.Navigate(new Uri("pack://application:,,,/MVVM/View/inventoryAddItemView.xaml", UriKind.Absolute));
                fContainer.Visibility = Visibility.Visible; // Ensure the frame is visible when navigating
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }
            
            txtItemID.Clear();
            txtItemName.Clear();
            txtQTY.Text = cqty;
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


            imgItem.Source = bitmapImage;
        }

        private void Inventory_Click(object sender, RoutedEventArgs e)
        {
            // Hide the frame, retaining its state
            fContainer.Visibility = Visibility.Collapsed;
        }

        private void ShowFrame_Click(object sender, RoutedEventArgs e)
        {
            // Show the frame again with the same content state
            fContainer.Visibility = Visibility.Visible;
        }

        private void fContainer_Navigated(object sender, NavigationEventArgs e)
        {

        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string itemId = txtItemID.Text;

            if (string.IsNullOrWhiteSpace(itemId))
            {
                MessageBox.Show("Please enter a valid Item ID.");
                return;
            }

            // Retrieve item details using ViewModel
            var item = _viewModel.GetItemById(itemId);

            if (item != null)
            {
                txtItemName.Text = item.itemName;
                txtQTY.Text = item.qty.ToString();
                txtType.Text = item.Type;
                txtModel.Text = item.Model;
                txtSpecial.Text = item.Special;
                txtSupplierName.Text = item.supplierName;
                txtAddress.Text = item.address;
                txtEmail.Text = item.email;
                txtNumber.Text = item.number;



                if (item.itemImage != null && item.itemImage.Length > 0)
                {
                    using (var memoryStream = new MemoryStream(item.itemImage))
                    {
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = memoryStream;
                        bitmapImage.EndInit();
                        imgItem.Source = bitmapImage;
                    }
                }
                else
                {
                    MessageBox.Show("No image found for this item or the image data is invalid.");
                }

            }
            else
            {
                MessageBox.Show("Item not found!");
            }

        }
        private void Image_MouseLeftButtonDownclick(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedImage = File.ReadAllBytes(openFileDialog.FileName);
                BitmapImage bitmap = new BitmapImage(new Uri(openFileDialog.FileName));
                imgItem.Source = bitmap;
            }

                MessageBox.Show("Image added successfully!");
            }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
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
                string.IsNullOrWhiteSpace(supplierName) ||
                string.IsNullOrWhiteSpace(address) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(number))
            {
                MessageBox.Show("One or more fields are invalid. Please check your inputs.");
                return;
            }

            try
            {
                _viewModel.UpdateItem(itemId, itemName, qty, type, model, special, supplierName, address, email, number, _selectedImage);
                MessageBox.Show("Item updated successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating item: {ex.Message}");
            }

        }

        private void btndelete_Click(object sender, RoutedEventArgs e)
        {
            string itemId = txtItemID.Text;

            if (string.IsNullOrWhiteSpace(itemId))
            {
                MessageBox.Show("Please enter a valid Item ID to delete.");
                return;
            }

            try
            {
                _viewModel.DeleteItem(itemId);
                MessageBox.Show("Item deleted successfully!");

                // Clear fields after deletion
                txtItemID.Clear();
                txtItemName.Clear();
                txtQTY.Text = cqty;
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
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting item: {ex.Message}");
            }

        }
    }
}

