using ModernApp.Core;
using ModernApp.MVC.Controller;
using ModernApp.MVC.View.EmployeeSubviews;
using Saliya_auto_care_Cashier.Notifications;
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

namespace ModernApp.MVC.View.ReportSubviews
{
    /// <summary>
    /// Interaction logic for AttendenceReportPreviewView.xaml
    /// </summary>
    public partial class AttendenceReportPreviewView : UserControl
    {

        public DateTime CurrentDate { get; set; } = DateTime.Now;
        public ObservableCollection<AttendenceData> attendence { get; set; }
        int totalEmployeeCount;
        private ReportController reportController;
        private ObservableObject dataProvider;
        public ObservableCollection<AttendenceData> AttendenceData { get; private set; }


        public AttendenceReportPreviewView()
        {
            InitializeComponent();
            DataContext = this;
            reportController = new ReportController();

            dataProvider = new ObservableObject();
            attendence = dataProvider.GetAttendenceSampleData();
            // Load sample data
            //Sales = GetSampleData();
            AttendenceReportDataGrid.ItemsSource = attendence;
        }


        public void CounttotalEmployees()
        {
            totalEmployeeCount = AttendenceReportDataGrid.Items.Count;
            TotalEmployeeLabel.Content = $"Employee no: {totalEmployeeCount}";
        }

        private void AttendenceReportDataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
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

            CheckInTimepicker.SelectedTime = null;
            CheckOutTimepicker.SelectedTime = null;

            AttendenceReportDataGrid.ItemsSource = attendence;
        }

        private void btnFilterApply_Click(object sender, RoutedEventArgs e)
        {
            // Get the filter values from the UI controls
            string employeeNameFilter = EmployeeNameTextBox.Text?.Trim();
            var selectedJobTitleItem = JobTitleComboBox.SelectedItem as ComboBoxItem;
            string selectedJobTitle = selectedJobTitleItem?.Content.ToString();
            var selectedStatusItem = StatusComboBox.SelectedItem as ComboBoxItem;
            string selectedStatus = selectedStatusItem?.Content.ToString();
            DateTime? startDate = StartDatePicker.SelectedDate;
            DateTime? endDate = EndDatePicker.SelectedDate;
            // Convert the SelectedTime (DateTime?) to TimeSpan?
            TimeSpan? selectedCheckInTime = CheckInTimepicker.SelectedTime.HasValue
                ? CheckInTimepicker.SelectedTime.Value.TimeOfDay
                : (TimeSpan?)null;

            TimeSpan? selectedCheckOutTime = CheckOutTimepicker.SelectedTime.HasValue
                ? CheckOutTimepicker.SelectedTime.Value.TimeOfDay
                : (TimeSpan?)null;


            // Filter the attendance data
            var filteredData = attendence.Where(attendance =>
                // Filter by employee name
                (string.IsNullOrEmpty(employeeNameFilter) ||
                (!string.IsNullOrEmpty(attendance.Name) && attendance.Name.IndexOf(employeeNameFilter, StringComparison.OrdinalIgnoreCase) >= 0)) &&

                // Filter by job title
                (string.IsNullOrEmpty(selectedJobTitle) || attendance.Position == selectedJobTitle) &&

                // Filter by status
                (string.IsNullOrEmpty(selectedStatus) || attendance.Status == selectedStatus) &&

                // Filter for a specific date if startDate is provided
                (!startDate.HasValue || attendance.AttendanceDate == startDate.Value) &&

                // Filter by attendance date range
                (!startDate.HasValue || !endDate.HasValue || (attendance.AttendanceDate >= startDate.Value && attendance.AttendanceDate <= endDate.Value)) &&

                // Filter where CheckInTime falls within the range
                (!selectedCheckInTime.HasValue || !selectedCheckOutTime.HasValue ||
                (attendance.CheckInTime.HasValue && attendance.CheckInTime.Value >= selectedCheckInTime.Value && attendance.CheckInTime.Value <= selectedCheckOutTime.Value)) &&

                // Filter where CheckOutTime falls within the range
                (!selectedCheckInTime.HasValue || !selectedCheckOutTime.HasValue ||
                (attendance.CheckOutTime.HasValue && attendance.CheckOutTime.Value >= selectedCheckInTime.Value && attendance.CheckOutTime.Value <= selectedCheckOutTime.Value)))
                .ToList();

            // Update the DataGrid view without modifying the original collection
            AttendenceReportDataGrid.ItemsSource = filteredData;

            // Update the total attendance count
            totalEmployeeCount = filteredData.Count();
            TotalEmployeeLabel.Content = $"Employee no: {totalEmployeeCount}";
        }

