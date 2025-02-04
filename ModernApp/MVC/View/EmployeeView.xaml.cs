using LiveCharts.Definitions.Series;
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
using ModernApp;
using Saliya_auto_care_Cashier.Notifications;
using ModernApp.MVC.View.InventorySubviews;
using ModernApp.MVC.View.EmployeeSubviews;
using ModernApp.MVC;
using MaterialDesignThemes.Wpf;
using Mysqlx.Notice;
using LiveCharts;
using LiveCharts.Wpf;
using MySql.Data.MySqlClient;
using System.Data;
using ModernApp.MVC.Model;

namespace ModernApp.MVVM.View
{
    /// <summary>
    /// Interaction logic for EmployeeView.xaml
    /// </summary>
    public partial class EmployeeView : UserControl
    {
        private readonly DBconnection dBconnection;
        public EmployeeView()
        {
            InitializeComponent();
            dBconnection = new DBconnection();
            DataContext = this;
            LoadDashboardData();
            LoadCharts();
            // Find the EmployeeDetails UserControl
            //EmployeeDetails employeeDetails = VisualTreeHelperExtensions.FindChild<EmployeeDetails>(this);
            //if (employeeDetails != null)
            //{
            //    // Subscribe to the EditEmployeeRequested event
            //    employeeDetails.EditEmployeeRequested += OnEditEmployeeRequested;
            //}

        }

        private void EmployeeReportDataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            
        }
        private void LoadDashboardData()
        {
            using (var connection = dBconnection.GetConnection())
            {
                connection.Open();

                // Get total employees count
                string totalQuery = "SELECT COUNT(*) FROM Employee";
                using (MySqlCommand cmd = new MySqlCommand(totalQuery, connection))
                {
                    TotalItemsTextBlock.Text = cmd.ExecuteScalar().ToString();
                }

                // Get employees on leave count
                string onLeaveQuery = "SELECT COUNT(*) FROM Employee WHERE Status='On Leave'";
                using (MySqlCommand cmd = new MySqlCommand(onLeaveQuery, connection))
                {
                    TotalOnLeaveTextBlock.Text = cmd.ExecuteScalar().ToString();
                }

                // Get new employees count (last 30 days)
                string newEmployeesQuery = "SELECT COUNT(*) FROM Employee WHERE HireDate >= CURDATE() - INTERVAL 30 DAY";
                using (MySqlCommand cmd = new MySqlCommand(newEmployeesQuery, connection))
                {
                    TotalNewEmployeesTextBlock.Text = cmd.ExecuteScalar().ToString();
                }

                // Get employees needing attention count (e.g., under probation)
                string attentionNeededQuery = "SELECT COUNT(*) FROM Employee WHERE Status='Probation'";
                using (MySqlCommand cmd = new MySqlCommand(attentionNeededQuery, connection))
                {
                    TotalAttentionNeededTextBlock.Text = cmd.ExecuteScalar().ToString();
                }
            }
        }

        private void LoadCharts()
        {
            using (var connection = dBconnection.GetConnection())
            {
                connection.Open();

                // ✅ Load Attendance Overview (Bar Chart)
                string attendanceQuery = @"
            SELECT a.Status, COUNT(a.AttendanceID) as Total 
            FROM Attendance a
            WHERE a.AttendanceDate >= CURDATE() - INTERVAL 30 DAY 
            GROUP BY a.Status";

                using (MySqlCommand cmd = new MySqlCommand(attendanceQuery, connection))
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    var labels = new List<string>();
                    var values = new ChartValues<int>();

                    while (reader.Read())
                    {
                        labels.Add(reader["Status"].ToString());  // Present, Absent, On Leave
                        values.Add(Convert.ToInt32(reader["Total"]));
                    }
                    reader.Close();

                    // ✅ Update Bar Chart
                    BarChart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Attendance",
                    Values = values,
                    Fill = System.Windows.Media.Brushes.Blue
                }
            };

