using ModernApp.MVC.View.EmployeeSubviews;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace ModernApp.MVC.Model
{
    internal class EmployeeModel
    {
        private readonly DBconnection _dbConnection;

        public EmployeeModel()
        {
            _dbConnection = new DBconnection();
        }

        /// <summary>
        /// Retrieves all employee records and returns them as a list.
        /// </summary>
        /// <returns>A List of Employee objects.</returns>
        public List<Employee> GetAllEmployees()
        {
            List<Employee> employees = new List<Employee>();

            try
            {
                using (var connection = _dbConnection.GetConnection())
                {
                    connection.Open();

                    string query = "SELECT * FROM Employee";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                employees.Add(new Employee
                                {
                                    EmployeeID = reader.GetInt32("EmployeeID"), // Correcting EmployeeID
                                    NationalIdentificationNumber = reader.GetString("NationalIdentificationNumber"), // Correctly mapping NationalIdentificationNumber
                                    Name = reader.GetString("Name"),
                                    Position = reader.GetString("Position"),
                                    Salary = reader.GetDecimal("Salary"),
                                    DOB = reader.GetDateTime("DOB"),
                                    Status = reader.GetString("Status"),
                                    Mail = reader.GetString("Mail"),
                                    Address = reader.GetString("Address"),
                                    Phone = reader.GetString("Phone"),
                                    HireDate = reader.GetDateTime("HireDate")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllEmployees: {ex.Message}");
            }

            return employees;
        }

        public bool AddEmployee(Employee employee)
        {
            try
            {
                string query = "INSERT INTO Employee (Name, Mail, DOB, Address, NationalIdentificationNumber, Position, Salary, Status, HireDate) " +
                               "VALUES (@Name, @Mail, @DOB, @Address, @NationalIdentificationNumber, @Position, @Salary, @Status, @HireDate)";

                using (var connection = _dbConnection.GetConnection())
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Name", employee.Name);
                    command.Parameters.AddWithValue("@Mail", employee.Mail);
                    command.Parameters.AddWithValue("@DOB", employee.DOB);
                    command.Parameters.AddWithValue("@Address", employee.Address);
                    command.Parameters.AddWithValue("@NationalIdentificationNumber", employee.NationalIdentificationNumber);
                    command.Parameters.AddWithValue("@Position", employee.Position);
                    command.Parameters.AddWithValue("@Salary", employee.Salary);
                    command.Parameters.AddWithValue("@Status", employee.Status);
                    command.Parameters.AddWithValue("@HireDate", employee.HireDate);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0; // Return true if rows were inserted
                }
            }
            catch (Exception)
            {
                return false; // Return false if any exception occurs
            }
        }



    }
}