        private void btnPrintPdf_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.IsEnabled = false;

                // Generate PDF file path
                string pdfFilePath = "AttendanceReport_WithHeaderFooter.pdf"; // Update with the desired path

                // Get the attendance data from the DataGrid
                List<AttendenceData> attendanceDataList = ((IEnumerable<AttendenceData>)AttendenceReportDataGrid.ItemsSource).ToList();

                // Get the filter values from the UI controls
                string employeeNameFilter = EmployeeNameTextBox.Text?.Trim();
                var selectedJobTitleItem = JobTitleComboBox.SelectedItem as ComboBoxItem;
                string selectedJobTitle = selectedJobTitleItem?.Content.ToString();
                var selectedStatusItem = StatusComboBox.SelectedItem as ComboBoxItem;
                string selectedStatus = selectedStatusItem?.Content.ToString();
                DateTime? startDate = StartDatePicker.SelectedDate;
                DateTime? endDate = EndDatePicker.SelectedDate;

                // Convert the SelectedTime (DateTime?) to TimeSpan?
                TimeSpan? selectedCheckInTime = CheckInTimepicker.SelectedTime.HasValue
                    ? CheckInTimepicker.SelectedTime.Value.TimeOfDay
                    : (TimeSpan?)null;

                TimeSpan? selectedCheckOutTime = CheckOutTimepicker.SelectedTime.HasValue
                    ? CheckOutTimepicker.SelectedTime.Value.TimeOfDay
                    : (TimeSpan?)null;

                // Get the total attendance record count
                int totalAttendanceRecords = attendanceDataList.Count;

                // Call the method to generate and print the attendance report
                reportController.GenerateAttendancePrintableDocument(
                    attendanceDataList,
                    pdfFilePath,
                    AttendenceReportDataGrid,
                    totalAttendanceRecords,
                    employeeNameFilter,
                    selectedJobTitle,
                    selectedStatus,
                    startDate,
                    endDate,
                    selectedCheckInTime,
                    selectedCheckOutTime
                );

                // Show the print dialog for the generated PDF
                reportController.PrintPdf(pdfFilePath);

                MessageBox.Show("Attendance report printed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void btnExportPdf_Click(object sender, RoutedEventArgs e)
        {
            // Get the attendance data from the DataGrid (assuming each row contains AttendanceData objects)
            // Get the sales data from the DataGrid (assuming each row contains EmployeeData objects)
            List<AttendenceData> attendanceDataList = new List<AttendenceData>();

            foreach (var row in AttendenceReportDataGrid.Items)
            {
                if (row is AttendenceData attendence)
                {
                    attendanceDataList.Add(attendence);
                }
            }


            // Get the filter values from the UI controls
            string employeeNameFilter = EmployeeNameTextBox.Text?.Trim();
            var selectedJobTitleItem = JobTitleComboBox.SelectedItem as ComboBoxItem;
            string selectedJobTitle = selectedJobTitleItem?.Content.ToString();
            var selectedStatusItem = StatusComboBox.SelectedItem as ComboBoxItem;
            string selectedStatus = selectedStatusItem?.Content.ToString();
            DateTime? startDate = StartDatePicker.SelectedDate;
            DateTime? endDate = EndDatePicker.SelectedDate;

            // Convert the SelectedTime (DateTime?) to TimeSpan?
            TimeSpan? selectedCheckInTime = CheckInTimepicker.SelectedTime.HasValue
                ? CheckInTimepicker.SelectedTime.Value.TimeOfDay
                : (TimeSpan?)null;

            TimeSpan? selectedCheckOutTime = CheckOutTimepicker.SelectedTime.HasValue
                ? CheckOutTimepicker.SelectedTime.Value.TimeOfDay
                : (TimeSpan?)null;

            // Call the ExportPdfWithHeaderFooter method with the attendance data, date range, and filters
            reportController.ExportPdfAttendanceReport(
                attendanceDataList,
                AttendenceReportDataGrid,
                totalEmployeeCount,
                employeeNameFilter,
                selectedJobTitle,
                selectedStatus,
                startDate,
                endDate,
                selectedCheckInTime,
                selectedCheckOutTime
            );
        }

    }

    public class AttendenceData
    {
        public int AttendanceID { get; set; }
        public int EmployeeID { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string Status { get; set; }
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
    }
}