                    BarChart.AxisX[0].Labels = labels;
                }

                // ✅ Load Salary Distribution (Pie Chart)
                string salaryQuery = "SELECT Position, COUNT(*) as Total FROM Employee GROUP BY Position";

                using (MySqlCommand cmd = new MySqlCommand(salaryQuery, connection))
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    var pieSeries = new SeriesCollection();

                    while (reader.Read())
                    {
                        pieSeries.Add(new PieSeries
                        {
                            Title = reader["Position"].ToString(),  // Uses Position instead of JobTitle
                            Values = new ChartValues<int> { Convert.ToInt32(reader["Total"]) }
                        });
                    }
                    reader.Close();

                    // ✅ Update Pie Chart
                    CategoryPieChart.Series = pieSeries;
                }
            }
        }



        private void PopulateEmployeeGrid(string query)
        {
            using (var connection = dBconnection.GetConnection())
            {
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                EmployeeReportDataGrid.ItemsSource = dt.DefaultView;
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string query = "SELECT * FROM Employee"; // Default query (all employees)
            PopulateEmployeeGrid(query);
        }

        private void borderOnLeave_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string query = "SELECT * FROM Employee WHERE Status='On Leave'";
            PopulateEmployeeGrid(query);
        }

        private void boarderNewEmployees_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string query = "SELECT * FROM Employee WHERE HireDate >= CURDATE() - INTERVAL 30 DAY";
            PopulateEmployeeGrid(query);
        }

        private void borderAttentionNeeded_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string query = "SELECT * FROM Employee WHERE Status='Probation'";
            PopulateEmployeeGrid(query);
        }

        //public static class VisualTreeHelperExtensions
        //{
        //    // Method to find a child control of a specific type
        //    public static T FindChild<T>(DependencyObject parent) where T : DependencyObject
        //    {
        //        if (parent == null) return null;

        //        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        //        {
        //            var child = VisualTreeHelper.GetChild(parent, i);
        //            if (child is T t) return t;

        //            // Recursively search for the child in the tree
        //            var childOfChild = FindChild<T>(child);
        //            if (childOfChild != null) return childOfChild;
        //        }

        //        return null;
        //    }
        //}

        //public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        //{
        //    DependencyObject parentObject = VisualTreeHelper.GetParent(child);
        //    if (parentObject == null) return null;
        //    if (parentObject is T parent) return parent;
        //    return FindParent<T>(parentObject);
        //}

        //public static T FindChild<T>(DependencyObject parent) where T : DependencyObject
        //{
        //    if (parent == null) return null;
        //    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        //    {
        //        var child = VisualTreeHelper.GetChild(parent, i);
        //        if (child is T t) return t;
        //        var childOfChild = FindChild<T>(child);
        //        if (childOfChild != null) return childOfChild;
        //    }
        //    return null;
        //}




        private int currentNewEmployees = 500;

 

       



        //Change colors in the mployeeCount accordign to increment and decrement 
        private void UpdateEmployeeCount(int newCount)
        {
            // Determine if the value is increasing or decreasing
            //var isIncreasing = newCount > currentNewEmployees;

            //// Update the TextBlock's value with the correct sign
            //totalNewEmployeesBlock.Text = (isIncreasing ? "+" : "-") + newCount;

            //// Change color based on the change
            //totalNewEmployeesBlock.Foreground = new SolidColorBrush(isIncreasing ? Colors.LightGreen : Colors.Red);

            //// Update the current value
            //currentNewEmployees = newCount;
        }


      

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //Notificationbox.ShowSuccess();
            try
            {
                // Ensure fInventoryContainer is a Frame control
                if (fEmployeeDetailsContainer != null)
                {
                    // Navigate to the InventoryDetailedView UserControl
                    fEmployeeDetailsContainer.Navigate(new AddNewEmployeeView());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }



        }

        private void btnEmployeeDetails_Click(object sender, RoutedEventArgs e)
        {
           
            try
            {
                // Ensure fInventoryContainer is a Frame control
                if (fEmployeeDetailsContainer != null)
                {
                    // Navigate to the InventoryDetailedView UserControl
                    fEmployeeDetailsContainer.Navigate(new EmployeeDetails());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Ensure fInventoryContainer is a Frame control
                if (fEmployeeDetailsContainer != null)
                {
                    // Navigate to the InventoryDetailedView UserControl
                    fEmployeeDetailsContainer.Navigate(new AttendenceSubView());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }
        }

        private void fEmployeeDetailsContainer_Navigated(object sender, NavigationEventArgs e)
        {

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

        public void OpenEmployeeDetailsView()
        {

            MessageBox.Show("OpeloyeViwe");

            fEmployeeDetailsContainer.Navigate(new EmployeeDetails());
        }



        //public void OpenEditEmployeeView(Employee employee)
        //{

        //    MessageBox.Show("OpeneditEmployeViwe");

        //    fEmployeeDetailsContainer.Navigate(new AddNewEmployeeView());

        //    MessageBox.Show("OpeneditEmployeViwe");
        //    var editEmployeeView = new EditEmployeeView();
        //    editEmployeeView.LoadEmployeeData(employee);

        //    // Assuming MyFrame is the navigation frame
        //    fEmployeeEditContainer.Content = editEmployeeView;
        //    editEmployeeView.LoadEmployeeData(null);
        //}

    }
}
