using System;
using System.Windows;
using ModernApp.MVC.Model; // Ensure this namespace includes AttendanceService

namespace ModernApp.MVC.Controller
{
    public class MarkAttendanceController
    {
        private static MarkAttendanceController instance;
        private readonly EmployeeModel employeeModel;


        public static MarkAttendanceController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MarkAttendanceController();
                }
                return instance;
            }
        }
        private MarkAttendanceController()
        {
            employeeModel = new EmployeeModel(); // Instance of database logic
        }

        public void ProcessAttendance(string barcode)
        {
            bool success = employeeModel.RegisterAttendance(barcode);

            if (success)
            {
                MessageBox.Show("Attendance marked successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Failed to mark attendance. Check if barcode is registered.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
