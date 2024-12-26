using Saliya_auto_care_Cashier.Notifications;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernApp.MVC.View.ReportSubviews
{
    public partial class SalesReportPreviewView : UserControl
    {
        public DateTime CurrentDate { get; set; } = DateTime.Now;
        public ObservableCollection<SalesData> Sales { get; set; }

        public SalesReportPreviewView()
        {
            InitializeComponent();
            DataContext = this;

            // Load sample data
            Sales = GetSampleData();
            SalesReportDataGrid.ItemsSource = Sales;
        }

        private ObservableCollection<SalesData> GetSampleData()
        {
            return new ObservableCollection<SalesData>
            {
                new SalesData { SaleID = 1, ProductName = "Oil Change", ProductType = "Service", SalesAmount = 1500.00M, SaleDate = DateTime.Today },
                new SalesData { SaleID = 2, ProductName = "Car Wash", ProductType = "Service", SalesAmount = 800.00M, SaleDate = DateTime.Today },
                new SalesData { SaleID = 3, ProductName = "Tire Replacement", ProductType = "Product", SalesAmount = 20000.00M, SaleDate = DateTime.Today },
                new SalesData { SaleID = 4, ProductName = "Battery Replacement", ProductType = "Product", SalesAmount = 12000.00M, SaleDate = DateTime.Today },
            };
        }


      


        private void RemoveScrollViewers(DependencyObject control)
        {
            if (control is ScrollViewer scrollViewer)
            {
                // Remove ScrollViewer content
                var content = scrollViewer.Content;
                scrollViewer.Content = null;

                // Replace with direct content
                if (control is UserControl userControl)
                {
                    userControl.Content = content;
                }
            }

            // Recursively remove from child elements
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(control); i++)
            {
                RemoveScrollViewers(VisualTreeHelper.GetChild(control, i));
            }
        }

        public void ExportPdfButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Temporarily hide the footer
                HideFooter();

                PrintDialog printDialog = new PrintDialog();

                if (printDialog.ShowDialog() == true)
                {
                    // Create a copy of the UserControl for exporting
                    UserControl exportContent = new UserControl
                    {
                        Content = this.Content
                    };

                    // Measure and arrange the content
                    exportContent.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    exportContent.Arrange(new Rect(new Point(0, 0), exportContent.DesiredSize));

                    // Export the content to PDF
                    printDialog.PrintVisual(exportContent, "Saliya Auto Care Sales Report");

                    MessageBox.Show("PDF exported successfully.", "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("PDF export canceled.", "Export Canceled", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Restore the footer
                ShowFooter();

                // Reload the UserControl
                ShowAgain();
            }
        }

        public void Buttonprint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.IsEnabled = false;

                // Temporarily hide the footer
                HideFooter();

                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    // Create a copy of the UserControl for printing
                    UserControl printContent = new UserControl
                    {
                        Content = this.Content
                    };

                    // Measure and arrange the content
                    printContent.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    printContent.Arrange(new Rect(new Point(0, 0), printContent.DesiredSize));

                    // Print the content
                    printDialog.PrintVisual(printContent, "Saliya Auto Care Sales Report");

                    MessageBox.Show("Printed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Print canceled.", "Canceled", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Restore the footer
                ShowFooter();

                // Reload the UserControl
                ShowAgain();

                this.IsEnabled = true;
            }
        }

        public void ShowAgain()
        {
            Notificationbox.ShowSuccess();
            UserControl newUser = new ReportSubviews.SalesReportPreviewView();
            this.Content = newUser;
        }

        private void HideFooter()
        {
            DockPanel footer = LogicalTreeHelper.FindLogicalNode(this, "FooterDockPanel") as DockPanel;
            if (footer != null)
            {
                footer.Visibility = Visibility.Collapsed;
            }
        }

        private void ShowFooter()
        {
            DockPanel footer = LogicalTreeHelper.FindLogicalNode(this, "FooterDockPanel") as DockPanel;
            if (footer != null)
            {
                footer.Visibility = Visibility.Visible;
            }
        }
        private void ReloadUserControl()
        {
            // Find the parent container
            var parent = VisualTreeHelper.GetParent(this) as Panel;
            if (parent != null)
            {
                // Create a new instance of the UserControl
                var newControl = new SalesReportPreviewView();

                // Remove the old UserControl
                parent.Children.Remove(this);

                // Add the new UserControl in its place
                parent.Children.Add(newControl);
            }
            else
            {
                MessageBox.Show("Unable to reload the report. Please refresh the application.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




    }

    public class SalesData
    {
        public int SaleID { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public decimal SalesAmount { get; set; }
        public DateTime SaleDate { get; set; }
    }
}
