using ModernApp.MVC.View.EmployeeSubviews;
using ModernApp.MVC.View.ReportSubviews;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using ModernApp.MVVM.Model;
using ModernApp.MVC.Model;


namespace ModernApp.Core
{
    class ObservableObject : INotifyPropertyChanged
    {
        private readonly reportModel reportModel;
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ObservableObject()
        {
            reportModel = new reportModel();
          
        }


        //public ObservableCollection<Employee> Employees { get; set; }
        //public Employee SelectedEmployee { get; set; }

        private Employee selectedEmployee;
        public Employee SelectedEmployee
        {
            get => selectedEmployee;
            set
            {

               
                selectedEmployee = value;
                OnPropertyChanged(); // Implement INotifyPropertyChanged
            }
        }

        public ObservableCollection<ModernApp.MVC.View.ReportSubviews.SalesData> GetSampleData()
        {
            var salesDataCollection = new ObservableCollection<ModernApp.MVC.View.ReportSubviews.SalesData>();

            try
            {
                // Fetch data as ObservableCollection<SalesDatalot>
                var salesDataLotList = reportModel.GetSalesDataFromDb(); // Returns ObservableCollection<SalesDatalot>

                // Map each SalesDatalot to SalesData
                foreach (var dataLot in salesDataLotList)
                {
                    var salesData = new ModernApp.MVC.View.ReportSubviews.SalesData
                    {
                        SaleID = dataLot.SaleID,
                        ProductName = dataLot.ProductName,
                        ProductType = dataLot.ProductType,
                        SalesAmount = dataLot.SalesAmount,
                        SaleDate = dataLot.SaleDate
                    };

                    salesDataCollection.Add(salesData);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately
                Console.WriteLine($"An error occurred while fetching sample data: {ex.Message}");
            }

            return salesDataCollection;
        }

        public ObservableCollection<ModernApp.MVC.View.ReportSubviews.EmployeeData> GetEmployeeSampleData()
        {
            var EmployeeDataCollection = new ObservableCollection<ModernApp.MVC.View.ReportSubviews.EmployeeData>();

            try
            {
                // Fetch data from the database
                var EmployeeDatalot = reportModel.GetEmployeeDataFromDb();

                // Iterate through the fetched data
                foreach (var dataLot in EmployeeDatalot)
                {
                    var employee = new ModernApp.MVC.View.ReportSubviews.EmployeeData
                    {
                        EmployeeID = dataLot.EmployeeID,
                        NationalIdentificationNumber = dataLot.NationalIdentificationNumber,
                        Name = dataLot.Name,
                        DOB = dataLot.DOB,
                        Salary = dataLot.Salary,
                        Status = dataLot.Status,
                        Mail = dataLot.Mail,
                        Position = dataLot.Position,
                        Address = dataLot.Address,
                        Phone = dataLot.Phone,
                        HireDate = dataLot.HireDate
                    };

                    EmployeeDataCollection.Add(employee);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately
                Console.WriteLine($"An error occurred while fetching sample data: {ex.Message}");
            }

            return EmployeeDataCollection;
        }


        public ObservableCollection<ModernApp.MVC.View.ReportSubviews.AttendenceData> GetAttendenceSampleData()
        {
            var AttendenceDataCollection = new ObservableCollection<ModernApp.MVC.View.ReportSubviews.AttendenceData>();

            try
            {
                // Fetch data from the database
                var AttendenceDatalot = reportModel.GetAttendanceDataFromDb();

                // Iterate through the fetched data
                foreach (var dataLot in AttendenceDatalot)
                {
                    var attendence = new ModernApp.MVC.View.ReportSubviews.AttendenceData
                    {
                        AttendanceID = dataLot.AttendanceID,
                        EmployeeID = dataLot.EmployeeID,
                        Name = dataLot.Name,
                        Status = dataLot.Status,
                        Position = dataLot.Position,
                        AttendanceDate = dataLot.AttendanceDate,
                        CheckInTime = dataLot.CheckInTime,
                        CheckOutTime = dataLot.CheckOutTime

                        

                 };

                    AttendenceDataCollection.Add(attendence);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately
                Console.WriteLine($"An error occurred while fetching sample data: {ex.Message}");
            }

            return AttendenceDataCollection;
        }





    }
}
