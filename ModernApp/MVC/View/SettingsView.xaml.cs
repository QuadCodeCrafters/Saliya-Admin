using ModernApp.MVC.View.InventorySubviews;
using ModernApp.MVC.View.SettingsSubViews;
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
using Prism.Events;
using ModernApp.Events;

namespace ModernApp.MVVM.View
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
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

        private void btnChangeAdminPwd_Click(object sender, RoutedEventArgs e)
        {
            App.EventAggregator.GetEvent<ChangeAdminPasswordEvent>().Publish("OpenChangePasswordDialog");
        }

        private void btnManageCashiers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Ensure fInventoryContainer is a Frame control
                if (fSettingsContainer != null)
                {
                    // Navigate to the InventoryDetailedView UserControl
                    fSettingsContainer.Navigate(new ManageCashierSubView());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }
        }

        private void btnCheckPrinters_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Ensure fInventoryContainer is a Frame control
                if (fSettingsContainer != null)
                {
                    // Navigate to the InventoryDetailedView UserControl
                    fSettingsContainer.Navigate(new ViewConnectedPrintersSubView());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }
        }

        private void btnOPenWebsite_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCheckDBstatus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Ensure fInventoryContainer is a Frame control
                if (fSettingsContainer != null)
                {
                    // Navigate to the InventoryDetailedView UserControl
                    fSettingsContainer.Navigate(new DBconnectionCheckSubView());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }
        }

        private void btnLogoutAdmin_Click(object sender, RoutedEventArgs e)
        {
            App.EventAggregator.GetEvent<logoutConfirmationEvent>().Publish("OpenlogoutConfirmDialog");
        }
    }
}
