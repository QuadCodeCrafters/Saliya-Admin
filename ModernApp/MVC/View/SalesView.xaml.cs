using LiveCharts.Wpf;
using LiveCharts;
using ModernApp.MVC.Controller;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;

namespace ModernApp.MVVM.View
{
    public partial class SalesView : UserControl, INotifyPropertyChanged
    {
        // Properties for chart values
        private ChartValues<double> _lineValues;
        private ChartValues<double> _barValues;
        
        private string[] _xLabels;
        private string _totalSalesDisplay;
        private SeriesCollection _seriesCollection;

        public ChartValues<double> LineValues
        {
            get => _lineValues;
            set
            {
                _lineValues = value;
                OnPropertyChanged(nameof(LineValues));
            }
        }

        public ChartValues<double> BarValues
        {
            get => _barValues;
            set
            {
                _barValues = value;
                OnPropertyChanged(nameof(BarValues));
            }
        }

        public string[] XLabels
        {
            get => _xLabels;
            set
            {
                _xLabels = value;
                OnPropertyChanged(nameof(XLabels));
            }
        }

        public string TotalSalesDisplay
        {
            get => _totalSalesDisplay;
            set
            {
                if (_totalSalesDisplay != value)
                {
                    _totalSalesDisplay = value;
                    OnPropertyChanged(nameof(TotalSalesDisplay));
                }
            }
        }

        public SeriesCollection SeriesCollection
        {
            get => _seriesCollection;
            set
            {
                _seriesCollection = value;
                OnPropertyChanged(nameof(SeriesCollection));
            }
        }

        private readonly SalesController _salesController;

        public SalesView()
        {
            InitializeComponent();

            // Pass the current application's Dispatcher to the SalesController
            _salesController = new SalesController(Application.Current.Dispatcher);

            // Subscribe to the sales update event
            _salesController.OnSalesUpdated += SalesController_OnSalesUpdated;

            // Set the DataContext for binding
            DataContext = this;

            // Load initial sales data and set up the chart
            LoadSalesData();
        }

        
        // Method to load sales data and update charts
        private void LoadSalesData()
        {
            // Get total sales data
            decimal totalSales = _salesController.GetTotalSales();
            TotalSalesDisplay = $"${totalSales:N0}";

            // Get today's sales data
            decimal todaySales = _salesController.GetTodaySales();
            TodaySalesDisplay = $"${todaySales:N0}";

            // Get total customers data
            decimal totalCus = _salesController.GetCustomersTotal();
            TotalCustomerDisplay = $"{totalCus:N0}";

            //// Get chart data for the bar series
            //BarValues = _salesController.GetSalesChartValues();
            LineValues = _salesController.GetThisMonthSalesChartValues();

            //// Set up series collection
            //SeriesCollection = new SeriesCollection
            //{
            //    new LineSeries
            //    {
            //        Title = "Sales Trend",
            //        Values = LineValues
            //    },
            //    new ColumnSeries
            //    {
            //        Title = "Daily Sales",
            //        Values = BarValues
            //    }
            //};

            //// Configure X-axis labels
            //XLabels = new[] { "Paint Job", "Vehicle Service", "Vehicle Repair", "Carrier Service", "Spare Parts" };

            // Retrieve sales data by category
            var salesDataByCategory = _salesController.GetSalesSumByCategory();
            var salesDateByCategoryThisYear = _salesController.GetSalesSumByCategoryForCurrentYear();
            var salesDateByCategoryThisMonth = _salesController.GetSalesSumByCategoryForCurrentMonth();
            var salesDateByCategoryToday = _salesController.GetSalesSumByCategoryForToday();


            // Ensure the order of categories matches the desired X-axis labels
            var categories = new[] { "Paint Jobs", "Vehicle Services", "Vehicle Repairs", "Carrier Service", "Spare Services" };
            var barChartValues = new ChartValues<double>();

            foreach (var category in categories)
            {
                if (salesDataByCategory.TryGetValue(category, out decimal salesSum))
                {
                    barChartValues.Add((double)salesSum);
                }
                else
                {
                    // Default to 0 if the category is missing from the data
                    barChartValues.Add(0);
                }
            }

            // Set BarValues to update the chart
            BarValues = barChartValues;

            // Update the SeriesCollection to reflect changes in BarValues
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
            {
                Title = "Sales Trend",
                Values = LineValues
            },
                new ColumnSeries
            {
                Title = "Category Sales",
                Values = BarValues
            }
        };

            // Configure X-axis labels to match categories
            XLabels = categories;
        

