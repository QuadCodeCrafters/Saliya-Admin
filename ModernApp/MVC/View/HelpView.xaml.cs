using ModernApp.MVC.View.EmployeeSubviews;
using ModernApp.MVC.View.HelpSubViews;
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

namespace ModernApp.MVVM.View
{
    /// <summary>
    /// Interaction logic for HelpView.xaml
    /// </summary>
    public partial class HelpView : UserControl
    {
        public HelpView()
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

     

        private void BtnHelpSales_Click(object sender, RoutedEventArgs e)
        {
            //Notificationbox.ShowSuccess();
            try
            {
                // Ensure fInventoryContainer is a Frame control
                if (FHelpViewContainer != null)
                {
                    // Navigate to the InventoryDetailedView UserControl
                    FHelpViewContainer.Navigate(new HelpSalesSubView());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Notificationbox.ShowSuccess();
            try
            {
                // Ensure fInventoryContainer is a Frame control
                if (FHelpViewContainer != null)
                {
                    // Navigate to the InventoryDetailedView UserControl
                    FHelpViewContainer.Navigate(new HelpInventroySubViews());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }
        }
    }
}
