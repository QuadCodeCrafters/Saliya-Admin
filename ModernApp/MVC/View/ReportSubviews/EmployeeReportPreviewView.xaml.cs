using ModernApp.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
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
using ModernApp.MVC.Controller;
using ModernApp.MVC.View.EmployeeSubviews;
using Saliya_auto_care_Cashier.Notifications;

namespace ModernApp.MVC.View.ReportSubviews
{
    /// <summary>
    /// Interaction logic for EmployeeReportPreviewView.xaml
    /// </summary>
    public partial class EmployeeReportPreviewView : UserControl
    {


        public DateTime CurrentDate { get; set; } = DateTime.Now;
        public ObservableCollection<EmployeeData> employees { get; set; }
        int totalEmployeeCount;
        private ReportController reportController;
        private ObservableObject dataProvider;
        public ObservableCollection<EmployeeData> EmployeeData { get; private set; }

        public EmployeeReportPreviewView()
        {
            InitializeComponent();
            DataContext = this;
            reportController = new ReportController();

            dataProvider = new ObservableObject();
            employees = dataProvider.GetEmployeeSampleData();
            // Load sample data
            //Sales = GetSampleData();
            EmployeeReportDataGrid.ItemsSource = employees;
            CounttotalEmployees();
        }

        private void btnclear_Click(object sender, RoutedEventArgs e)
        {
            // Clear the TextBox
            EmployeeNameTextBox.Clear();

            // Reset the ComboBoxes to their default state
            JobTitleComboBox.SelectedIndex = -1;
            StatusComboBox.SelectedIndex = -1;


            // Clear the DatePickers
            StartDatePicker.SelectedDate = null;
            EndDatePicker.SelectedDate = null;

            EmployeeReportDataGrid.ItemsSource = employees;
            CounttotalEmployees();
        }

        //filters the data based on the user input
        private void btnApply_Click(object sender, RoutedEventArgs e)
        {

            // Get the filter values from the UI controls
            string employeeNameFilter = EmployeeNameTextBox.Text?.Trim();
            var selectedJobTitleItem = JobTitleComboBox.SelectedItem as ComboBoxItem;
            string selectedJobTitle = selectedJobTitleItem?.Content.ToString();
            var selectedStatusItem = StatusComboBox.SelectedItem as ComboBoxItem;
            string selectedStatus = selectedStatusItem?.Content.ToString();
            DateTime? startDate = StartDatePicker.SelectedDate;
            DateTime? endDate = EndDatePicker.SelectedDate;

            // Filter the data
            var filteredData = employees.Where(employee =>
              (string.IsNullOrEmpty(employeeNameFilter) ||
              (!string.IsNullOrEmpty(employee.Name) && employee.Name.IndexOf(employeeNameFilter, StringComparison.OrdinalIgnoreCase) >= 0)) &&
              (string.IsNullOrEmpty(selectedJobTitle) || employee.Position == selectedJobTitle) &&
              (string.IsNullOrEmpty(selectedStatus) || employee.Status == selectedStatus) &&
              (!startDate.HasValue || employee.HireDate >= startDate.Value) &&
              (!endDate.HasValue || employee.HireDate <= endDate.Value))
                .ToList();

            // Update the DataGrid view without modifying the original collection
            EmployeeReportDataGrid.ItemsSource = filteredData;
            // Assuming 'filteredData' is the collection of employees currently displayed in the DataGrid
            totalEmployeeCount = filteredData.Count(employee => employee.EmployeeID != null);

            // Update the label content with the total count
            TotalEmployeeLabel.Content = $"Employee no: {totalEmployeeCount}";


        }


        public void CounttotalEmployees()
        {
            totalEmployeeCount = EmployeeReportDataGrid.Items.Count;
            TotalEmployeeLabel.Content = $"Employee no: {totalEmployeeCount}";
        }

        private void EmployeeReportDataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Find the parent ScrollViewer
            var scrollViewer = GetParentScrollViewer((DependencyObject)sender);

