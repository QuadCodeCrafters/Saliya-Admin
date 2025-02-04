using LiveCharts.Wpf;
using LiveCharts;
using ModernApp.MVC.Controller;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;
using System;
using System.Collections.Generic;
using ModernApp.MVC.View.SalesSubViews;

namespace ModernApp.MVVM.View
{
    public partial class SalesView : UserControl, INotifyPropertyChanged
    {
        private readonly SalesController _salesController;

        // Chart Data
        private ChartValues<double> _lineValues;
        private ChartValues<double> _barValues;
        private SeriesCollection _seriesCollection;

        // UI Bindings
        private string[] _xLabels;
        private string _totalSalesDisplay;
        private string _todaySalesDisplay;
        private string _graphSalesDetails;
        private string _totalCustomerDisplay;

        public ChartValues<double> LineValues
        {
            get => _lineValues;
            set { _lineValues = value; OnPropertyChanged(nameof(LineValues)); }
        }

        public ChartValues<double> BarValues
        {
            get => _barValues;
            set { _barValues = value; OnPropertyChanged(nameof(BarValues)); }
        }

        public string[] XLabels
        {
            get => _xLabels;
            set { _xLabels = value; OnPropertyChanged(nameof(XLabels)); }
        }

        public string TotalSalesDisplay
        {
            get => _totalSalesDisplay;
            set { _totalSalesDisplay = value; OnPropertyChanged(nameof(TotalSalesDisplay)); }
        }

        public string TodaySalesDisplay
        {
            get => _todaySalesDisplay;
            set { _todaySalesDisplay = value; OnPropertyChanged(nameof(TodaySalesDisplay)); }
        }

        public string GraphSalesDetails
        {
            get => _graphSalesDetails;
            set { _graphSalesDetails = value; OnPropertyChanged(nameof(GraphSalesDetails)); }
        }

        public string TotalCustomerDisplay
        {
            get => _totalCustomerDisplay;
            set { _totalCustomerDisplay = value; OnPropertyChanged(nameof(TotalCustomerDisplay)); }
        }

        public SeriesCollection SeriesCollection
        {
            get => _seriesCollection;
            set { _seriesCollection = value; OnPropertyChanged(nameof(SeriesCollection)); }
        }

        public SalesView()
        {
            InitializeComponent();
            _salesController = new SalesController(Application.Current.Dispatcher);
            _salesController.OnSalesUpdated += SalesController_OnSalesUpdated;
            DataContext = this;

            LoadSalesData();
            
        }

        private void LoadSalesData()
        {
            // Load sales metrics
            TotalSalesDisplay = $"Rs.{_salesController.GetTotalSales():N0}";
            TodaySalesDisplay = $"Rs.{_salesController.GetTodaySales():N0}";
            TotalCustomerDisplay = $"{_salesController.GetCustomersTotal():N0}";

            // Load sales chart data
            LineValues = _salesController.GetThisMonthSalesChartValues();
            BarValues = GetBarChartValues(_salesController.GetSalesSumByCategory());

            // Setup Series Collection
            SeriesCollection = new SeriesCollection
            {
                new LineSeries { Title = "Sales Trend", Values = LineValues },
                new ColumnSeries { Title = "Category Sales", Values = BarValues }
            };

            // Setup category labels
            XLabels = new[] { "Paint Jobs", "Vehicle Services", "Vehicle Repairs", "Carrier Service", "Spare Services" };

            // Load top sales data
            var salesData = _salesController.GetSalesWithProductDetails();
            TopSalesDataGrid.ItemsSource = salesData;
            txtTopProduct.Text = salesData.Count > 0 ? salesData[0].ProductName : "No data available";
        }

       


        private ChartValues<double> GetBarChartValues(Dictionary<string, decimal> salesData)
        {
            var categories = new[] { "Paint Jobs", "Vehicle Services", "Vehicle Repairs", "Carrier Service", "Spare Services" };
            var barValues = new ChartValues<double>();

            foreach (var category in categories)
                barValues.Add(salesData.TryGetValue(category, out var salesSum) ? (double)salesSum : 0);

            return barValues;
        }

        private void SalesController_OnSalesUpdated(decimal totalSales)
        {
            TotalSalesDisplay = $"${totalSales:N0}";
        }

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (GraphStateManipulateComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedRange = selectedItem.Content.ToString();
                Dictionary<string, decimal> salesData;

                switch (selectedRange)
                {
                    case "This Year":
                        GraphSalesDetails = $"Yearly Sales: ${_salesController.GetThisYearSales():N0}";
                        LineValues = _salesController.GetThisYearSalesChartValues();
                        salesData = _salesController.GetSalesSumByCategoryForCurrentYear();
                        break;

                    case "This Month":
                        GraphSalesDetails = $"Monthly Sales: ${_salesController.GetThisMonthSales():N0}";
                        LineValues = _salesController.GetThisMonthSalesChartValues();
                        salesData = _salesController.GetSalesSumByCategoryForCurrentMonth();
                        break;

                    case "Today":
                        GraphSalesDetails = $"Today's Sales: ${_salesController.GetTodaySales():N0}";
                        LineValues = _salesController.GetTodaySalesChartValues();
                        salesData = _salesController.GetSalesSumByCategoryForToday();
                        break;

                    default:
                        MessageBox.Show("Please select a valid option.");
                        return;
                }

                BarValues = GetBarChartValues(salesData);
            }
        }

        private void btnCustomersDetails_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                fCustomerDetailsContainer?.Navigate(new CustomerDetailsViewSubView());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }
        }

        private void btnSalesDetails_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                fCustomerDetailsContainer?.Navigate(new viewSalesdetailsViewSubView());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to page: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
