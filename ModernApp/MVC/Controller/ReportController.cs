using ModernApp.MVC.View.EmployeeSubviews;
using ModernApp.MVC.View.ReportSubviews;
using Mysqlx.Crud;
using PdfSharp.Charting;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ModernApp.MVC.Controller
{
    internal class ReportController
    {
        //Export a PDF report with header and footer
        //Sales report 
        public void ExportPdfWithHeaderFooter(List<SalesData> salesData, DataGrid dataGrid, decimal totalSales, DateTime? fromDate, DateTime? toDate, string selectedproducttype)
        {
            try
            {
                // Create PDF document
                PdfDocument document = new PdfDocument();
                document.Info.Title = "Saliya Auto Care Sales Report";

                const double pageWidth = 8.27 * 72; // A4 width in points (72 DPI)
                const double pageHeight = 11.69 * 72; // A4 height in points (72 DPI)
                const double margin = 20; // Margin for content

                double yOffset = margin + 90; // Leave space for header
                double footerHeight = 50;

                // Load the logo image
                string logoPath = "D:\\personal\\personal\\ModernApp\\ModernApp\\Images\\304778802_418138250302865_3903839059240116346_n-removebg-preview (1).png"; // Update with the actual path
                XImage logo = XImage.FromFile(logoPath);

                // Extract headers from DataGrid
                string[] headers = dataGrid.Columns
                    .Select(col => col.Header?.ToString() ?? string.Empty)
                    .ToArray();

                // Reduce the width of the first column to 50
                double firstColumnWidth = 50;
                double remainingWidth = pageWidth - firstColumnWidth - (headers.Length - 1) * 5; // Subtract space for other columns

                // Calculate the remaining column widths for other columns
                double[] columnWidths = new double[headers.Length];
                columnWidths[0] = firstColumnWidth;
                for (int i = 1; i < headers.Length; i++)
                {
                    columnWidths[i] = remainingWidth / (headers.Length - 1);
                }

                // Add pages and content
                PdfPage page = document.AddPage();
                page.Size = PdfSharp.PageSize.A4;
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XFont font = new XFont("Arial", 9, XFontStyle.Regular);

                bool isHeaderDrawn = false;

                foreach (var sale in salesData)
                {
                    if (!isHeaderDrawn)
                    {
                        // Draw header with optional date range
                        DrawHeader(gfx, logo, margin, pageWidth, totalSales, fromDate, toDate, selectedproducttype);

                        // Draw table header
                        DrawTableRow(gfx, font, headers, columnWidths, margin, yOffset, true);
                        yOffset += 20;
                        isHeaderDrawn = true;
                    }

                    if (yOffset > pageHeight - margin - footerHeight)
                    {
                        DrawFooter(gfx, pageWidth, pageHeight, footerHeight);

                        page = document.AddPage();
                        page.Size = PdfSharp.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        yOffset = margin + 100;

                        DrawTableRow(gfx, font, headers, columnWidths, margin, yOffset, true);
                        yOffset += 20;
                    }

                    string[] rowData =
                    {
                sale.SaleID.ToString(),
                sale.ProductName,
                sale.ProductType,
                sale.SalesAmount.ToString("C"),
                sale.SaleDate.ToShortDateString()
            };

                    DrawTableRow(gfx, font, rowData, columnWidths, margin, yOffset, false);
                    yOffset += 20;
                }

                DrawFooter(gfx, pageWidth, pageHeight, footerHeight);

                string filename = "SalesReport_WithHeaderFooter.pdf";
                document.Save(filename);
                MessageBox.Show("PDF exported successfully.", "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                System.Diagnostics.Process.Start(filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        public void ExportPdfEmployeeReport(
       List<EmployeeData> employeeDataList,
       DataGrid dataGrid,
       int totalEmployeeCount,
       string employeeNameFilter,
       string selectedJobTitle,
       string selectedStatus,
       DateTime? startDate,
       DateTime? endDate)
        {
            try
            {
                // Create PDF document
                PdfDocument document = new PdfDocument();
                document.Info.Title = "Saliya Auto Care Employee Report";

                const double pageWidth = 8.27 * 72; // A4 width in points (72 DPI)
                const double pageHeight = 11.69 * 72; // A4 height in points (72 DPI)
                const double margin = 20; // Margin for content

                double yOffset = margin + 120; // Leave space for header
                double footerHeight = 50;

                // Load the logo image
                string logoPath = "D:\\personal\\personal\\ModernApp\\ModernApp\\Images\\304778802_418138250302865_3903839059240116346_n-removebg-preview (1).png"; // Update with the actual path
                XImage logo = XImage.FromFile(logoPath);

                // Extract headers from DataGrid
                string[] headers = { "EmployeeID", "NIC", "Name", "DOB", "Salary", "Status", "Position", "Phone", "HireDate" };
                
                    //.Select(col => col.Header?.ToString() ?? string.Empty)
                    //.ToArray();


                // Reduce the width of the first column to 50
                double firstColumnWidth = 50;
                double remainingWidth = pageWidth - firstColumnWidth - (headers.Length - 1) * 5; // Subtract space for other columns

                // Calculate the remaining column widths for other columns
                double[] columnWidths = new double[headers.Length];
                columnWidths[0] = firstColumnWidth;
                for (int i = 1; i < headers.Length; i++)
                {
                    columnWidths[i] = remainingWidth / (headers.Length - 1);
                }

                // Add pages and content
                PdfPage page = document.AddPage();
                page.Size = PdfSharp.PageSize.A4;
                XGraphics gfx = XGraphics.FromPdfPage(page);

                // Smaller font for better fit
                XFont headerFont = new XFont("Segoe UI", 9, XFontStyle.Regular);
                XFont rowFont = new XFont("Arial", 8, XFontStyle.Regular);

                bool isHeaderDrawn = false;

                foreach (var employee in employeeDataList)
                {
                    if (!isHeaderDrawn)
                    {
                        // Draw header with optional filters
                        DrawEmployeeReportHeader(gfx, logo, margin, pageWidth, totalEmployeeCount, selectedJobTitle, selectedStatus, startDate, endDate);

                        // Draw table header
                        DrawTableRow(gfx, headerFont, headers, columnWidths, margin, yOffset, true);
                        yOffset += 25; // Adjust row spacing
                        isHeaderDrawn = true;
                    }

                    if (yOffset > pageHeight - margin - footerHeight)
                    {
                        DrawFooter(gfx, pageWidth, pageHeight, footerHeight);

                        page = document.AddPage();
                        page.Size = PdfSharp.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        yOffset = margin + 80;


                        DrawTableRow(gfx, headerFont, headers, columnWidths, margin, yOffset, true);
                        yOffset += 20; // Adjust row spacing
                    }

                    string[] rowData =
{
    employee.EmployeeID.ToString(),
    employee.NationalIdentificationNumber,
    employee.Name,
    employee.DOB.ToShortDateString(),
    employee.Salary.ToString("C"),  // Format salary as currency
    employee.Status,
    employee.Position,
    employee.Phone,
    employee.HireDate.ToShortDateString()
};



                    DrawTableRow(gfx, rowFont, rowData, columnWidths, margin, yOffset, false);
                    yOffset += 20; // Adjust row spacing
                }

                DrawFooter(gfx, pageWidth, pageHeight, footerHeight);

                string filename = "EmployeeReport_WithFilters.pdf";
                document.Save(filename);
                MessageBox.Show("PDF exported successfully.", "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                System.Diagnostics.Process.Start(filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public void ExportPdfAttendanceReport(
    List<AttendenceData> attendanceDataList,
    DataGrid dataGrid,
    int totalAttendanceCount,
    string employeeNameFilter,
    string selectedJobTitle,
    string selectedStatus,
    DateTime? startDate,
    DateTime? endDate,
    TimeSpan? selectedCheckInTime,
    TimeSpan? selectedCheckOutTime)
        {
            try
            {
                // Create PDF document
                PdfDocument document = new PdfDocument();
                document.Info.Title = "Saliya Auto Care Attendance Report";

                const double pageWidth = 8.27 * 72; // A4 width in points (72 DPI)
                const double pageHeight = 11.69 * 72; // A4 height in points (72 DPI)
                const double margin = 20; // Margin for content

                double yOffset = margin + 120; // Leave space for header
                double footerHeight = 50;

                // Load the logo image
                string logoPath = "D:\\personal\\personal\\ModernApp\\ModernApp\\Images\\304778802_418138250302865_3903839059240116346_n-removebg-preview (1).png"; // Update with the actual path
                XImage logo = XImage.FromFile(logoPath);

                // Headers for Attendance Report
                string[] headers = { "AttendanceID", "EmployeeID", "Name", "Position", "AttendanceDate", "Status", "CheckIn", "CheckOut" };

                // Column width calculations
                double firstColumnWidth = 70;
                double remainingWidth = pageWidth - firstColumnWidth - (headers.Length - 1) * 5;

                double[] columnWidths = new double[headers.Length];
                columnWidths[0] = firstColumnWidth;
                for (int i = 1; i < headers.Length; i++)
                {
                    columnWidths[i] = remainingWidth / (headers.Length - 1);
                }

                // Add pages and content
                PdfPage page = document.AddPage();
                page.Size = PdfSharp.PageSize.A4;
                XGraphics gfx = XGraphics.FromPdfPage(page);

                XFont headerFont = new XFont("Segoe UI", 9, XFontStyle.Regular);
                XFont rowFont = new XFont("Arial", 8, XFontStyle.Regular);

                bool isHeaderDrawn = false;

                foreach (var attendance in attendanceDataList)
                {
                    if (!isHeaderDrawn)
                    {
                        // Draw header with optional filters
                        DrawAttendanceReportHeader(gfx, logo, margin, pageWidth, totalAttendanceCount, selectedJobTitle, selectedStatus, startDate, endDate, selectedCheckInTime, selectedCheckOutTime);

                        // Draw table header
                        DrawTableRow(gfx, headerFont, headers, columnWidths, margin, yOffset, true);
                        yOffset += 25; // Adjust row spacing
                        isHeaderDrawn = true;
                    }

                    if (yOffset > pageHeight - margin - footerHeight)
                    {
                        DrawFooter(gfx, pageWidth, pageHeight, footerHeight);

                        page = document.AddPage();
                        page.Size = PdfSharp.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        yOffset = margin + 80;

                        DrawTableRow(gfx, headerFont, headers, columnWidths, margin, yOffset, true);
                        yOffset += 20; // Adjust row spacing
                    }

                    string[] rowData =
                    {
                attendance.AttendanceID.ToString(),
                attendance.EmployeeID.ToString(),
                attendance.Name,
                attendance.Position,
                attendance.AttendanceDate.ToShortDateString(),
                attendance.Status,
                attendance.CheckInTime?.ToString(@"hh\:mm") ?? "N/A", // Format TimeSpan as hh:mm
                attendance.CheckOutTime?.ToString(@"hh\:mm") ?? "N/A"
            };

                    DrawTableRow(gfx, rowFont, rowData, columnWidths, margin, yOffset, false);
                    yOffset += 20; // Adjust row spacing
                }

                DrawFooter(gfx, pageWidth, pageHeight, footerHeight);

                string filename = "AttendanceReport_WithFilters.pdf";
                document.Save(filename);
                MessageBox.Show("PDF exported successfully.", "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                System.Diagnostics.Process.Start(filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }






        // Updated DrawHeader to include date range
        private void DrawHeader(XGraphics gfx, XImage logo, double margin, double pageWidth, decimal totalSales, DateTime? fromDate, DateTime? toDate, string selectedproducttype)
        {
            double logoWidth = 80;
            double logoHeight = 80;

            gfx.DrawImage(logo, margin, margin, logoWidth, logoHeight);

            gfx.DrawString("Saliya Auto Care", new XFont("Arial", 18, XFontStyle.Bold),
                XBrushes.Black, new XPoint(margin + logoWidth + 10, margin + 20));
            gfx.DrawString("Sales Report", new XFont("Arial", 14, XFontStyle.Regular),
                XBrushes.Gray, new XPoint(margin + logoWidth + 10, margin + 40));
            gfx.DrawString($"Date: {DateTime.Now:MMMM dd, yyyy}", new XFont("Arial", 12, XFontStyle.Regular),
                XBrushes.Gray, new XPoint(margin + logoWidth + 10, margin + 60));

            double currentYPosition = margin + 80; // Start position for the header information

            // Define the space to place the two strings horizontally
            double xPosition = margin + logoWidth + 10;

            if (fromDate.HasValue && toDate.HasValue)
            {
                gfx.DrawString($"Report Period: {fromDate.Value:yyyy.MM.dd} - {toDate.Value:yyyy.MM.dd}",
                    new XFont("Arial", 12, XFontStyle.Regular), XBrushes.Gray,
                    new XPoint(xPosition, currentYPosition));
                xPosition += 230; // Move the x position for the next string horizontally
            }

            if (!string.IsNullOrEmpty(selectedproducttype))
            {
                gfx.DrawString($"Product Type: {selectedproducttype}",
                    new XFont("Arial", 12, XFontStyle.Regular), XBrushes.Gray,
                    new XPoint(xPosition, currentYPosition));
            }

            string totalSalesText = $"Total Sales: {totalSales:C}";
            gfx.DrawString(totalSalesText, new XFont("Arial", 12, XFontStyle.Bold),
                XBrushes.Black, new XPoint(pageWidth - margin - 200, margin + 40));
        }


        // Helper method to draw the header
        //employee report
        private void DrawEmployeeReportHeader(
     XGraphics gfx,
     XImage logo,
     double margin,
     double pageWidth,
     int totalEmployeeCount,
     string selectedJobTitle,
     string selectedStatus,
     DateTime? startDate,
     DateTime? endDate)
        {
            double logoWidth = 80;
            double logoHeight = 80;

            // Draw the logo
            gfx.DrawImage(logo, margin, margin, logoWidth, logoHeight);

            // Draw header text
            gfx.DrawString("Saliya Auto Care", new XFont("Segoe UI", 18, XFontStyle.Bold),
                XBrushes.Black, new XPoint(margin + logoWidth + 10, margin + 20));
            gfx.DrawString("Employee Report", new XFont("Segoe UI", 14, XFontStyle.Bold),
                XBrushes.Gray, new XPoint(margin + logoWidth + 10, margin + 40));
            gfx.DrawString($"Generated on: {DateTime.Now:MMMM dd, yyyy}", new XFont("Segoe UI", 12, XFontStyle.Regular),
                XBrushes.Gray, new XPoint(margin + logoWidth + 10, margin + 60));

            // Total employees
            double xPosition = margin + logoWidth + 10; // Start next to the header text
            double yPosition = margin + 80;
            gfx.DrawString($"Total Employees: {totalEmployeeCount}", new XFont("Segoe UI", 10, XFontStyle.Regular),
                XBrushes.Gray, new XPoint(xPosition, yPosition));

            // Adjust yPosition for filters (stacked vertically)
            yPosition += 20;

            // Filters stacked below "Total Employees"
            if (startDate.HasValue && endDate.HasValue)
            {
                gfx.DrawString($"Report Period: {startDate.Value:yyyy.MM.dd} - {endDate.Value:yyyy.MM.dd}",
                    new XFont("Segoe UI", 10, XFontStyle.Regular), XBrushes.Gray,
                    new XPoint(xPosition, yPosition));
                xPosition += 190; // Increment y position for next line
            }

           

            if (!string.IsNullOrEmpty(selectedJobTitle))
            {
                gfx.DrawString($"Job Title: {selectedJobTitle}",
                    new XFont("Segoe UI", 10, XFontStyle.Regular), XBrushes.Gray,
                    new XPoint(xPosition, yPosition));
                xPosition += 110; // Increment y position for next line
            }

            if (!string.IsNullOrEmpty(selectedStatus))
            {
                gfx.DrawString($"Status: {selectedStatus}",
                    new XFont("Segoe UI", 10, XFontStyle.Regular), XBrushes.Gray,
                    new XPoint(xPosition, yPosition));
            }

            // Check if there is data to display
            if (totalEmployeeCount == 0)
            {
                throw new InvalidOperationException("No data available to display in the report after applying the filters.");
            }
        }

        //attendence header
        private void DrawAttendanceReportHeader(
    XGraphics gfx,
    XImage logo,
    double margin,
    double pageWidth,
    int totalAttendanceCount,
    string employeeNameFilter,
    string selectedStatus,
    DateTime? startDate,
    DateTime? endDate,
    TimeSpan? checkInTime,
    TimeSpan? checkOutTime)
        {
            double logoWidth = 80;
            double logoHeight = 80;

            // Draw the logo
            gfx.DrawImage(logo, margin, margin, logoWidth, logoHeight);

            // Draw header text
            gfx.DrawString("Saliya Auto Care", new XFont("Segoe UI", 18, XFontStyle.Bold),
                XBrushes.Black, new XPoint(margin + logoWidth + 10, margin + 20));
            gfx.DrawString("Attendance Report", new XFont("Segoe UI", 14, XFontStyle.Bold),
                XBrushes.Gray, new XPoint(margin + logoWidth + 10, margin + 40));
            gfx.DrawString($"Generated on: {DateTime.Now:MMMM dd, yyyy}", new XFont("Segoe UI", 12, XFontStyle.Regular),
                XBrushes.Gray, new XPoint(margin + logoWidth + 10, margin + 60));

            // Total attendance records
            double xPosition = margin + logoWidth + 10; // Start next to the header text
            double yPosition = margin + 80;
            gfx.DrawString($"Total Records: {totalAttendanceCount}", new XFont("Segoe UI", 10, XFontStyle.Regular),
                XBrushes.Gray, new XPoint(xPosition, yPosition));

            // Adjust yPosition for filters (stacked vertically)
            yPosition += 20;

            // Filters stacked below "Total Records"
            if (startDate.HasValue && endDate.HasValue)
            {
                gfx.DrawString($"Report Period: {startDate.Value:yyyy.MM.dd} - {endDate.Value:yyyy.MM.dd}",
                    new XFont("Segoe UI", 10, XFontStyle.Regular), XBrushes.Gray,
                    new XPoint(xPosition, yPosition));
                xPosition += 190; // Increment y position for next line
            }

            if (startDate.HasValue && !endDate.HasValue)
            {
                gfx.DrawString($"Selected Date: {startDate.Value:yyyy.MM.dd}",
                    new XFont("Segoe UI", 10, XFontStyle.Regular), XBrushes.Gray,
                    new XPoint(xPosition, yPosition));
                yPosition += 20; // Increment y position for next line
            }

            //if (!string.IsNullOrEmpty(employeeNameFilter))
            //{
            //    gfx.DrawString($"Employee: {employeeNameFilter}",
            //        new XFont("Segoe UI", 10, XFontStyle.Regular), XBrushes.Gray,
            //        new XPoint(xPosition, yPosition));
            //    yPosition += 20; // Increment y position for next line
            //}

            if (!string.IsNullOrEmpty(selectedStatus))
            {
                gfx.DrawString($"Status: {selectedStatus}",
                    new XFont("Segoe UI", 10, XFontStyle.Regular), XBrushes.Gray,
                    new XPoint(xPosition, yPosition));
                xPosition += 190; // Increment y position for next line
            }

            if (checkInTime.HasValue || checkOutTime.HasValue)
            {
                string timeFilter = "Time: ";
                if (checkInTime.HasValue)
                    timeFilter += $"Check-In ≥ {checkInTime.Value:hh\\:mm} ";
                if (checkOutTime.HasValue)
                    timeFilter += $"Check-Out ≤ {checkOutTime.Value:hh\\:mm}";

                gfx.DrawString(timeFilter, new XFont("Segoe UI", 10, XFontStyle.Regular), XBrushes.Gray,
                    new XPoint(xPosition, yPosition));
                xPosition += 190; // Increment y position for next line
            }

            // Check if there is data to display
            if (totalAttendanceCount == 0)
            {
                throw new InvalidOperationException("No data available to display in the report after applying the filters.");
            }
        }




        //// Helper method to draw the header
        ////spare parts report
        //private void DrawSparePartsHeader(XGraphics gfx, XImage logo, double margin, double pageWidth)
        //{
        //    double logoWidth = 80;
        //    double logoHeight = 80;

        //    // Draw logo
        //    gfx.DrawImage(logo, margin, margin, logoWidth, logoHeight);

        //    // Draw header text
        //    gfx.DrawString("Saliya Auto Care", new XFont("Arial", 18, XFontStyle.Bold),
        //        XBrushes.Black, new XPoint(margin + logoWidth + 10, margin + 20));
        //    gfx.DrawString("Spare parts Report", new XFont("Arial", 14, XFontStyle.Regular),
        //        XBrushes.Gray, new XPoint(margin + logoWidth + 10, margin + 40));
        //    gfx.DrawString($"Date: {DateTime.Now:MMMM dd, yyyy}", new XFont("Arial", 12, XFontStyle.Regular),
        //        XBrushes.Gray, new XPoint(margin + logoWidth + 10, margin + 60));
        //}


        //// Helper method to draw the header
        ////gross profit report
        //private void DrawGrossProfitHeader(XGraphics gfx, XImage logo, double margin, double pageWidth)
        //{
        //    double logoWidth = 80;
        //    double logoHeight = 80;

        //    // Draw logo
        //    gfx.DrawImage(logo, margin, margin, logoWidth, logoHeight);

        //    // Draw header text
        //    gfx.DrawString("Saliya Auto Care", new XFont("Arial", 18, XFontStyle.Bold),
        //        XBrushes.Black, new XPoint(margin + logoWidth + 10, margin + 20));
        //    gfx.DrawString("Gross profit Report", new XFont("Arial", 14, XFontStyle.Regular),
        //        XBrushes.Gray, new XPoint(margin + logoWidth + 10, margin + 40));
        //    gfx.DrawString($"Date: {DateTime.Now:MMMM dd, yyyy}", new XFont("Arial", 12, XFontStyle.Regular),
        //        XBrushes.Gray, new XPoint(margin + logoWidth + 10, margin + 60));
        //}



        // Helper method to draw a table row
        //private void DrawTableRow(XGraphics gfx, XFont font, string[] data, double[] columnWidths, double xOffset, double yOffset, bool isHeader)
        //{
        //    // Set the brush for the text color (black for both header and data)
        //    XBrush textBrush = isHeader ? XBrushes.Black : XBrushes.Black;

        //    // Set the background color for the header row (light gray) or no background for data rows
        //    if (isHeader)
        //    {
        //        // Fill the header row with light gray
        //        gfx.DrawRectangle(XBrushes.LightGray, new XRect(xOffset, yOffset, columnWidths.Sum(), 20));
        //    }

        //    // Draw the table cells (with text) for both header and data rows
        //    for (int i = 0; i < data.Length; i++)
        //    {
        //        // Draw the cell borders
        //        gfx.DrawRectangle(XPens.LightGray, new XRect(xOffset, yOffset, columnWidths[i], 20));

        //        // Draw the text inside the cell
        //        gfx.DrawString(data[i], font, textBrush, new XPoint(xOffset + 5, yOffset + 15));

        //        // Move to the next column
        //        xOffset += columnWidths[i];
        //    }
        //}
        private void DrawTableRow(XGraphics gfx, XFont font, string[] data, double[] columnWidths, double xOffset, double yOffset, bool isHeader)
        {
            // Set text and background colors
            XBrush textBrush = XBrushes.Black;

            if (isHeader)
            {
                // Draw header background
                gfx.DrawRectangle(XBrushes.LightGray, new XRect(xOffset, yOffset, columnWidths.Sum(), 20));
            }
            else
            {
                // Draw the top and bottom horizontal borders
                double totalRowWidth = columnWidths.Sum();
                gfx.DrawLine(XPens.LightGray, xOffset, yOffset, xOffset + totalRowWidth, yOffset); // Top border
                gfx.DrawLine(XPens.LightGray, xOffset, yOffset + 20, xOffset + totalRowWidth, yOffset + 20); // Bottom border
              
            }

            // Draw each cell
            for (int i = 0; i < data.Length; i++)
            {
                // Measure the text size to center it
                var textSize = gfx.MeasureString(data[i], font);
                double textX = xOffset + (columnWidths[i] - textSize.Width) / 2; // Center horizontally
                double textY = yOffset + (20 - textSize.Height) / 2 + 5;         // Center vertically with padding

                // Draw the text inside the cell
                gfx.DrawString(data[i], font, textBrush, new XPoint(textX, textY));

                // Move to the next column
                xOffset += columnWidths[i];
            }
        }





        // Helper method to draw the footer
        private void DrawFooter(XGraphics gfx, double pageWidth, double pageHeight, double footerHeight)
        {
            string footerText = "© 2024 Saliya Auto Care, All Rights Reserved | Contact: info@saliyaautocare.com";
            gfx.DrawString(footerText, new XFont("Arial", 10, XFontStyle.Regular),
                XBrushes.Gray, new XPoint(pageWidth / 2, pageHeight - footerHeight), XStringFormats.Center);
        }

        //direcly it prints by opening the canon printer dialog box
        // Generate a printable document with header and footer

        public void GeneratePrintableDocument(List<SalesData> salesData, string pdfFilePath, DataGrid dataGrid, decimal totalsales , DateTime? fromDate, DateTime? toDate, string selectedproducttype)
        {
            try
            {
                // Extract headers from the DataGrid
                string[] headers = dataGrid.Columns
                    .Select(col => col.Header?.ToString() ?? string.Empty)
                    .ToArray();

                // Create PDF document
                PdfDocument document = new PdfDocument();
                document.Info.Title = "Saliya Auto Care Sales Report";

                const double pageWidth = 8.27 * 72; // A4 width in points (72 DPI)
                const double pageHeight = 11.69 * 72; // A4 height in points (72 DPI)
                const double margin = 40; // Margin for content

                double yOffset = margin + 100; // Leave space for header
                double footerHeight = 50;

                // Load the logo image
                string logoPath = "D:\\personal\\personal\\ModernApp\\ModernApp\\Images\\304778802_418138250302865_3903839059240116346_n-removebg-preview (1).png"; // Update with the actual path
                XImage logo = XImage.FromFile(logoPath);

                // Add pages and content
                PdfPage page = document.AddPage();
                page.Size = PdfSharp.PageSize.A4;
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XFont font = new XFont("Arial", 12, XFontStyle.Regular);

                bool isHeaderDrawn = false; // Flag to track if the header has been drawn

                // Reduce the width of the first column to 50
                double firstColumnWidth = 50;
                double remainingWidth = pageWidth - firstColumnWidth - (headers.Length - 1) * 5; // Subtract space for other columns

                // Calculate the remaining column widths for other columns
                double[] columnWidths = new double[headers.Length];

                // Set the width of the first column
                columnWidths[0] = firstColumnWidth;

                // Calculate the width for the other columns
                for (int i = 1; i < headers.Length; i++)
                {
                    columnWidths[i] = remainingWidth / (headers.Length - 1);
                }


                // Draw table header and header only on the first page
                foreach (var sale in salesData)
                {
                    if (!isHeaderDrawn)
                    {
                        // Draw header on the first page

                        // Draw header with optional date range
                        DrawHeader(gfx, logo, margin, pageWidth, totalsales, fromDate, toDate,selectedproducttype);

                        // Draw table header
                        DrawTableRow(gfx, font, headers, columnWidths, margin, yOffset, true);
                        yOffset += 20;
                        isHeaderDrawn = true; // Set flag to true after drawing the header
                    }

                    if (yOffset > pageHeight - margin - footerHeight) // Check if page is full
                    {
                        DrawFooter(gfx, pageWidth, pageHeight, footerHeight);

                        // Add new page
                        page = document.AddPage();
                        page.Size = PdfSharp.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        yOffset = margin + 100;

                        // Redraw header and table header on the new page
                        DrawTableRow(gfx, font, headers, columnWidths, margin, yOffset, true);
                        yOffset += 20;
                    }

                    string[] rowData =
                    {
                sale.SaleID.ToString(),
                sale.ProductName,
                sale.ProductType,
                sale.SalesAmount.ToString("C"),
                sale.SaleDate.ToShortDateString()
            };

                    DrawTableRow(gfx, font, rowData, columnWidths, margin, yOffset, false);
                    yOffset += 20;
                }

                // Draw footer on the last page
                DrawFooter(gfx, pageWidth, pageHeight, footerHeight);

                // Save the document
                string filename = "SalesReport_WithHeaderFooter.pdf";
                document.Save(pdfFilePath);
                MessageBox.Show("PDF exported successfully.", "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    //printing of employee report 
        public void GenerateEmployeePrintableDocument(
      List<EmployeeData> employeeDataList,
      string pdfFilePath,
      DataGrid dataGrid,
      int totalEmployeeCount,
      string employeeNameFilter,
      string selectedJobTitle,
      string selectedStatus,
      DateTime? startDate,
      DateTime? endDate)
        {
            try
            {
                // Extract headers from the DataGrid
                string[] headers = { "EmployeeID", "NIC", "Name", "DOB", "Salary", "Status", "Position", "Phone", "HireDate" };

                // Create PDF document
                PdfDocument document = new PdfDocument();
                document.Info.Title = "Saliya Auto Care Employee Report";

                const double pageWidth = 8.27 * 72; // A4 width in points (72 DPI)
                const double pageHeight = 11.69 * 72; // A4 height in points (72 DPI)
                const double margin = 40; // Margin for content

                double yOffset = margin + 120; // Leave space for header
                double footerHeight = 50;

                // Load the logo image
                string logoPath = "D:\\personal\\personal\\ModernApp\\ModernApp\\Images\\304778802_418138250302865_3903839059240116346_n-removebg-preview (1).png"; // Update with the actual path
                XImage logo = XImage.FromFile(logoPath);

                // Add pages and content
                PdfPage page = document.AddPage();
                page.Size = PdfSharp.PageSize.A4;
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XFont headerFont = new XFont("Segoe UI", 9, XFontStyle.Regular);
                XFont rowFont = new XFont("Arial", 8, XFontStyle.Regular);

                bool isHeaderDrawn = false; // Flag to track if the header has been drawn

                // Reduce the width of the first column to 50
                double firstColumnWidth = 50;
                double remainingWidth = pageWidth - firstColumnWidth - (headers.Length - 1) * 5; // Subtract space for other columns

                // Calculate the remaining column widths for other columns
                double[] columnWidths = new double[headers.Length];
                columnWidths[0] = firstColumnWidth;

                for (int i = 1; i < headers.Length; i++)
                {
                    columnWidths[i] = remainingWidth / (headers.Length - 1);
                }

                // Draw table header and content
                foreach (var employee in employeeDataList)
                {
                    if (!isHeaderDrawn)
                    {
                        // Draw header with filters
                        DrawEmployeeReportHeader(gfx, logo, margin, pageWidth, totalEmployeeCount, selectedJobTitle, selectedStatus, startDate, endDate);

                        // Draw table header
                        DrawTableRow(gfx, headerFont, headers, columnWidths, margin, yOffset, true);
                        yOffset += 20;
                        isHeaderDrawn = true; // Set flag to true after drawing the header
                    }

                    if (yOffset > pageHeight - margin - footerHeight) // Check if page is full
                    {
                        DrawFooter(gfx, pageWidth, pageHeight, footerHeight);

                        // Add new page
                        page = document.AddPage();
                        page.Size = PdfSharp.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        yOffset = margin + 100;

                        // Redraw header and table header on the new page
                        DrawTableRow(gfx, headerFont, headers, columnWidths, margin, yOffset, true);
                        yOffset += 20;
                    }

                    // Prepare employee data row
                    string[] rowData =
                    {
                employee.EmployeeID.ToString(),
                employee.NationalIdentificationNumber,
                employee.Name,
                employee.DOB.ToShortDateString(),
                employee.Salary.ToString("C"),  // Format salary as currency
                employee.Status,
                employee.Position,
                employee.Phone,
                employee.HireDate.ToShortDateString()
            };

                    DrawTableRow(gfx, rowFont, rowData, columnWidths, margin, yOffset, false);
                    yOffset += 20;
                }

                // Draw footer on the last page
                DrawFooter(gfx, pageWidth, pageHeight, footerHeight);

                // Save the document
                document.Save(pdfFilePath);
                MessageBox.Show("PDF exported successfully.", "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public void GenerateAttendancePrintableDocument(
    List<AttendenceData> attendanceDataList,
    string pdfFilePath,
    DataGrid dataGrid,
    int totalRecordCount,
    string employeeNameFilter,
    string selectedJobTitle,
    string selectedStatus,
    DateTime? startDate,
    DateTime? endDate,
    TimeSpan? selectedCheckInTime,
    TimeSpan? selectedCheckOutTime)
        {
            try
            {
                // Extract headers for the attendance report
                string[] headers = { "AttendanceID","EmployeeID", "Name", "Date", "Status", "Check-In", "Check-Out" };

                // Create PDF document
                PdfDocument document = new PdfDocument();
                document.Info.Title = "Saliya Auto Care Attendance Report";

                const double pageWidth = 8.27 * 72; // A4 width in points (72 DPI)
                const double pageHeight = 11.69 * 72; // A4 height in points (72 DPI)
                const double margin = 40; // Margin for content
                double yOffset = margin + 120; // Space for header
                double footerHeight = 50;

                // Load the logo image
                string logoPath = "D:\\personal\\personal\\ModernApp\\ModernApp\\Images\\304778802_418138250302865_3903839059240116346_n-removebg-preview (1).png"; // Update with the actual path
                XImage logo = XImage.FromFile(logoPath);

                // Add the first page
                PdfPage page = document.AddPage();
                page.Size = PdfSharp.PageSize.A4;
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XFont headerFont = new XFont("Segoe UI", 9, XFontStyle.Regular);
                XFont rowFont = new XFont("Arial", 8, XFontStyle.Regular);

                bool isHeaderDrawn = false;

                // Reduce the width of the first column to 50
                double firstColumnWidth = 70;
                double remainingWidth = pageWidth - firstColumnWidth - (headers.Length - 1) * 5; // Subtract space for other columns

                // Calculate the remaining column widths for other columns
                double[] columnWidths = new double[headers.Length];
                columnWidths[0] = firstColumnWidth;

                for (int i = 1; i < headers.Length; i++)
                {
                    columnWidths[i] = remainingWidth / (headers.Length - 1);
                }

                // Draw table header and content
                foreach (var attendence in attendanceDataList)
                {
                    if (!isHeaderDrawn)
                    {
                        // Draw header with filters
                        DrawAttendanceReportHeader(gfx, logo, margin, pageWidth, totalRecordCount, employeeNameFilter,  selectedStatus, startDate, endDate, selectedCheckInTime, selectedCheckOutTime);

                        // Draw table header
                        DrawTableRow(gfx, headerFont, headers, columnWidths, margin, yOffset, true);
                        yOffset += 20;
                        isHeaderDrawn = true;
                    }

                    if (yOffset > pageHeight - margin - footerHeight)
                    {
                        DrawFooter(gfx, pageWidth, pageHeight, footerHeight);

                        // Add new page
                        page = document.AddPage();
                        page.Size = PdfSharp.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        yOffset = margin + 100;

                        // Redraw table header on new page
                        DrawTableRow(gfx, headerFont, headers, columnWidths, margin, yOffset, true);
                        yOffset += 20;
                    }

                    string[] rowData =
                    {
                attendence.AttendanceID.ToString(),
                attendence.EmployeeID.ToString(),
                attendence.Name,
                attendence.AttendanceDate.ToShortDateString(),
                attendence.Status,
                attendence.CheckInTime?.ToString(@"hh\:mm") ?? "N/A", // Format TimeSpan as hh:mm
                attendence.CheckOutTime?.ToString(@"hh\:mm") ?? "N/A"
            };


                    DrawTableRow(gfx, rowFont, rowData, columnWidths, margin, yOffset, false);
                    yOffset += 20;
                }

                // Draw footer on the last page
                DrawFooter(gfx, pageWidth, pageHeight, footerHeight);

                // Save the document
                document.Save(pdfFilePath);
                MessageBox.Show("Attendance report PDF exported successfully.", "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }





        public void PrintPdf(string pdfFilePath)
        {
            try
            {
                // Determine the path to Microsoft Edge or Acrobat Reader
                string edgePath = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe"; // Adjust for your system
                string acrobatReaderPath = @"C:\Program Files\Adobe\Acrobat DC\Acrobat\Acrobat.exe"; // Adjust for your system

                //// Set the command to open the PDF in Microsoft Edge and print it
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    FileName = acrobatReaderPath,
                    Arguments = $"/t \"{pdfFilePath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                //{
                //    FileName = edgePath,
                //    Arguments = $"/print \"{pdfFilePath}\"", // Print the PDF without opening the UI
                //    UseShellExecute = false,
                //    CreateNoWindow = true
                //};

                //// If Edge is not available, fallback to Adobe Acrobat Reader
                //if (!File.Exists(acrobatReaderPath) && File.Exists(edgePath))
                //{
                //processInfo = new ProcessStartInfo 
                //{

                //};
                //}

                Process.Start(processInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while printing the PDF: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

       
    }
}
