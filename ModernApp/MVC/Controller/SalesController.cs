using LiveCharts;
using ModernApp.MVC.Model;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows.Threading;
using static ModernApp.MVC.Model.SalesModel;

namespace ModernApp.MVC.Controller
{
    internal class SalesController : IDisposable
    {
        private readonly SalesModel _salesModel;
        private readonly Timer _refreshTimer;
        private readonly Dispatcher _dispatcher;

        public event Action<decimal> OnSalesUpdated;

        public SalesController(Dispatcher dispatcher)
        {
            _salesModel = new SalesModel();
            _dispatcher = dispatcher;

            _refreshTimer = new Timer(60000); // 1-minute refresh
            _refreshTimer.Elapsed += RefreshSalesData;
            //_refreshTimer.Start();

            // Initial data load
            RefreshSalesData(null, null);
        }

        public decimal GetTodaySales() => _salesModel.GetTodaySales();
        public decimal GetThisMonthSales() => _salesModel.GetThisMonthSales();
        public decimal GetThisYearSales() => _salesModel.GetThisYearSales();
        public decimal GetTotalSales() => _salesModel.GetTotalSales();
        public decimal GetCustomersTotal() => _salesModel.GetCustomersTotal();

        // Fetch category sales with error handling
        private Dictionary<string, decimal> SafeFetch(Func<Dictionary<string, decimal>> fetchFunction)
        {
            try { return fetchFunction(); }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SalesController: {ex.Message}");
                return new Dictionary<string, decimal>();
            }
        }

        public Dictionary<string, decimal> GetSalesSumByCategory() => SafeFetch(_salesModel.GetSalesSumByCategory);
        public Dictionary<string, decimal> GetSalesSumByCategoryForCurrentYear() => SafeFetch(_salesModel.GetSalesSumByCategoryForCurrentYear);
        public Dictionary<string, decimal> GetSalesSumByCategoryForCurrentMonth() => SafeFetch(_salesModel.GetSalesSumByCategoryForCurrentMonth);
        public Dictionary<string, decimal> GetSalesSumByCategoryForToday() => SafeFetch(_salesModel.GetSalesSumByCategoryForToday);
        public List<SalesEntry> GetSalesWithProductDetails() => _salesModel.GetSalesWithProductDetails();

        // Fetch line chart values
        private ChartValues<double> ConvertToChartValues(List<decimal> data)
        {
            return new ChartValues<double>(data.ConvertAll(d => (double)d));
        }

        public ChartValues<double> GetTodaySalesChartValues() => ConvertToChartValues(_salesModel.GetTodaySalesChartValues());
        public ChartValues<double> GetThisMonthSalesChartValues() => ConvertToChartValues(_salesModel.GetThisMonthSalesChartValues());
        public ChartValues<double> GetThisYearSalesChartValues() => ConvertToChartValues(_salesModel.GetThisYearSalesChartValues());

        private void RefreshSalesData(object sender, ElapsedEventArgs e)
        {
            decimal latestSales = _salesModel.GetTotalSales();
            _dispatcher.Invoke(() => OnSalesUpdated?.Invoke(latestSales));
        }

        public void Dispose()
        {
            _refreshTimer?.Stop();
            _refreshTimer?.Dispose();
        }
    }
}
