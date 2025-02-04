using ModernApp.MVC.View.AttendenceSubViews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using LiveCharts;
using LiveCharts.Wpf;
using MySql.Data.MySqlClient;
using System.Runtime.CompilerServices;
using ModernApp.MVC.Model;

namespace ModernApp.MVVM.View
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl, INotifyPropertyChanged
    {
        private readonly DBconnection dBconnection;
        private string _dailySales;
        private string _monthlyRevenue;
        private string _attendanceRate;
        private string _stockAlerts;
        private SeriesCollection _salesChartData;
        private ObservableCollection<string> _salesLabels = new ObservableCollection<string>();
        private SeriesCollection _productSalesChartData;
        private DispatcherTimer _timer;

        public string DailySales
        {
            get => _dailySales;
            set { _dailySales = value; OnPropertyChanged(); }
        }

        public string MonthlyRevenue
        {
            get => _monthlyRevenue;
            set { _monthlyRevenue = value; OnPropertyChanged(); }
        }

        public string AttendanceRate
        {
            get => _attendanceRate;
            set { _attendanceRate = value; OnPropertyChanged(); }
        }

        public string StockAlerts
        {
            get => _stockAlerts;
            set { _stockAlerts = value; OnPropertyChanged(); }
        }

        public SeriesCollection SalesChartData
        {
            get => _salesChartData;
            set { _salesChartData = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> SalesLabels
        {
            get => _salesLabels;
            set { _salesLabels = value; OnPropertyChanged(); }
        }

        public SeriesCollection ProductSalesChartData
        {
            get => _productSalesChartData;
            set { _productSalesChartData = value; OnPropertyChanged(); }
        }

        public DashboardView()
        {
            InitializeComponent();
            dBconnection = new DBconnection();
            DataContext = this;
            LoadDashboardData();
            LoadSalesChartData();
            LoadServiceSalesData();
            InitializePeriodicUpdate();
        }

        private void InitializePeriodicUpdate()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMinutes(1); // Set the interval to 1 minute (you can adjust this)
            _timer.Tick += (sender, e) => UpdateDashboardData();
            _timer.Start();
        }

        private void UpdateDashboardData()
        {
            LoadDashboardData();        // Load the latest dashboard data
            LoadSalesChartData();       // Reload the sales chart data
            LoadServiceSalesData();     // Reload the service sales data
        }

        private void LoadDashboardData()
        {
            try
            {
                using (MySqlConnection conn = new DBconnection().GetConnection())
                {
                    conn.Open();

                    // Get Daily Sales
                    using (MySqlCommand cmd = new MySqlCommand("SELECT SUM(TotalAmount) FROM Invoice WHERE Date = CURDATE()", conn))
                    {
                        object result = cmd.ExecuteScalar();
                        DailySales = result != DBNull.Value ? $"Rs.{result}" : "Rs.0";
                    }

                    // Get Monthly Revenue
                    using (MySqlCommand cmd = new MySqlCommand("SELECT SUM(TotalAmount) FROM Invoice WHERE MONTH(Date) = MONTH(CURDATE())", conn))
                    {
                        object result = cmd.ExecuteScalar();
                        MonthlyRevenue = result != DBNull.Value ? $"Rs.{result}" : "Rs.0";
                    }

                    // Get Attendance Rate
                    using (MySqlCommand cmd = new MySqlCommand("SELECT (COUNT(*) * 100 / (SELECT COUNT(*) FROM Employee)) FROM Attendance WHERE AttendanceDate = CURDATE() AND Status = 'Present'", conn))
                    {
                        object result = cmd.ExecuteScalar();
                        AttendanceRate = result != DBNull.Value ? $"{Convert.ToInt32(result)}%" : "0%";
                    }

                    // Get Stock Alerts
                    using (MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM inventory WHERE Quantity < 5", conn))
                    {
                        object result = cmd.ExecuteScalar();
                        StockAlerts = result != DBNull.Value ? $"{result} Items Low" : "No Alerts";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dashboard data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadSalesChartData()
        {
            try
            {
                ChartValues<double> salesValues = new ChartValues<double>();
                ObservableCollection<string> dates = new ObservableCollection<string>();

                using (MySqlConnection connection = new DBconnection().GetConnection())
                {
                    connection.Open(); // Ensure connection is opened

                    using (MySqlCommand cmd = new MySqlCommand(@"
                    WITH DateRange AS (
                        SELECT CURDATE() - INTERVAL n DAY AS SaleDate
                        FROM (SELECT 0 n UNION ALL SELECT 1 UNION ALL SELECT 2 UNION ALL SELECT 3 
                              UNION ALL SELECT 4 UNION ALL SELECT 5 UNION ALL SELECT 6) AS numbers
                    )
                    SELECT 
                        d.SaleDate, 
                        COALESCE(SUM(i.TotalAmount), 0) AS TotalSales
                    FROM DateRange d
                    LEFT JOIN Invoice i ON DATE(i.Date) = d.SaleDate
                    GROUP BY d.SaleDate
                    ORDER BY d.SaleDate ASC;", connection))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                salesValues.Add(reader.GetDouble(1));
                                dates.Add(reader.GetDateTime(0).ToString("MMM dd")); // Example: "Jan 01"
                            }
                        }
                    }
                }

                // Update the chart data
                SalesChartData = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Sales",
                        Values = salesValues,
                        PointGeometry = DefaultGeometries.Circle,
                        PointGeometrySize = 10
                    }
                };

                SalesLabels = dates; // Update X-Axis Labels
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading sales chart data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadServiceSalesData()
        {
            try
            {
                SeriesCollection barChartData = new SeriesCollection();
                Dictionary<string, int> paintServiceSales = new Dictionary<string, int>();
                Dictionary<string, int> serviceSales = new Dictionary<string, int>();
                Dictionary<string, int> repairSales = new Dictionary<string, int>();

                using (MySqlConnection connection = new DBconnection().GetConnection())
                {
                    connection.Open();

                    // Step 1: Retrieve all service names from each category
                    HashSet<string> paintServices = GetServiceNames("paintservices", connection);
                    HashSet<string> services = GetServiceNames("services", connection);
                    HashSet<string> repairs = GetServiceNames("repairs", connection);

                    // Step 2: Get invoice data where description matches any service name
                    string query = @"
                SELECT id.Description, SUM(id.Quantity) AS TotalQuantity
                FROM InvoiceDetails id
                JOIN Invoice i ON id.InvoiceID = i.InvoiceID
                WHERE MONTH(i.Date) = MONTH(CURRENT_DATE)
                GROUP BY id.Description
                ORDER BY TotalQuantity DESC;";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string description = reader.GetString(0);
                                int quantity = reader.GetInt32(1);

                                // Categorize based on service tables
                                if (paintServices.Contains(description))
                                    paintServiceSales[description] = quantity;
                                else if (services.Contains(description))
                                    serviceSales[description] = quantity;
                                else if (repairs.Contains(description))
                                    repairSales[description] = quantity;
                            }
                        }
                    }
                }

                // Step 3: Populate the Bar Chart
                barChartData.Add(new ColumnSeries
                {
                    Title = "Paint Services",
                    Values = new ChartValues<int> { paintServiceSales.Values.Count > 0 ? paintServiceSales.Values.Sum() : 0 },
                    Fill = new SolidColorBrush(Colors.Blue)
                });

                barChartData.Add(new ColumnSeries
                {
                    Title = "Services",
                    Values = new ChartValues<int> { serviceSales.Values.Count > 0 ? serviceSales.Values.Sum() : 0 },
                    Fill = new SolidColorBrush(Colors.Green)
                });

                barChartData.Add(new ColumnSeries
                {
                    Title = "Repairs",
                    Values = new ChartValues<int> { repairSales.Values.Count > 0 ? repairSales.Values.Sum() : 0 },
                    Fill = new SolidColorBrush(Colors.Red)
                });

                // Step 4: Update the chart data
                ProductSalesChartData = barChartData;
                OnPropertyChanged(nameof(ProductSalesChartData));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading service sales data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Helper method to fetch service names from a given table
        private HashSet<string> GetServiceNames(string tableName, MySqlConnection connection)
        {
            HashSet<string> serviceNames = new HashSet<string>();
            string query = $"SELECT name FROM {tableName};";

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        serviceNames.Add(reader.GetString(0));
                    }
                }
            }
            return serviceNames;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void btnMarkAttendence_Click(object sender, RoutedEventArgs e)
        {
            MarkAttendece m1 = new MarkAttendece();
            m1.Show();
        }

        public void Window_Closing(object sender, RoutedEventArgs e)
        {
            if (_timer != null)
            {
                MessageBox.Show("Timer stopped method");
                _timer.Stop();
                _timer = null;
            }
        }

        public void stoptimer()
        {
            if (_timer != null)
            {
                MessageBox.Show("Timer stopped method");
                _timer.Stop();
                _timer = null;
            }
        }


    }
}