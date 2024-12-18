using ModernApp.MVC.View.EmployeeSubviews;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

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
                                    HireDate = reader.GetDateTime("HireDate"),
                                    ProfilePicPath = reader["ImagePath"].ToString()


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

            if (employees == null || employees.Count == 0)
            {
                MessageBox.Show("No employee data Database found.");
               
            }

            return employees;
        }

        public bool AddEmployee(Employee employee)
        {
            try
            {
                string query = "INSERT INTO Employee (Name, Mail, DOB, Address, NationalIdentificationNumber, Position, Salary, Phone ,HireDate, ImagePath) " +
                               "VALUES (@Name, @Mail, @DOB, @Address, @NationalIdentificationNumber, @Position, @Salary, @Phone, @HireDate,@ImagePath)";

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
                    command.Parameters.AddWithValue("@HireDate", employee.HireDate);
                    command.Parameters.AddWithValue("@Phone", employee.Phone);
                    command.Parameters.AddWithValue("@ImagePath", employee.ProfilePicPath);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0; // Return true if rows were inserted
                }
            }
            catch (Exception)
            {
                return false; // Return false if any exception occurs
            }
        }


        public bool UpdateEmployee(Employee employee)
        {
            string query = @"
            UPDATE Employees
            SET Name = @Name,
                Position = @Position,
                Salary = @Salary,
                DOB = @DOB,
                Status = @Status,
                Mail = @Mail,
                Address = @Address,
                Phone = @Phone,
                HireDate = @HireDate,
                NationalIdentificationNumber = @NationalIdentificationNumber,
                ProfilePicPath = @ProfilePicPath
            WHERE EmployeeID = @EmployeeID";

            using (var connection = new SqlConnection("YourConnectionString"))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", employee.Name);
                command.Parameters.AddWithValue("@Position", employee.Position);
                command.Parameters.AddWithValue("@Salary", employee.Salary);
                command.Parameters.AddWithValue("@DOB", employee.DOB);
                command.Parameters.AddWithValue("@Status", employee.Status);
                command.Parameters.AddWithValue("@Mail", employee.Mail);
                command.Parameters.AddWithValue("@Address", employee.Address);
                command.Parameters.AddWithValue("@Phone", employee.Phone);
                command.Parameters.AddWithValue("@HireDate", employee.HireDate);
                command.Parameters.AddWithValue("@NationalIdentificationNumber", employee.NationalIdentificationNumber);
                command.Parameters.AddWithValue("@ProfilePicPath", employee.ProfilePicPath);
                command.Parameters.AddWithValue("@EmployeeID", employee.EmployeeID);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }



    }
}
