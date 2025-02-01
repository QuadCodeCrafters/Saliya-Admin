using ModernApp.MVC.View.EmployeeSubviews;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ZXing;

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
                // Generate a unique barcode number (Example: First 8 characters of a GUID)
                string barcodeNumber = Guid.NewGuid().ToString().Substring(0, 8);
                string barcodePath = GenerateBarcode(barcodeNumber, employee.Name); // Generate and save barcode image

                string query = "INSERT INTO Employee (Name, Mail, DOB, Address, NationalIdentificationNumber, Position, Salary, Phone, HireDate, ImagePath, BarcodeNumber) " +
                               "VALUES (@Name, @Mail, @DOB, @Address, @NationalIdentificationNumber, @Position, @Salary, @Phone, @HireDate, @ImagePath, @BarcodeNumber)";

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
                    command.Parameters.AddWithValue("@BarcodeNumber", barcodeNumber); // Store barcode number only

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0; // Return true if rows were inserted
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false; // Return false if any exception occurs
            }
        }


        public bool RecordAttendance(string scannedBarcodeNumber)
        {
            try
            {
                // Query to match the scanned barcode number with an employee
                string query = "SELECT Id FROM Employee WHERE BarcodeNumber = @BarcodeNumber";

                using (var connection = _dbConnection.GetConnection())
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@BarcodeNumber", scannedBarcodeNumber);

                    object result = command.ExecuteScalar();

                    if (result != null)
                    {
                        int employeeId = Convert.ToInt32(result);

                        // Insert attendance record
                        string attendanceQuery = "INSERT INTO Attendance (EmployeeId, Status) VALUES (@EmployeeId, @Status)";
                        MySqlCommand attendanceCommand = new MySqlCommand(attendanceQuery, connection);
                        attendanceCommand.Parameters.AddWithValue("@EmployeeId", employeeId);
                        attendanceCommand.Parameters.AddWithValue("@Status", "Present");

                        int rowsAffected = attendanceCommand.ExecuteNonQuery();
                        return rowsAffected > 0; // Return true if attendance is logged
                    }
                    else
                    {
                        // No matching employee found
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false; // Return false if an exception occurs
            }
        }


        private string GenerateBarcode(string barcodeText, string employeeName)
        {
            try
            {
                // Define the folder path where the barcode image should be saved
                string folderPath = @"D:\personal\Bar-code";

                // Create folder if it does not exist
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Define the barcode file path
                string filePath = Path.Combine(folderPath, $"{employeeName}_Barcode.png");

                // Create a barcode writer instance
                var barcodeWriter = new BarcodeWriter
                {
                    Format = BarcodeFormat.CODE_128, // Use CODE_128 format
                    Options = new ZXing.Common.EncodingOptions
                    {
                        Width = 300,
                        Height = 100
                    }
                };

                // Generate barcode image
                using (Bitmap bitmap = barcodeWriter.Write(barcodeText))
                {
                    // Save the barcode image to the folder
                    bitmap.Save(filePath, ImageFormat.Png);
                }

                return filePath; // Return the saved barcode file path
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating barcode: {ex.Message}");
                throw new Exception("Error generating barcode");
            }
        }

        public bool RegisterAttendance(string scannedBarcode)
        {
            try
            {
                using (var connection = _dbConnection.GetConnection())
                {
                    connection.Open();

                    // Step 1: Find Employee by Barcode
                    string findEmployeeQuery = "SELECT EmployeeID FROM Employee WHERE BarcodeNumber = @Barcode";
                    MySqlCommand findEmployeeCmd = new MySqlCommand(findEmployeeQuery, connection);
                    findEmployeeCmd.Parameters.AddWithValue("@Barcode", scannedBarcode);

                    object result = findEmployeeCmd.ExecuteScalar();
                    if (result == null)
                    {
                        return false; // No employee found with this barcode
                    }

                    int employeeID = Convert.ToInt32(result);
                    DateTime currentDate = DateTime.Now.Date;
                    DateTime currentTime = DateTime.Now;

                    // Step 2: Check if Employee has an Attendance Entry for Today
                    string checkAttendanceQuery = "SELECT AttendanceID, CheckInTime, CheckOutTime FROM Attendance WHERE EmployeeID = @EmployeeID AND AttendanceDate = @Date";
                    MySqlCommand checkAttendanceCmd = new MySqlCommand(checkAttendanceQuery, connection);
                    checkAttendanceCmd.Parameters.AddWithValue("@EmployeeID", employeeID);
                    checkAttendanceCmd.Parameters.AddWithValue("@Date", currentDate);

                    using (var reader = checkAttendanceCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int attendanceID = reader.GetInt32(0);
                            object checkInTime = reader["CheckInTime"];
                            object checkOutTime = reader["CheckOutTime"];
                            reader.Close();

                            if (checkInTime == DBNull.Value)
                            {
                                // First scan of the day (Check-In)
                                string updateCheckInQuery = "UPDATE Attendance SET CheckInTime = @CheckInTime WHERE AttendanceID = @AttendanceID";
                                MySqlCommand updateCheckInCmd = new MySqlCommand(updateCheckInQuery, connection);
                                updateCheckInCmd.Parameters.AddWithValue("@CheckInTime", currentTime);
                                updateCheckInCmd.Parameters.AddWithValue("@AttendanceID", attendanceID);
                                updateCheckInCmd.ExecuteNonQuery();
                                return true;
                            }
                            else if (checkOutTime == DBNull.Value)
                            {
                                // Second scan of the day (Check-Out)
                                string updateCheckOutQuery = "UPDATE Attendance SET CheckOutTime = @CheckOutTime WHERE AttendanceID = @AttendanceID";
                                MySqlCommand updateCheckOutCmd = new MySqlCommand(updateCheckOutQuery, connection);
                                updateCheckOutCmd.Parameters.AddWithValue("@CheckOutTime", currentTime);
                                updateCheckOutCmd.Parameters.AddWithValue("@AttendanceID", attendanceID);
                                updateCheckOutCmd.ExecuteNonQuery();
                                return true;
                            }
                            else
                            {
                                return false; // Already checked in and out
                            }
                        }
                        else
                        {
                            reader.Close();
                            // First scan of the day → Insert new attendance record
                            string insertAttendanceQuery = "INSERT INTO Attendance (EmployeeID, AttendanceDate, Status, CheckInTime) VALUES (@EmployeeID, @Date, 'Present', @CheckInTime)";
                            MySqlCommand insertCmd = new MySqlCommand(insertAttendanceQuery, connection);
                            insertCmd.Parameters.AddWithValue("@EmployeeID", employeeID);
                            insertCmd.Parameters.AddWithValue("@Date", currentDate);
                            insertCmd.Parameters.AddWithValue("@CheckInTime", currentTime);
                            insertCmd.ExecuteNonQuery();
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        //MessageBox.Show($"Name: {updateEmployee.Name}\n" +
        //        $"Position: {updateEmployee.Position}\n" +
        //        $"Salary: {updateEmployee.Salary:C}\n" +
        //        $"DOB: {updateEmployee.DOB.ToShortDateString()}\n" +
        //        $"Mail: {updateEmployee.Mail}\n" +
        //        $"Address: {updateEmployee.Address}\n" +
        //        $"Phone: {updateEmployee.Phone}\n" +
        //        $"National ID: {updateEmployee.NationalIdentificationNumber}\n" +
        //        $"Image Path: {updateEmployee.ProfilePicPath}\n" +
        //        $"Employee ID: {updateEmployee.EmployeeID}",
        //        "Employee Update Information", MessageBoxButton.OK, MessageBoxImage.Information);


        public bool UpdateEmployee(Employee updateEmployee)
        {
            try
            {
                string query = @"
UPDATE Employee
SET 
    Name = @Name,
    Position = @Position,
    Salary = @Salary,
    DOB = @DOB,
    Mail = @Mail,
    Address = @Address,
    Phone = @Phone,
    NationalIdentificationNumber = @NationalIdentificationNumber,
    ImagePath = @ImagePath
WHERE EmployeeID = @EmployeeID";

                using (var connection = _dbConnection.GetConnection())
                {
                    connection.Open();  
                    MySqlCommand command = new MySqlCommand(query, connection);

                    // Add parameters to the command
                    command.Parameters.AddWithValue("@Name", updateEmployee.Name);
                    command.Parameters.AddWithValue("@Position", updateEmployee.Position);
                    command.Parameters.AddWithValue("@Salary", updateEmployee.Salary);
                    command.Parameters.AddWithValue("@DOB", updateEmployee.DOB);
                    command.Parameters.AddWithValue("@Mail", updateEmployee.Mail);
                    command.Parameters.AddWithValue("@Address", updateEmployee.Address);
                    command.Parameters.AddWithValue("@Phone", updateEmployee.Phone);
                    command.Parameters.AddWithValue("@NationalIdentificationNumber", updateEmployee.NationalIdentificationNumber);
                    command.Parameters.AddWithValue("@ImagePath", updateEmployee.ProfilePicPath);
                    command.Parameters.AddWithValue("@EmployeeID", updateEmployee.EmployeeID);

                    int rowsAffected = command.ExecuteNonQuery();

                    // Debugging output
                    if (rowsAffected == 0)
                    {
                        MessageBox.Show("No rows were updated. Please check the EmployeeID.",
                            "Update Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }

                    return true; // Successfully updated
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }



    }
}
