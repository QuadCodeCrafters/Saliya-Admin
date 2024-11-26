using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data.MySqlClient;

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

        public decimal GetTotalSales()
        {
            decimal totalSales = 0;

            using (var connection = _dbConnection.GetConnection())
            {
                string query = "SELECT SUM(salesAmount) FROM SalesTableTestOne";
                var command = new MySqlCommand(query, connection);

                connection.Open();
                var result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    totalSales = Convert.ToDecimal(result);
                }
            }

            return totalSales;
        }


        //DB connection to get all sales data
        public List<double> GetSalesData()
        {
            var salesData = new List<double>();

            using (var connection = _dbConnection.GetConnection())
            {
                string query = "SELECT SalesAmount FROM SalesTableTest"; // Replace with your actual column and table name
                var command = new MySqlCommand(query, connection);

                try
                {
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (double.TryParse(reader["salesAmount"].ToString(), out double value))
                            {
                                salesData.Add(value);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions appropriately
                    Console.WriteLine($"Error fetching sales data: {ex.Message}");
                }
            }

            return salesData;
        }

        //Get data to calculate todays sales 
        public decimal GetTodaySales()
        {
            decimal todaySales = 0;

            using (var connection = _dbConnection.GetConnection())
            {
                string query = "SELECT SUM(SalesAmount) AS TodaySales FROM SalesTableTestOne WHERE SaleDate = CURDATE();";
                var command = new MySqlCommand(query, connection);

                connection.Open();
                var result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                   
                    todaySales = Convert.ToDecimal(result);
                    

                }
                MessageBox.Show(Convert.ToString(todaySales));
            }

            return todaySales;
        }


        // Get data to calculate this month's sales
        public decimal GetThisMonthSales()
        {
            decimal thisMonthSales = 0;

            using (var connection = _dbConnection.GetConnection())
            {
                string query = @"
            SELECT SUM(SalesAmount) AS ThisMonthSales 
            FROM SalesTableTest 
            WHERE MONTH(SaleDate) = MONTH(CURDATE()) 
            AND YEAR(SaleDate) = YEAR(CURDATE());";

                var command = new MySqlCommand(query, connection);

                connection.Open();
                var result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    thisMonthSales = Convert.ToDecimal(result);
                }

                MessageBox.Show(Convert.ToString(thisMonthSales));
            }

            return thisMonthSales;
        }


        // Get data to calculate this month's sales
        public decimal GetThisYearSales()
        {
            decimal thisYearSales = 0;

            using (var connection = _dbConnection.GetConnection())
            {
                string query = @"
            SELECT SUM(SalesAmount) AS ThisYearSales 
            FROM SalesTableTest 
            WHERE  YEAR(SaleDate) = YEAR(CURDATE());";

                var command = new MySqlCommand(query, connection);

                connection.Open();
                var result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    thisYearSales = Convert.ToDecimal(result);
                }

                MessageBox.Show(Convert.ToString(thisYearSales));
            }

            return thisYearSales;
        }


        // Get all this year sales data (individual sales amounts)
        public List<double> GetAllthisYearSalesData()
        {
            var salesThisYearData = new List<double>();

            using (var connection = _dbConnection.GetConnection())
            {
                string query = "SELECT SalesAmount FROM SalesTableTestOne WHERE YEAR(SaleDate) = YEAR(CURDATE()); "; 
                var command = new MySqlCommand(query, connection);

                try
                {
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (double.TryParse(reader["SalesAmount"].ToString(), out double salesAmount))
                            {
                                salesThisYearData.Add(salesAmount);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions appropriately
                    Console.WriteLine($"Error fetching all sales data: {ex.Message}");
                }
            }

            return salesThisYearData;
        }


        // Get all this month sales data (individual sales amounts)
        public List<double> GetAllthisMonthSalesData()
        {
            var salesThisMonthData = new List<double>();

            using (var connection = _dbConnection.GetConnection())
            {
                string query = "SELECT SalesAmount FROM SalesTableTestOne WHERE MONTH(SaleDate) = MONTH(CURDATE()) AND YEAR(SaleDate) = YEAR(CURDATE()); ";


                var command = new MySqlCommand(query, connection);

                try
                {
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (double.TryParse(reader["SalesAmount"].ToString(), out double salesAmount))
                            {
                                salesThisMonthData.Add(salesAmount);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions appropriately
                    Console.WriteLine($"Error fetching all sales data: {ex.Message}");
                }
            }

            return salesThisMonthData;
        }


        // Get all today sales data (individual sales amounts)
        public List<double> GetAllTodaySalesData()
        {
            var salesTodayData = new List<double>();

            using (var connection = _dbConnection.GetConnection())
            {
                string query = "SELECT SalesAmount FROM SalesTableTestOne WHERE SaleDate = CURDATE();";


                var command = new MySqlCommand(query, connection);

                try
                {
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (double.TryParse(reader["SalesAmount"].ToString(), out double salesAmount))
                            {
                                salesTodayData.Add(salesAmount);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions appropriately
                    Console.WriteLine($"Error fetching all sales data: {ex.Message}");
                }
            }

            return salesTodayData;
        }





        /// <summary>
        /// Gets the sum of sales for each product type.
        /// </summary>
        /// <returns>A dictionary with product types as keys and their sales sums as values.</returns>
        public Dictionary<string, decimal> GetSalesSumByCategory()
        {
            var salesData = new Dictionary<string, decimal>
            {
                { "Paint Jobs", 0 },
                { "Vehicle Services", 0 },
                { "Vehicle Repairs", 0 },
                { "Carrier Service", 0 },
                { "Spare Services", 0 }
            };

            using (var connection = _dbConnection.GetConnection())
            {
                string query = @"
                    SELECT ProductType, SUM(SalesAmount) AS TotalSales 
                    FROM SalesTableTestOne 
                    WHERE ProductType IN ('Paint Jobs', 'Vehicle Services', 'Vehicle Repairs', 'Carrier Service', 'Spare Services') 
                    GROUP BY ProductType;";

                var command = new MySqlCommand(query, connection);

                try
                {
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string category = reader["ProductType"].ToString();
                            if (decimal.TryParse(reader["TotalSales"].ToString(), out decimal totalSales)
                                && salesData.ContainsKey(category))
                            {
                                salesData[category] = totalSales;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching sales sums by category: {ex.Message}");
                }
            }

            return salesData;
        }




        /// <summary>
        /// Gets the sum of sales for each product type for the current month.
        /// </summary>
        /// <returns>A dictionary with product types as keys and their sales sums as values.</returns>
        public Dictionary<string, decimal> GetSalesSumByCategoryForCurrentMonth()
        {
            var salesData = new Dictionary<string, decimal>
        {
            { "Paint Jobs", 0 },
            { "Vehicle Services", 0 },
            { "Vehicle Repairs", 0 },
            { "Carrier Service", 0 },
            { "Spare Services", 0 }
        };

            using (var connection = _dbConnection.GetConnection())
            {
                string query = @"
            SELECT ProductType, SUM(SalesAmount) AS TotalSales 
            FROM SalesTableTestOne 
            WHERE ProductType IN ('Paint Jobs', 'Vehicle Services', 'Vehicle Repairs', 'Carrier Service', 'Spare Services') 
              AND MONTH(SaleDate) = MONTH(CURRENT_DATE())
              AND YEAR(SaleDate) = YEAR(CURRENT_DATE())
            GROUP BY ProductType;";

                var command = new MySqlCommand(query, connection);

                try
                {
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string category = reader["ProductType"].ToString();
                            if (decimal.TryParse(reader["TotalSales"].ToString(), out decimal totalSales)
                                && salesData.ContainsKey(category))
                            {
                                salesData[category] = totalSales;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching sales sums by category for current month: {ex.Message}");
                }
            }

            return salesData;
        }



        /// <summary>
        /// Gets the sum of sales for each product type for the current year.
        /// </summary>
        /// <returns>A dictionary with product types as keys and their sales sums as values.</returns>
        public Dictionary<string, decimal> GetSalesSumByCategoryForCurrentYear()
        {
            var salesData = new Dictionary<string, decimal>
        {
            { "Paint Jobs", 0 },
            { "Vehicle Services", 0 },
            { "Vehicle Repairs", 0 },
            { "Carrier Service", 0 },
            { "Spare Services", 0 }
        };

            using (var connection = _dbConnection.GetConnection())
            {
                string query = @"
            SELECT ProductType, SUM(SalesAmount) AS TotalSales 
            FROM SalesTableTestOne 
            WHERE ProductType IN ('Paint Jobs', 'Vehicle Services', 'Vehicle Repairs', 'Carrier Service', 'Spare Services') 
            AND YEAR(SaleDate) = YEAR(CURRENT_DATE())
            GROUP BY ProductType;";

                var command = new MySqlCommand(query, connection);

                try
                {
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string category = reader["ProductType"].ToString();
                            if (decimal.TryParse(reader["TotalSales"].ToString(), out decimal totalSales)
                                && salesData.ContainsKey(category))
                            {
                                salesData[category] = totalSales;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching sales sums by category for current month: {ex.Message}");
                }
            }

            return salesData;
        }


        /// <summary>
        /// Gets the sum of sales for each product type for today.
        /// </summary>
        /// <returns>A dictionary with product types as keys and their sales sums as values.</returns>
        public Dictionary<string, decimal> GetSalesSumByCategoryForToday()
        {
            var salesData = new Dictionary<string, decimal>
        {
            { "Paint Jobs", 0 },
            { "Vehicle Services", 0 },
            { "Vehicle Repairs", 0 },
            { "Carrier Service", 0 },
            { "Spare Services", 0 }
        };

            using (var connection = _dbConnection.GetConnection())
            {
                string query = @"
            SELECT ProductType, SUM(SalesAmount) AS TotalSales 
            FROM SalesTableTestOne 
            WHERE ProductType IN ('Paint Jobs', 'Vehicle Services', 'Vehicle Repairs', 'Carrier Service', 'Spare Services') 
              AND DATE(SaleDate) = CURRENT_DATE()
            GROUP BY ProductType;";

                var command = new MySqlCommand(query, connection);

                try
                {
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string category = reader["ProductType"].ToString();
                            if (decimal.TryParse(reader["TotalSales"].ToString(), out decimal totalSales)
                                && salesData.ContainsKey(category))
                            {
                                salesData[category] = totalSales;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching sales sums by category for today: {ex.Message}");
                }
            }

            return salesData;
        }



        public List<SalesEntry> GetSalesWithProductDetails()
        {
            var salesData = new List<SalesEntry>();

            using (var connection = _dbConnection.GetConnection())
            {
                string query = @"
            SELECT ProductType, SUM(SalesAmount) AS TotalSales 
            FROM SalesTableTestOne 
            GROUP BY ProductType 
            ORDER BY TotalSales DESC 
            LIMIT 10;";

                var command = new MySqlCommand(query, connection);

                try
                {
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string productType = reader["ProductType"].ToString();
                            if (decimal.TryParse(reader["TotalSales"].ToString(), out decimal totalSales))
                            {
                                salesData.Add(new SalesEntry
                                {
                                    ProductName = productType,
                                    SalesAmount = totalSales
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching top sales data: {ex.Message}");
                }
            }

            return salesData;
        }





        public decimal GetCustomersTotal()
        {
            decimal totalCustomers = 0;

            using (var connection = _dbConnection.GetConnection())
            {
                string query = "SELECT COUNT(*) FROM customers;";
                var command = new MySqlCommand(query, connection);

                connection.Open();
                var result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {

                    totalCustomers = Convert.ToDecimal(result);


                }
                MessageBox.Show(Convert.ToString(totalCustomers));
            }

            return totalCustomers;
        }

    }
}
