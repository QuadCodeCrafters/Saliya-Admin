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

namespace ModernApp.MVC.Controller
{
    internal class ReportController
    {
        //Export a PDF report with header and footer

        public void ExportPdfWithHeaderFooter(List<SalesData> salesData)
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

                // Add pages and content
                PdfPage page = document.AddPage();
                page.Size = PdfSharp.PageSize.A4;
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XFont font = new XFont("Arial", 12, XFontStyle.Regular);

                bool isHeaderDrawn = false; // Flag to track if the header has been drawn

                // Define column headers
                string[] headers = { "Sale ID", "Product Name", "Product Type", "Sales Amount", "Sale Date" };
                double[] columnWidths = { 50, 150, 100, 100, 100 };

                // Draw table header and header only on the first page
                foreach (var sale in salesData)
                {
                    if (!isHeaderDrawn)
                    {
                        // Draw header on the first page
                        DrawHeader(gfx, logo, margin, pageWidth);
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
                document.Save(filename);
                MessageBox.Show("PDF exported successfully.", "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                System.Diagnostics.Process.Start(filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Helper method to draw the header
        private void DrawHeader(XGraphics gfx, XImage logo, double margin, double pageWidth)
        {
            double logoWidth = 80;
            double logoHeight = 80;

            // Draw logo
            gfx.DrawImage(logo, margin, margin, logoWidth, logoHeight);

            // Draw header text
            gfx.DrawString("Saliya Auto Care", new XFont("Arial", 18, XFontStyle.Bold),
                XBrushes.Black, new XPoint(margin + logoWidth + 10, margin + 20));
            gfx.DrawString("Sales Report", new XFont("Arial", 14, XFontStyle.Regular),
                XBrushes.Gray, new XPoint(margin + logoWidth + 10, margin + 40));
            gfx.DrawString($"Date: {DateTime.Now:MMMM dd, yyyy}", new XFont("Arial", 12, XFontStyle.Regular),
                XBrushes.Gray, new XPoint(margin + logoWidth + 10, margin + 60));
        }



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



       

    }
}
