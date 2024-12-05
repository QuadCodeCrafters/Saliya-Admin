using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;

public class EmployeeController
{
    public SeriesCollection AttendanceValues { get; set; }
    public List<string> Days { get; set; }
    public Func<double, string> YFormatter { get; set; }

    public EmployeeController()
    {
        // Days of the week
        Days = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

        // Generate random attendance data
        Random random = new Random();
        var randomData = new ChartValues<int>();
        for (int i = 0; i < 7; i++)
        {
            randomData.Add(random.Next(1, 10)); // Random values between 1 and 10
        }

        AttendanceValues = new SeriesCollection
        {
            new ColumnSeries
            {
                Title = "Attendance",
                Values = randomData
            }
        };

        // Y-axis formatter
        YFormatter = value => value.ToString("N0");
    }
}
