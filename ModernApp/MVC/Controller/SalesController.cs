using LiveCharts;
using ModernApp.MVC.Model;
using System;
using System.Collections.Generic;
using System.Timers;
using static ModernApp.MVC.Model.SalesModel;

namespace ModernApp.MVC.Controller
{
    internal class SalesController : IDisposable
    {
        private readonly SalesModel _salesModel;
        private readonly Timer _refreshTimer;
        private readonly System.Windows.Threading.Dispatcher _dispatcher;

        public event Action<decimal> OnSalesUpdated;

        public SalesController(System.Windows.Threading.Dispatcher dispatcher)
        {
            _salesModel = new SalesModel();
            _dispatcher = dispatcher;

            _refreshTimer = new Timer(60000); // 1 minute in milliseconds
            _refreshTimer.Elapsed += RefreshSalesData;
            //_refreshTimer.Start();

            // Initial load of sales data
            RefreshSalesData(null, null);
        }

        //Get todays sales data
        public decimal GetTodaySales()
        {
            return _salesModel.GetTodaySales();
        }

        //Get thisMonth sales data
        public decimal GetThisMonthSales()
        {
            return _salesModel.GetThisMonthSales();
        }

        //Get this year sales data
        public decimal GetThisYearSales()
        {
            return _salesModel.GetThisYearSales();
        }

        public decimal GetCustomersTotal()
        {
            return _salesModel.GetCustomersTotal();
        }


        // Get data for the bar chart
        public ChartValues<double> GetSalesChartValues()
        {
            var salesData = _salesModel.GetSalesData();
            return new ChartValues<double>(salesData);
        }

        // Get data for the line chart(This year sales data)
        public ChartValues<double> GetThisYearSalesChartValues()
        {
            var salesThisyearData = _salesModel.GetAllthisYearSalesData();
            return new ChartValues<double>(salesThisyearData);
        }

        // Get data for the line chart(This month sales data)
        public ChartValues<double> GetThisMonthSalesChartValues()
        {
            var salesThisMonthData = _salesModel.GetAllthisMonthSalesData();
            return new ChartValues<double>(salesThisMonthData);
        }

        // Get data for the line chart(This today sales data)
        public ChartValues<double> GetTodaySalesChartValues()
        {
            var salesTodayData = _salesModel.GetAllTodaySalesData();
            return new ChartValues<double>(salesTodayData);
        }


        /// <summary>
        /// Retrieves the sales sum for each product type.
        /// </summary>
        /// <returns>A dictionary with product types as keys and their sales sums as values.</returns>
        public Dictionary<string, decimal> GetSalesSumByCategory()
        {
            try
            {
                return _salesModel.GetSalesSumByCategory();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SalesController: {ex.Message}");
                return new Dictionary<string, decimal>();
            }
        }


        /// <summary>
        /// Retrieves the sales sum for each product type by this year.
        /// </summary>
        /// <returns>A dictionary with product types as keys and their sales sums as values.</returns>
        public Dictionary<string, decimal> GetSalesSumByCategoryForCurrentYear()
        {
            try
            {
                return _salesModel.GetSalesSumByCategoryForCurrentYear();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SalesController: {ex.Message}");
                return new Dictionary<string, decimal>();
            }
        }

        /// <summary>
        /// Retrieves the sales sum for each product type by this month.
        /// </summary>
        /// <returns>A dictionary with product types as keys and their sales sums as values.</returns>
        public Dictionary<string, decimal> GetSalesSumByCategoryForCurrentMonth()
        {
            try
            {
                return _salesModel.GetSalesSumByCategoryForCurrentMonth();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SalesController: {ex.Message}");
                return new Dictionary<string, decimal>();
            }
        }

        /// <summary>
        /// Retrieves the sales sum for each product type by today.
        /// </summary>
        /// <returns>A dictionary with product types as keys and their sales sums as values.</returns>
        public Dictionary<string, decimal> GetSalesSumByCategoryForToday()
        {
            try
            {
                return _salesModel.GetSalesSumByCategoryForToday();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SalesController: {ex.Message}");
                return new Dictionary<string, decimal>();
            }
        }





        //Get all top sales data 
        public List<SalesEntry> GetSalesWithProductDetails()
        {
            return _salesModel.GetSalesWithProductDetails();
        }

        // Refresh sales data on timer
        private void RefreshSalesData(object sender, ElapsedEventArgs e)
        {
            decimal latestSales = _salesModel.GetTotalSales();

            // Use dispatcher to ensure UI thread execution
            _dispatcher.Invoke(() =>
            {
                OnSalesUpdated?.Invoke(latestSales); // Notify the view with the updated sales value
            });
        }

        // Get total sales from the model
        public decimal GetTotalSales()
        {
            return _salesModel.GetTotalSales();
        }




        
        // Dispose resources
        public void Dispose()
        {
            _refreshTimer?.Stop();
            _refreshTimer?.Dispose();
        }
    }
}
