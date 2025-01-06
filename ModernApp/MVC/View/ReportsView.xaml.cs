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
    /// Interaction logic for ReportsView.xaml
    /// </summary>
    public partial class ReportsView : UserControl
    {
        public ReportsView()
        {
            InitializeComponent();
        }

        private void freportpreviewContainer_Navigated(object sender, NavigationEventArgs e)
        {

        }

       

        private void btnSalesReportGenerate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (freportpreviewContainer != null)
                {
                    freportpreviewContainer.Navigate(new Uri("pack://application:,,,/MVC/View/ReportSubviews/SalesReportPreviewView.xaml", UriKind.Absolute));
                }
                else
                {
                    MessageBox.Show("Frame container is not initialized.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }
        }

        private void btnEmployeeReportGenerate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (freportpreviewContainer != null)
                {
                    freportpreviewContainer.Navigate(new Uri("pack://application:,,,/MVC/View/ReportSubviews/EmployeeReportPreviewView.xaml", UriKind.Absolute));
                }
                else
                {
                    MessageBox.Show("Frame container is not initialized.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }
        }

     private void btnAttendenceReportGenerate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (freportpreviewContainer != null)
                {
                    freportpreviewContainer.Navigate(new Uri("pack://application:,,,/MVC/View/ReportSubviews/AttendenceReportPreviewView.xaml", UriKind.Absolute));
                }
                else
                {
                    MessageBox.Show("Frame container is not initialized.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }
        }


        private void btnGrossProfitReportGenerate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (freportpreviewContainer != null)
                {
                    freportpreviewContainer.Navigate(new Uri("pack://application:,,,/MVC/View/ReportSubviews/GrossProfitReportPreviewView.xaml", UriKind.Absolute));
                }
                else
                {
                    MessageBox.Show("Frame container is not initialized.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }
        }
    }



    
}
