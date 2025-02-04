using ModernApp.Core;
using ModernApp.MVC.Controller;
using ModernApp.MVC.Model;
using MySql.Data.MySqlClient;
using Saliya_auto_care_Cashier.Notifications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ModernApp.MVC.View.EmployeeSubviews
{
    /// <summary>
    /// Interaction logic for AttendenceSubView.xaml
    /// </summary>
    public partial class AttendenceSubView : UserControl
    {
        public ObservableCollection<AttendenceTableData> EmployeeAttendenceList { get; set; } = new ObservableCollection<AttendenceTableData>();
        private readonly DBconnection dbConnection; // Use the preloaded DB connection
        private int totalEmployeeCount;

        public AttendenceSubView()
        {
            InitializeComponent();
            dbConnection = new DBconnection();  // Initialize the connection
            DataContext = this;
            LoadAttendanceData();
        }
        public void CountTotalEmployees()
        {
            int totalEmployeeCount = AttendenceDataGrid.Items.Count;
            TotalEmployeeLabel.Content = $"Employee no: {totalEmployeeCount}";
        }

        private void btnFilterApply_Click(object sender, RoutedEventArgs e)
        {
            // Get the filter values from the UI controls
            string employeeNameFilter = EmployeeNameTextBox.Text?.Trim();
            string selectedJobTitle = (JobTitleComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            string selectedStatus = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            DateTime? startDate = StartDatePicker.SelectedDate;
            DateTime? endDate = EndDatePicker.SelectedDate;

            TimeSpan? selectedCheckInTime = CheckInTimepicker.SelectedTime?.TimeOfDay;
            TimeSpan? selectedCheckOutTime = CheckOutTimepicker.SelectedTime?.TimeOfDay;

            // Filter the attendance data
            var filteredData = EmployeeAttendenceList.Where(attendance =>
                // Filter by employee name
                (string.IsNullOrEmpty(employeeNameFilter) ||
                (!string.IsNullOrEmpty(attendance.Name) && attendance.Name.IndexOf(employeeNameFilter, StringComparison.OrdinalIgnoreCase) >= 0)) &&

                // Filter by job title
                (string.IsNullOrEmpty(selectedJobTitle) || attendance.Position == selectedJobTitle) &&

                // Filter by status
                (string.IsNullOrEmpty(selectedStatus) || attendance.Status == selectedStatus) &&

                // Filter by attendance date range
                (!startDate.HasValue || attendance.AttendanceDate >= startDate.Value) &&
                (!endDate.HasValue || attendance.AttendanceDate <= endDate.Value) &&

                // Filter by Check-In and Check-Out times
                (!selectedCheckInTime.HasValue || (attendance.CheckInTime.HasValue && attendance.CheckInTime.Value >= selectedCheckInTime.Value)) &&
                (!selectedCheckOutTime.HasValue || (attendance.CheckOutTime.HasValue && attendance.CheckOutTime.Value <= selectedCheckOutTime.Value))
            ).ToList();

            // Update DataGrid view with filtered data
            AttendenceDataGrid.ItemsSource = filteredData;

            // Update total employee count
            CountTotalEmployees();
        }

        private void btnclear_Click(object sender, RoutedEventArgs e)
        {
            // Clear the filters
            EmployeeNameTextBox.Clear();
            JobTitleComboBox.SelectedIndex = -1;
            StatusComboBox.SelectedIndex = -1;
            StartDatePicker.SelectedDate = null;
            EndDatePicker.SelectedDate = null;
            CheckInTimepicker.SelectedTime = null;
            CheckOutTimepicker.SelectedTime = null;

            // Restore the full data set
            AttendenceDataGrid.ItemsSource = EmployeeAttendenceList;

            // Update the employee count
            CountTotalEmployees();
        }

        // Ensure smooth scrolling inside the DataGrid
        private void AttendenceReportDataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollViewer = GetParentScrollViewer((DependencyObject)sender);
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta / 3.0);
                e.Handled = true;
            }
        }

        private ScrollViewer GetParentScrollViewer(DependencyObject child)
        {
            while (child != null && !(child is ScrollViewer))
            {
                child = VisualTreeHelper.GetParent(child);
            }
            return child as ScrollViewer;
        }

        // Navigate back to the dashboard
        private void btnBackDashboard_Click(object sender, RoutedEventArgs e)
        {
            var parentFrame = FindParent<Frame>(this);
            if (parentFrame != null)
            {
                parentFrame.Content = null;
            }
        }

        // Helper function to find parent Frame
        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            return parentObject == null ? null : parentObject as T ?? FindParent<T>(parentObject);
        }


        private void LoadAttendanceData()
        {
            try
            {
                using (var connection = dbConnection.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        SELECT 
                            a.AttendanceID, a.EmployeeID, e.Name, e.Position, e.Mail,
                            a.Status, a.AttendanceDate, a.CheckInTime, a.CheckOutTime
                        FROM Attendance a
                        JOIN Employee e ON a.EmployeeID = e.EmployeeID";

                    using (var cmd = new MySqlCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        EmployeeAttendenceList.Clear();
                        while (reader.Read())
                        {
                            EmployeeAttendenceList.Add(new AttendenceTableData
                            {
                                AttendanceID = reader.GetInt32("AttendanceID"),
                                EmployeeID = reader.GetInt32("EmployeeID"),
                                Name = reader.GetString("Name"),
                                Mail = reader.GetString("Mail"),
                                Position = reader.GetString("Position"),
                                Status = reader.GetString("Status"),
                                AttendanceDate = reader.GetDateTime("AttendanceDate"),
                                CheckInTime = reader.IsDBNull(reader.GetOrdinal("CheckInTime")) ? (TimeSpan?)null : reader.GetTimeSpan("CheckInTime"),
                                CheckOutTime = reader.IsDBNull(reader.GetOrdinal("CheckOutTime")) ? (TimeSpan?)null : reader.GetTimeSpan("CheckOutTime"),
                            });
                        }
                    }
                }
                AttendenceDataGrid.ItemsSource = EmployeeAttendenceList;
                UpdateEmployeeCount();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading attendance data: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private List<AttendenceTableData> GetEmployeesWithThreeDaysAbsence()
        {
            var absentEmployees = new List<AttendenceTableData>();
            var groupedByEmployee = EmployeeAttendenceList.GroupBy(e => e.EmployeeID);

            foreach (var group in groupedByEmployee)
            {
                var orderedAttendance = group.OrderBy(e => e.AttendanceDate).ToList(); // FIXED ORDER

                for (int i = 0; i <= orderedAttendance.Count - 3; i++)
                {
                    if (orderedAttendance[i].Status.Equals("Absent", StringComparison.OrdinalIgnoreCase) &&
                        orderedAttendance[i + 1].Status.Equals("Absent", StringComparison.OrdinalIgnoreCase) &&
                        orderedAttendance[i + 2].Status.Equals("Absent", StringComparison.OrdinalIgnoreCase))
                    {
                        absentEmployees.Add(group.First()); // Add only once per employee
                        break;
                    }
                }
            }

            return absentEmployees;
        }



        private async void btnSendMail_Click(object sender, RoutedEventArgs e)
        {
           

            var absentEmployees = GetEmployeesWithThreeDaysAbsence();

            if (absentEmployees.Count == 0)
            {
                MessageBox.Show("No employees found with 3 consecutive days of absence.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            foreach (var employee in absentEmployees)
            {
                bool emailSent = await MailSender.SendEmailAsync(employee.Mail, employee.Name);

                if (emailSent)
                    MessageBox.Show($"Email sent to {employee.Name} ({employee.Mail})", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show($"Failed to send email to {employee.Name} ({employee.Mail})", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void UpdateEmployeeCount()
        {
            totalEmployeeCount = EmployeeAttendenceList.Count;
            TotalEmployeeLabel.Content = $"Total Employees: {totalEmployeeCount}";
        }

        

        private void btnclear_Click_1(object sender, RoutedEventArgs e)
        {
            ClearHelper.ClearTextBoxesAndComboBoxes(this);
            AttendenceDataGrid.ItemsSource = EmployeeAttendenceList;
            UpdateEmployeeCount();
        }
    }

    public class AttendenceTableData
    {
        public int AttendanceID { get; set; }
        public int EmployeeID { get; set; }
        public string Name { get; set; }
        public string Mail { get; set; }
        public string Position { get; set; }
        public string Status { get; set; }
        public DateTime AttendanceDate { get; set; }
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
    }
}
