using ModernApp.MVC.View.InventorySubviews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ModernApp.MVVM.View
{
    /// <summary>
    /// Interaction logic for InventoryView.xaml
    /// </summary>
    public partial class InventoryView : UserControl
    {
        public InventoryView()
        {
            InitializeComponent();
           
        }

     

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;
            if (parentObject is T parent) return parent;
            return FindParent<T>(parentObject);
        }

        public static T FindChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T t) return t;
                var childOfChild = FindChild<T>(child);
                if (childOfChild != null) return childOfChild;
            }
            return null;
        }



        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // Ensure fInventoryContainer is a Frame control
                if (fInventoryContainer != null)
                {
                    // Navigate to the InventoryDetailedView UserControl
                    fInventoryContainer.Navigate(new InventoryDetailedView());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }
        }

        private void btnAddItemInventory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Ensure fInventoryContainer is a Frame control
                if (fInventoryContainer != null)
                {
                    // Navigate to the InventoryDetailedView UserControl
                    fInventoryContainer.Navigate(new InventryAddSubView());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }

        }

        private void btnInventoryDetails_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Ensure fInventoryContainer is a Frame control
                if (fInventoryContainer != null)
                {
                    // Navigate to the InventoryDetailedView UserControl
                    fInventoryContainer.Navigate(new InventoryDetailedView());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }
        }

        private void btnUpdateInventory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Ensure fInventoryContainer is a Frame control
                if (fInventoryContainer != null)
                {
                    // Navigate to the InventoryDetailedView UserControl
                    fInventoryContainer.Navigate(new InventoryUpdateSubView());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }
        }

        private void btnSparePartsInventory_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    public class InventoryItem
    {
        public string ItemName { get; set; }
        public string SKU { get; set; }
        public int StockLevel { get; set; }
        public string Supplier { get; set; }
        public string Category { get; set; }
        public int ReorderLevel { get; set; }
        public string Price { get; set; }
    }
}
