using ModernApp.MVC.View.InventorySubviews;
using ModernApp.MVC.View.ServicesSubviews;
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
using LiveCharts;
using LiveCharts.Wpf;
using MySql.Data.MySqlClient;
using ModernApp.MVC.Model;

namespace ModernApp.MVC.View
{
    /// <summary>
    /// Interaction logic for ServicesView.xaml
    /// </summary>
    public partial class ServicesView : UserControl
    {
        private readonly DBconnection dBconnection;
        public ServicesView()
        {
            InitializeComponent();
            dBconnection = new DBconnection();
            LoadData();
        }

        private void btnAddServices_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Ensure fInventoryContainer is a Frame control
                if (fInventoryContainer != null)
                {
                    // Navigate to the InventoryDetailedView UserControl
                    fInventoryContainer.Navigate(new AddServicesSubViews());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }
        }

        private void btnViewServices_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Ensure fInventoryContainer is a Frame control
                if (fInventoryContainer != null)
                {
                    // Navigate to the InventoryDetailedView UserControl
                    fInventoryContainer.Navigate(new viewServicesDetailsSubViews());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }
        }

        private void btnUpdateServices_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Ensure fInventoryContainer is a Frame control
                if (fInventoryContainer != null)
                {
                    // Navigate to the InventoryDetailedView UserControl
                    fInventoryContainer.Navigate(new UpdateServicesSubviews());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }
        }

        private void LoadData()
        {
            LoadServiceCounts();
            LoadMonthlyUsage();
            LoadServiceDistribution();
        }

        private void LoadServiceCounts()
        {
            using (var connection = dBconnection.GetConnection()) // Use existing DB connection
            {
                connection.Open();

                TotalPaintTextBlock.Text = GetServiceCount(connection, "paintservices").ToString();
                TotalVehicalTextBlock.Text = GetServiceCount(connection, "services").ToString();
                TotalRepaiTextBlock.Text = GetServiceCount(connection, "repairs").ToString();
            }
        }

        private int GetServiceCount(MySqlConnection conn, string tableName)
        {
            using (MySqlCommand cmd = new MySqlCommand($"SELECT COUNT(*) FROM {tableName}", conn))
            {
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private void LoadMonthlyUsage()
        {
            Dictionary<string, int> monthlyUsage = new Dictionary<string, int>();

            using (var connection = dBconnection.GetConnection()) // Use existing DB connection
            {
                connection.Open();
                string query = @"
                    SELECT DATE_FORMAT(i.Date, '%Y-%m') AS Month, COUNT(id.DetailID) AS Count 
                    FROM InvoiceDetails id
                    JOIN Invoice i ON id.InvoiceID = i.InvoiceID
                    GROUP BY Month
                    ORDER BY Month ASC";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string month = reader.GetString("Month");
                        int count = reader.GetInt32("Count");
                        monthlyUsage[month] = count;
                    }
                }
            }

            BarChart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Services Used",
                    Values = new ChartValues<int>(monthlyUsage.Values)
                }
            };

            BarChart.AxisX[0].Labels = monthlyUsage.Keys.ToList();
        }

        private void LoadServiceDistribution()
        {
            int paintCount = 0, serviceCount = 0, repairCount = 0;

            using (var connection = dBconnection.GetConnection()) // Use existing DB connection
            {
                connection.Open();

                paintCount = GetServiceCount(connection, "paintservices");
                serviceCount = GetServiceCount(connection, "services");
                repairCount = GetServiceCount(connection, "repairs");
            }

            // Calculate differences
            int diffPaintService = Math.Abs(paintCount - serviceCount);
            int diffServiceRepair = Math.Abs(serviceCount - repairCount);
            int diffPaintRepair = Math.Abs(paintCount - repairCount);

            CategoryPieChart.Series = new SeriesCollection
    {
        new PieSeries
        {
            Title = "Paint vs Services",
            Values = new ChartValues<int> { diffPaintService },
            Fill = System.Windows.Media.Brushes.Blue,
            DataLabels = true
        },
        new PieSeries
        {
            Title = "Services vs Repairs",
            Values = new ChartValues<int> { diffServiceRepair },
            Fill = System.Windows.Media.Brushes.Green,
            DataLabels = true
        },
        new PieSeries
        {
            Title = "Paint vs Repairs",
            Values = new ChartValues<int> { diffPaintRepair },
            Fill = System.Windows.Media.Brushes.Orange,
            DataLabels = true
        }
    };
        }

    }
}