            if (scrollViewer != null)
            {
                // Adjust vertical offset based on the mouse wheel delta
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta / 3.0);

                // Mark the event as handled to prevent default behavior
                e.Handled = true;
            }
        }

        private ScrollViewer GetParentScrollViewer(DependencyObject child)
        {
            // Traverse up the visual tree to find the ScrollViewer
            while (child != null && !(child is ScrollViewer))
            {
                child = VisualTreeHelper.GetParent(child);
            }
            return child as ScrollViewer;
        }

        private void btnBackReportDashboard_Click(object sender, RoutedEventArgs e)
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

        private void btnExportPdf_Click(object sender, RoutedEventArgs e)
        {
            // Get the sales data from the DataGrid (assuming each row contains EmployeeData objects)
            List<EmployeeData> employeeDataList = new List<EmployeeData>();

            foreach (var row in EmployeeReportDataGrid.Items)
            {
                if (row is EmployeeData employee)
                {
                    employeeDataList.Add(employee);
                }
            }

            // Get the applied filters
            string employeeNameFilter = EmployeeNameTextBox.Text?.Trim();
            var selectedJobTitleItem = JobTitleComboBox.SelectedItem as ComboBoxItem;
            string selectedJobTitle = selectedJobTitleItem?.Content.ToString();
            var selectedStatusItem = StatusComboBox.SelectedItem as ComboBoxItem;
            string selectedStatus = selectedStatusItem?.Content.ToString();
            DateTime? startDate = StartDatePicker.SelectedDate;
            DateTime? endDate = EndDatePicker.SelectedDate;

            // Call the ExportPdfWithHeaderFooter method with the sales data, date range, and filters
            reportController.ExportPdfEmployeeReport(
                employeeDataList,
                EmployeeReportDataGrid,
                totalEmployeeCount,
                employeeNameFilter,
                selectedJobTitle,
                selectedStatus,
                startDate,
                endDate
            );

        }

        private void btnPrintPdf_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.IsEnabled = false;

            

                // Generate PDF
                string pdfFilePath = "EmployeeReport_WithHeaderFooter.pdf"; // Update with the desired path

                // Get the employee data from the DataGrid (assuming EmployeeReportDataGrid is the name of your DataGrid)
                List<EmployeeData> employeeDataList = ((IEnumerable<EmployeeData>)EmployeeReportDataGrid.ItemsSource).ToList();

                // Get the filter values
                string employeeNameFilter = EmployeeNameTextBox.Text; // Assuming a TextBox for name filter
                var selectedJobTitleItem = JobTitleComboBox.SelectedItem as ComboBoxItem;
                string selectedJobTitle = selectedJobTitleItem?.Content.ToString();
                var selectedStatusItem = StatusComboBox.SelectedItem as ComboBoxItem;
                string selectedStatus = selectedStatusItem?.Content.ToString();
                DateTime? startDate = StartDatePicker.SelectedDate;
                DateTime? endDate = EndDatePicker.SelectedDate;

                // Get total employee count (you can customize this if needed)
                int totalEmployeeCount = employeeDataList.Count;

                // Call the GenerateEmployeePrintableDocument method with the employee data and filters
                reportController.GenerateEmployeePrintableDocument(
                    employeeDataList,
                    pdfFilePath,
                    EmployeeReportDataGrid,
                    totalEmployeeCount,
                    employeeNameFilter,
                    selectedJobTitle,
                    selectedStatus,
                    startDate,
                    endDate
                );

                // Show the print dialog for the generated PDF
                reportController.PrintPdf(pdfFilePath);

                MessageBox.Show("Printed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Notificationbox.ShowSuccess();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
              
                this.IsEnabled = true;
            }
        }
    }


    public class EmployeeData
    {
        public int EmployeeID { get; set; }
        public string NationalIdentificationNumber { get; set; }
        public string Name { get; set; }
        public DateTime DOB { get; set; }
        public decimal Salary { get; set; }
        public string Status { get; set; }
        public string Mail { get; set; }
        public string Position { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public DateTime HireDate { get; set; }
    }
}