        //Load all Top sales data to Grid 
        var salesData = _salesController.GetSalesWithProductDetails();
            TopSalesDataGrid.ItemsSource = salesData;
            // Check if there's data and get the first item's ProductName
            if (salesData != null && salesData.Count > 0)
            {
                txtTopProduct.Text = salesData[0].ProductName;
            }
            else
            {
                txtTopProduct.Text = "No data available"; // Optional: handle empty case
            }
        }

        // Event handler for sales updates
        private void SalesController_OnSalesUpdated(decimal totalSales)
        {
            // Update the total sales display
            TotalSalesDisplay = $"${totalSales:N0}";
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Today's sales total
        private string _todaySalesDisplay;
        public string TodaySalesDisplay
        {
            get => _todaySalesDisplay;
            set
            {
                if (_todaySalesDisplay != value)
                {
                    _todaySalesDisplay = value;
                    OnPropertyChanged(nameof(TodaySalesDisplay));
                }
            }
        }

        // Sales details based on selected range
        private string _graphSalesDetails;
        public string GraphSalesDetails
        {
            get => _graphSalesDetails;
            set
            {
                if (_graphSalesDetails != value)
                {
                    _graphSalesDetails = value;
                    OnPropertyChanged(nameof(GraphSalesDetails));
                }
            }
        }

        // Total customers display
        private string _totalCustomerDisplay;
        public string TotalCustomerDisplay
        {
            get => _totalCustomerDisplay;
            set
            {
                if (_totalCustomerDisplay != value)
                {
                    _totalCustomerDisplay = value;
                    OnPropertyChanged(nameof(TotalCustomerDisplay));
                }
            }
        }

        // ComboBox Selection Changed Event Handler
        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

            // Retrieve sales data by category
            var salesDataByCategory = _salesController.GetSalesSumByCategory();
            var salesDateByCategoryThisYear = _salesController.GetSalesSumByCategoryForCurrentYear();
            var salesDateByCategoryThisMonth = _salesController.GetSalesSumByCategoryForCurrentMonth();
            var salesDateByCategoryToday = _salesController.GetSalesSumByCategoryForToday();


            // Ensure the order of categories matches the desired X-axis labels
            var categories = new[] { "Paint Jobs", "Vehicle Services", "Vehicle Repairs", "Carrier Service", "Spare Services" };
            var barChartValues = new ChartValues<double>();

           
            if (GraphStateManipulateComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedRange = selectedItem.Content.ToString();

                // Update GraphSalesDetails based on the selected range
                switch (selectedRange)
                {
                    case "This Year":
                        decimal thisYearSales = _salesController.GetThisYearSales(); // Ensure this method exists
                        GraphSalesDetails = $"Yearly Sales: ${thisYearSales:N0}";
                        txtBSummaryTotsales.Text = thisYearSales.ToString();
                        LineValues = _salesController.GetThisYearSalesChartValues();
                        foreach (var category in categories)
                        {
                            if (salesDateByCategoryThisYear.TryGetValue(category, out decimal salesSum))
                            {
                                barChartValues.Add((double)salesSum);
                            }
                            else
                            {
                                // Default to 0 if the category is missing from the data
                                barChartValues.Add(0);
                            }
                        }

                        // Set BarValues to update the chart
                        BarValues = barChartValues;
                        break;

                    case "This Month":
                        decimal thisMonthSales = _salesController.GetThisMonthSales(); // Ensure this method exists
                        GraphSalesDetails = $"Monthly Sales: ${thisMonthSales:N0}";
                        txtBSummaryTotsales.Text = thisMonthSales.ToString();
                        LineValues = _salesController.GetThisMonthSalesChartValues();
                        foreach (var category in categories)
                        {
                            if (salesDateByCategoryThisMonth.TryGetValue(category, out decimal salesSum))
                            {
                                barChartValues.Add((double)salesSum);
                            }
                            else
                            {
                                // Default to 0 if the category is missing from the data
                                barChartValues.Add(0);
                            }
                        }

                        // Set BarValues to update the chart
                        BarValues = barChartValues;
                        break;

                    case "Today":
                        decimal todaySales = _salesController.GetTodaySales();
                        GraphSalesDetails = $"Today's Sales: ${todaySales:N0}";
                        txtBSummaryTotsales.Text = todaySales.ToString();
                        LineValues = _salesController.GetTodaySalesChartValues();
                        foreach (var category in categories)
                        {
                            if (salesDateByCategoryToday.TryGetValue(category, out decimal salesSum))
                            {
                                barChartValues.Add((double)salesSum);
                            }
                            else
                            {
                                // Default to 0 if the category is missing from the data
                                barChartValues.Add(0);
                            }
                        }

                        // Set BarValues to update the chart
                        BarValues = barChartValues;
                        break;

                    default:
                        MessageBox.Show("Please select a valid option.");
                        break;
                }
            }
        }
    }
}
