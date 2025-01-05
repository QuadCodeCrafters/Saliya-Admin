using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
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




        public List<EmployeeDatalot> GetEmployeeDataFromDb()
        {
            var employeeDataList = new List<EmployeeDatalot>();

            // SQL query to fetch data from Employee table
            string query = "SELECT EmployeeID, NationalIdentificationNumber, Name, DOB, Salary, Status, Mail, Position, Address, Phone, HireDate FROM Employee";

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
                            var employeeData = new EmployeeDatalot
                            {
                                EmployeeID = reader.GetInt32("EmployeeID"),
                                NationalIdentificationNumber = reader.GetString("NationalIdentificationNumber"),
                                Name = reader.GetString("Name"),
                                DOB = reader.GetDateTime("DOB"),
                                Salary = reader.GetDecimal("Salary"),
                                Status = reader.GetString("Status"),
                                Mail = reader.GetString("Mail"),
                                Position = reader.GetString("Position"),
                                Address = reader.GetString("Address"),
                                Phone = reader.GetString("Phone"),
                                HireDate = reader.GetDateTime("HireDate")
                            };

                            employeeDataList.Add(employeeData); // Add to the list
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately
                Console.WriteLine($"An error occurred while fetching data from the database: {ex.Message}");
            }

            return employeeDataList;
        }

        //get data for attendence report 
        public List<AttendenceDatalot> GetAttendanceDataFromDb()
        {
            var attendanceDataList = new List<AttendenceDatalot>();

            // SQL query to fetch data from Attendance and Employee tables
            string query = @"
        SELECT 
            a.AttendanceID,
            a.EmployeeID,
            e.Name,
            e.Position,
            a.AttendanceDate,
            a.Status,
            a.CheckInTime,
            a.CheckOutTime
        FROM 
            Attendance a
        INNER JOIN 
            Employee e
        ON 
            a.EmployeeID = e.EmployeeID";

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
                            var attendanceData = new AttendenceDatalot
                            {
                                AttendanceID = reader.GetInt32("AttendanceID"),
                                EmployeeID = reader.GetInt32("EmployeeID"),
                                Name = reader.GetString("Name"),
                                Position = reader.GetString("Position"),
                                AttendanceDate = reader.GetDateTime("AttendanceDate"),
                                Status = reader.GetString("Status"),
                                CheckInTime = reader.IsDBNull(reader.GetOrdinal("CheckInTime"))
                                              ? (TimeSpan?)null
                                              : reader.GetTimeSpan("CheckInTime"),
                                CheckOutTime = reader.IsDBNull(reader.GetOrdinal("CheckOutTime"))
                                               ? (TimeSpan?)null
                                               : reader.GetTimeSpan("CheckOutTime")
                            };

                            attendanceDataList.Add(attendanceData); // Add to the list
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately
                Console.WriteLine($"An error occurred while fetching data from the database: {ex.Message}");
            }

            return attendanceDataList;
        }

    }


    public class AttendenceDatalot
    {
        public int AttendanceID { get; set; }
        public int EmployeeID { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string Status { get; set; }
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
    }



    public class EmployeeDatalot
    {
        public int EmployeeID { get; set; }
        public string NationalIdentificationNumber { get; set; }
        public string Name { get; set; }
        public DateTime DOB { get; set; }
        public decimal Salary { get; set; }
        public string Status { get; set; }
        public string Mail { get; set; }
        public string Position { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public DateTime HireDate { get; set; }
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
