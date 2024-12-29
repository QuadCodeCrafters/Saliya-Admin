using ModernApp.MVC.View.EmployeeSubviews;
using ModernApp.MVC.View.ReportSubviews;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;


namespace ModernApp.Core
{
    class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
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


        public ObservableCollection<SalesData> GetSampleData()
        {
            return new ObservableCollection<SalesData>
        {
            new SalesData { SaleID = 1, ProductName = "Oil Change", ProductType = "Service", SalesAmount = 1500.00M, SaleDate = DateTime.Today },
            new SalesData { SaleID = 2, ProductName = "Car Wash", ProductType = "Service", SalesAmount = 800.00M, SaleDate = DateTime.Today },
            new SalesData { SaleID = 3, ProductName = "Tire Replacement", ProductType = "Product", SalesAmount = 20000.00M, SaleDate = DateTime.Today },
            new SalesData { SaleID = 4, ProductName = "Battery Replacement", ProductType = "Product", SalesAmount = 12000.00M, SaleDate = DateTime.Today },
            new SalesData { SaleID = 5, ProductName = "Air Filter Replacement", ProductType = "Product", SalesAmount = 3000.00M, SaleDate = DateTime.Today },
            new SalesData { SaleID = 6, ProductName = "Engine Diagnostic", ProductType = "Service", SalesAmount = 5000.00M, SaleDate = DateTime.Today },
              new SalesData { SaleID = 4, ProductName = "Battery Replacement", ProductType = "Product", SalesAmount = 12000.00M, SaleDate = DateTime.Today },
                               new SalesData { SaleID = 4, ProductName = "Battery Replacement", ProductType = "Product", SalesAmount = 12000.00M, SaleDate = DateTime.Today },
                                 new SalesData { SaleID = 4, ProductName = "Battery Replacement", ProductType = "Product", SalesAmount = 12000.00M, SaleDate = DateTime.Today },
                                   new SalesData { SaleID = 4, ProductName = "Battery Replacement", ProductType = "Product", SalesAmount = 12000.00M, SaleDate = DateTime.Today },
                                     new SalesData { SaleID = 4, ProductName = "Battery Replacement", ProductType = "Product", SalesAmount = 12000.00M, SaleDate = DateTime.Today },
                                       new SalesData { SaleID = 4, ProductName = "Battery Replacement", ProductType = "Product", SalesAmount = 12000.00M, SaleDate = DateTime.Today },
                                         new SalesData { SaleID = 4, ProductName = "Battery Replacement", ProductType = "Product", SalesAmount = 12000.00M, SaleDate = DateTime.Today },
                                           new SalesData { SaleID = 4, ProductName = "Battery Replacement", ProductType = "Product", SalesAmount = 12000.00M, SaleDate = DateTime.Today },
                                             new SalesData { SaleID = 4, ProductName = "Battery Replacement", ProductType = "Product", SalesAmount = 12000.00M, SaleDate = DateTime.Today },
                                               new SalesData { SaleID = 4, ProductName = "Battery Replacement", ProductType = "Product", SalesAmount = 12000.00M, SaleDate = DateTime.Today },
                                                 new SalesData { SaleID = 4, ProductName = "Battery Replacement", ProductType = "Product", SalesAmount = 12000.00M, SaleDate = DateTime.Today },
                                                   new SalesData { SaleID = 4, ProductName = "Battery Replacement", ProductType = "Product", SalesAmount = 12000.00M, SaleDate = DateTime.Today },
                                                     new SalesData { SaleID = 4, ProductName = "Battery Replacement", ProductType = "Product", SalesAmount = 12000.00M, SaleDate = DateTime.Today },
                                                       new SalesData { SaleID = 4, ProductName = "Battery Replacement", ProductType = "Product", SalesAmount = 12000.00M, SaleDate = DateTime.Today },
                                                         new SalesData { SaleID = 4, ProductName = "Battery Replacement", ProductType = "Product", SalesAmount = 12000.00M, SaleDate = DateTime.Today },
                                                           new SalesData { SaleID = 4, ProductName = "Battery Replacement", ProductType = "Product", SalesAmount = 12000.00M, SaleDate = DateTime.Today },
                                                             new SalesData { SaleID = 4, ProductName = "Battery Replacement", ProductType = "Product", SalesAmount = 12000.00M, SaleDate = DateTime.Today },
                                                               new SalesData { SaleID = 4, ProductName = "Battery Replacement", ProductType = "Product", SalesAmount = 12000.00M, SaleDate = DateTime.Today },
                                                                 new SalesData { SaleID = 4, ProductName = "Battery Replacement", ProductType = "Product", SalesAmount = 12000.00M, SaleDate = DateTime.Today },
                                                                   new SalesData { SaleID = 4, ProductName = "Battery Replacement", ProductType = "Product", SalesAmount = 12000.00M, SaleDate = DateTime.Today },
                                                                     new SalesData { SaleID = 4, ProductName = "Battery Replacement", ProductType = "Product", SalesAmount = 12000.00M, SaleDate = DateTime.Today },
        };
        }
    }
}
