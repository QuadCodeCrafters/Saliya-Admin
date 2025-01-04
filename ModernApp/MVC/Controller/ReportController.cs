using ModernApp.MVC.View.ReportSubviews;
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
                const double margin = 40; // Margin for content

                double yOffset = margin + 100; // Leave space for header
                double footerHeight = 50;

                // Load the logo image
                string logoPath = "D:\\personal\\Coding\\C#\\ModernApp\\ModernApp\\Images\\304778802_418138250302865_3903839059240116346_n-removebg-preview (1).png"; // Update with the actual path
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
                XFont font = new XFont("Arial", 12, XFontStyle.Regular);

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
        private void DrawTableRow(XGraphics gfx, XFont font, string[] data, double[] columnWidths, double xOffset, double yOffset, bool isHeader)
        {
            // Set the brush for the text color (black for both header and data)
            XBrush textBrush = isHeader ? XBrushes.Black : XBrushes.Black;

            // Set the background color for the header row (light gray) or no background for data rows
            if (isHeader)
            {
                // Fill the header row with light gray
                gfx.DrawRectangle(XBrushes.LightGray, new XRect(xOffset, yOffset, columnWidths.Sum(), 20));
            }

            // Draw the table cells (with text) for both header and data rows
            for (int i = 0; i < data.Length; i++)
            {
                // Draw the cell borders
                gfx.DrawRectangle(XPens.LightGray, new XRect(xOffset, yOffset, columnWidths[i], 20));

                // Draw the text inside the cell
                gfx.DrawString(data[i], font, textBrush, new XPoint(xOffset + 5, yOffset + 15));

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
                string logoPath = "D:\\personal\\Coding\\C#\\ModernApp\\ModernApp\\Images\\304778802_418138250302865_3903839059240116346_n-removebg-preview (1).png"; // Update with the actual path
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
