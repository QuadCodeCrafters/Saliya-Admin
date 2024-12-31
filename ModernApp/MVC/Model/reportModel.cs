using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace ModernApp.MVC.Model
{
    public class reportModel
    {
        private readonly DBconnection dbConnection;


        public reportModel()
        {
            dbConnection = new DBconnection();
        }


        public List<SalesDatalot> GetSalesDataFromDb()
        {
            var salesDataList = new List<SalesDatalot>();

            // SQL query to fetch data from SalesTableTestOne
            string query = "SELECT SaleID, ProductName, ProductType, SalesAmount, SaleDate FROM SalesTableTestOne";

            try
            {
                // Ensure the database connection is properly managed
                using (var connection = dbConnection.GetConnection())
                {
                    connection.Open(); // Open the connection

                    // Create the MySQL command
                    var command = new MySqlCommand(query, connection);

                    using (command)
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var salesData = new SalesDatalot
                            {
                                SaleID = reader.GetInt32("SaleID"),
                                ProductName = reader.GetString("ProductName"),
                                ProductType = reader.GetString("ProductType"),
                                SalesAmount = reader.GetDecimal("SalesAmount"),
                                SaleDate = reader.GetDateTime("SaleDate")
                            };

                            salesDataList.Add(salesData); // Add to the list
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately
                Console.WriteLine($"An error occurred while fetching data from the database: {ex.Message}");
            }

            return salesDataList;
        }

    }

    public class SalesDatalot
    {
        public int SaleID { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public decimal SalesAmount { get; set; }
        public DateTime SaleDate { get; set; }
    }
}
