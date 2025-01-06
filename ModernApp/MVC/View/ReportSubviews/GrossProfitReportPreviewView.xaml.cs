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

namespace ModernApp.MVC.View.ReportSubviews
{
    /// <summary>
    /// Interaction logic for GrossProfitReportPreviewView.xaml
    /// </summary>
    public partial class GrossProfitReportPreviewView : UserControl
    {
        public SeriesCollection ChartSeries { get; set; }
        public Func<double, string> Formatter { get; set; }

        public GrossProfitReportPreviewView()
        {
            InitializeComponent();

            ChartSeries = new SeriesCollection();
            Formatter = value => value.ToString("C");
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




        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double sales = double.Parse(SalesTextBox.Text);
                double spareParts = double.Parse(SparePartsTextBox.Text);
                double salary = double.Parse(SalaryTextBox.Text);
                double expenses = double.Parse(ExpensesTextBox.Text);

                // Calculate totals
                double totalIncome = sales + spareParts;
                double totalExpenses = salary + expenses;
                double grossProfit = totalIncome - totalExpenses;

                // Update chart data
                GrossProfitChart.Series.Clear();
                GrossProfitChart.Series.Add(new ColumnSeries
                {
                    Title = "Amounts",
                    Values = new ChartValues<double> { sales, spareParts, totalExpenses, grossProfit }
                });

                MessageBox.Show($"Gross Profit Calculated: {grossProfit:C}", "Calculation Successful");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Calculation Error");
            }
        }
    }
}
