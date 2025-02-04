using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using LiveCharts;

namespace ModernApp.MVC.Model
{
    public class SalesModel
    {
        private readonly DBconnection _dbConnection;

        public class SalesEntry
        {
            public string ProductName { get; set; }
            public decimal SalesAmount { get; set; }
        }

        public SalesModel()
        {
            _dbConnection = new DBconnection();
        }

        private decimal ExecuteScalarQuery(string query)
        {
            try
            {
                using (var connection = _dbConnection.GetConnection())
                using (var command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    var result = command.ExecuteScalar();
                    return result != null && result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
                return 0;
            }
        }

        public decimal GetTotalSales() => ExecuteScalarQuery("SELECT SUM(TotalAmount) FROM Invoice;");
        public decimal GetTodaySales() => ExecuteScalarQuery("SELECT SUM(TotalAmount) FROM Invoice WHERE Date = CURDATE();");
        public decimal GetThisMonthSales() => ExecuteScalarQuery("SELECT SUM(TotalAmount) FROM Invoice WHERE MONTH(Date) = MONTH(CURDATE()) AND YEAR(Date) = YEAR(CURDATE());");
        public decimal GetThisYearSales() => ExecuteScalarQuery("SELECT SUM(TotalAmount) FROM Invoice WHERE YEAR(Date) = YEAR(CURDATE());");
        public decimal GetCustomersTotal() => ExecuteScalarQuery("SELECT COUNT(*) FROM vehicleregister;");

        private Dictionary<string, decimal> GetSalesByCategory(string dateFilter)
        {
            var salesData = new Dictionary<string, decimal>
            {
                { "Paint Jobs", 0 },
                { "Vehicle Services", 0 },
                { "Vehicle Repairs", 0 },
                { "Carrier Service", 0 },
                { "Spare Services", 0 }
            };

            string query = $@"
            SELECT 
                CASE 
                    WHEN ps.name IS NOT NULL THEN 'Paint Jobs'
                    WHEN s.name IS NOT NULL THEN 'Vehicle Services'
                    WHEN r.name IS NOT NULL THEN 'Vehicle Repairs'
                    ELSE 'Other'
                END AS Category,
                SUM(id.Amount) AS TotalSales
            FROM InvoiceDetails id
            LEFT JOIN paintservices ps ON id.Description = ps.name
            LEFT JOIN services s ON id.Description = s.name
            LEFT JOIN repairs r ON id.Description = r.name
            JOIN Invoice i ON id.InvoiceID = i.InvoiceID
            {dateFilter}
            GROUP BY Category;";

            try
            {
                using (var connection = _dbConnection.GetConnection())
                using (var command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string category = reader["Category"].ToString();
                            if (decimal.TryParse(reader["TotalSales"].ToString(), out decimal totalSales))
                            {
                                salesData[category] = totalSales;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching sales sums: {ex.Message}");
            }

            return salesData;
        }

        public Dictionary<string, decimal> GetSalesSumByCategory() => GetSalesByCategory("");
        public Dictionary<string, decimal> GetSalesSumByCategoryForToday() => GetSalesByCategory("WHERE DATE(i.Date) = CURDATE()");
        public Dictionary<string, decimal> GetSalesSumByCategoryForCurrentMonth() => GetSalesByCategory("WHERE MONTH(i.Date) = MONTH(CURDATE()) AND YEAR(i.Date) = YEAR(CURDATE())");
        public Dictionary<string, decimal> GetSalesSumByCategoryForCurrentYear() => GetSalesByCategory("WHERE YEAR(i.Date) = YEAR(CURDATE())");

        public List<SalesEntry> GetSalesWithProductDetails()
        {
            var salesData = new List<SalesEntry>();
            string query = "SELECT id.Description AS ProductName, SUM(id.Amount) AS TotalSales FROM InvoiceDetails id GROUP BY id.Description ORDER BY TotalSales DESC LIMIT 10;";

            try
            {
                using (var connection = _dbConnection.GetConnection())
                using (var command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            salesData.Add(new SalesEntry
                            {
                                ProductName = reader["ProductName"].ToString(),
                                SalesAmount = Convert.ToDecimal(reader["TotalSales"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching top sales data: {ex.Message}");
            }

            return salesData;
        }

        private List<decimal> GetSalesChartValues(string dateFilter)
        {
            var salesValues = new List<decimal>();
            string query = $"SELECT SUM(TotalAmount) FROM Invoice {dateFilter} GROUP BY Date ORDER BY Date;";

            try
            {
                using (var connection = _dbConnection.GetConnection())
                using (var command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            salesValues.Add(Convert.ToDecimal(reader[0]));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching sales chart data: {ex.Message}");
            }

            return salesValues;
        }

        public List<decimal> GetTodaySalesChartValues() => GetSalesChartValues("WHERE Date = CURDATE()");
        public List<decimal> GetThisMonthSalesChartValues() => GetSalesChartValues("WHERE MONTH(Date) = MONTH(CURDATE()) AND YEAR(Date) = YEAR(CURDATE())");
        public List<decimal> GetThisYearSalesChartValues() => GetSalesChartValues("WHERE YEAR(Date) = YEAR(CURDATE())");
    }
}
