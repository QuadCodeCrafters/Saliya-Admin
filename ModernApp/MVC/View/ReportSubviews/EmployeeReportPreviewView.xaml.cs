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

namespace ModernApp.MVC.View.ReportSubviews
{
    /// <summary>
    /// Interaction logic for EmployeeReportPreviewView.xaml
    /// </summary>
    public partial class EmployeeReportPreviewView : UserControl
    {


        public DateTime CurrentDate { get; set; } = DateTime.Now;
        public ObservableCollection<EmployeeData> employees { get; set; }

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
        }

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
