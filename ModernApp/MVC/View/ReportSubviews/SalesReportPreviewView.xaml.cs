﻿using Saliya_auto_care_Cashier.Notifications;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Security.Cryptography;
using System.Linq;

namespace ModernApp.MVC.View.ReportSubviews
{
    public partial class SalesReportPreviewView : UserControl
    {
        public DateTime CurrentDate { get; set; } = DateTime.Now;
        public ObservableCollection<SalesData> Sales { get; set; }

        public SalesReportPreviewView()
        {
            InitializeComponent();
            DataContext = this;

            // Load sample data
            Sales = GetSampleData();
            SalesReportDataGrid.ItemsSource = Sales;
        }

        private ObservableCollection<SalesData> GetSampleData()
        {
            return new ObservableCollection<SalesData>
            {
                new SalesData { SaleID = 1, ProductName = "Oil Change", ProductType = "Service", SalesAmount = 1500.00M, SaleDate = DateTime.Today },
                new SalesData { SaleID = 2, ProductName = "Car Wash", ProductType = "Service", SalesAmount = 800.00M, SaleDate = DateTime.Today },
                new SalesData { SaleID = 3, ProductName = "Tire Replacement", ProductType = "Product", SalesAmount = 20000.00M, SaleDate = DateTime.Today },
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
                                     new SalesData { SaleID = 4, ProductName = "Battery Replacement", ProductType = "Product", SalesAmount = 12000.00M, SaleDate = DateTime.Today },
                                      new SalesData { SaleID = 4, ProductName = "Battery Replacement", ProductType = "Product", SalesAmount = 12000.00M, SaleDate = DateTime.Today },
                                       new SalesData { SaleID = 4, ProductName = "Battery Replacement", ProductType = "Product", SalesAmount = 12000.00M, SaleDate = DateTime.Today },
                                        new SalesData { SaleID = 4, ProductName = "Battery Replacement", ProductType = "Product", SalesAmount = 12000.00M, SaleDate = DateTime.Today },
                               new SalesData { SaleID = 4, ProductName = "Battery Replacement", ProductType = "Product", SalesAmount = 12000.00M, SaleDate = DateTime.Today },
                               new SalesData { SaleID = 4, ProductName = "Battery Replacement", ProductType = "Product", SalesAmount = 12000.00M, SaleDate = DateTime.Today },
            };
        }


      


        private void RemoveScrollViewers(DependencyObject control)
        {
            if (control is ScrollViewer scrollViewer)
            {
                // Remove ScrollViewer content
                var content = scrollViewer.Content;
                scrollViewer.Content = null;

                // Replace with direct content
                if (control is UserControl userControl)
                {
                    userControl.Content = content;
                }
            }

            // Recursively remove from child elements
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(control); i++)
            {
                RemoveScrollViewers(VisualTreeHelper.GetChild(control, i));
            }
        }

        //public void ExportPdfButton_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        // Temporarily hide the footer
        //        HideFooter();

        //        PrintDialog printDialog = new PrintDialog();

        //        if (printDialog.ShowDialog() == true)
        //        {
        //            // Get A4 dimensions in device-independent units (1/96 inch per unit)
        //            const double A4Width = 8.27 * 96; // A4 width in inches (8.27 inches * 96 DPI)
        //            const double A4Height = 11.69 * 96; // A4 height in inches (11.69 inches * 96 DPI)

        //            // Create a copy of the UserControl for printing
        //            UserControl exportContent = new UserControl
        //            {
        //                Content = this.Content
        //            };

        //            // Measure and arrange the content to fit A4 dimensions
        //            exportContent.Measure(new Size(A4Width, A4Height));
        //            exportContent.Arrange(new Rect(new Point(0, 0), new Size(A4Width, A4Height)));

        //            // Scale the content to fit the page size
        //            double scaleX = A4Width / exportContent.ActualWidth;
        //            double scaleY = A4Height / exportContent.ActualHeight;
        //            exportContent.LayoutTransform = new ScaleTransform(scaleX, scaleY);

        //            // Print the content
        //            printDialog.PrintVisual(exportContent, "Saliya Auto Care Sales Report");

        //            MessageBox.Show("PDF exported successfully.", "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information);
        //            Notificationbox.ShowSuccess();
        //        }
        //        else
        //        {
        //            MessageBox.Show("PDF export canceled.", "Export Canceled", MessageBoxButton.OK, MessageBoxImage.Warning);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //    finally
        //    {
        //        // Restore the footer
        //        ShowFooter();

        //        // Reload the UserControl
        //        ShowAgain();
        //    }


        //}

     

        // Export button click event handler
        private void ExportPdfButton_Click(object sender, RoutedEventArgs e)
        {
            Sales = GetSampleData();
            // Get the sales data as a list from your observable collection
            List<SalesData> salesDataList = Sales.ToList();

            // Call the ExportPdfWithHeaderFooter method with the sales data
            ExportPdfWithHeaderFooter(salesDataList);
        }





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

                // Draw header
                DrawHeader(gfx, logo, margin, pageWidth);

                // Define column headers
                string[] headers = { "Sale ID", "Product Name", "Product Type", "Sales Amount", "Sale Date" };
                double[] columnWidths = { 50, 150, 100, 100, 100 };

                // Draw table header
                DrawTableRow(gfx, font, headers, columnWidths, margin, yOffset, true);
                yOffset += 20;

                // Draw data rows
                foreach (var sale in salesData)
                {
                    if (yOffset > pageHeight - margin - footerHeight) // Check if page is full
                    {
                        DrawFooter(gfx, pageWidth, pageHeight, footerHeight);

                        // Add new page
                        page = document.AddPage();
                        page.Size = PdfSharp.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        yOffset = margin + 100;

                        // Redraw header and table header
                        DrawHeader(gfx, logo, margin, pageWidth);
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
            XBrush brush = isHeader ? XBrushes.LightGray : XBrushes.Black;
            for (int i = 0; i < data.Length; i++)
            {
                gfx.DrawRectangle(XPens.Black, new XRect(xOffset, yOffset, columnWidths[i], 20));
                gfx.DrawString(data[i], font, brush, new XPoint(xOffset + 5, yOffset + 15));
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

        public void Buttonprint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.IsEnabled = false;

                // Temporarily hide the footer
                HideFooter();

                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    // Create a copy of the UserControl for printing
                    UserControl printContent = new UserControl
                    {
                        Content = this.Content
                    };

                    // Measure and arrange the content
                    printContent.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    printContent.Arrange(new Rect(new Point(0, 0), printContent.DesiredSize));

                    // Print the content
                    printDialog.PrintVisual(printContent, "Saliya Auto Care Sales Report");

                    MessageBox.Show("Printed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    Notificationbox.ShowSuccess();
                }
                else
                {
                    MessageBox.Show("Print canceled.", "Canceled", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Restore the footer
                ShowFooter();

                // Reload the UserControl
                ShowAgain();

                this.IsEnabled = true;
            }
        }

        public void ShowAgain()
        {
           
            UserControl newUser = new ReportSubviews.SalesReportPreviewView();
            this.Content = newUser;
        }

        private void HideFooter()
        {
            DockPanel footer = LogicalTreeHelper.FindLogicalNode(this, "FooterDockPanel") as DockPanel;
            if (footer != null)
            {
                footer.Visibility = Visibility.Collapsed;
            }
        }

        private void ShowFooter()
        {
            DockPanel footer = LogicalTreeHelper.FindLogicalNode(this, "FooterDockPanel") as DockPanel;
            if (footer != null)
            {
                footer.Visibility = Visibility.Visible;
            }
        }
        private void ReloadUserControl()
        {
            // Find the parent container
            var parent = VisualTreeHelper.GetParent(this) as Panel;
            if (parent != null)
            {
                // Create a new instance of the UserControl
                var newControl = new SalesReportPreviewView();

                // Remove the old UserControl
                parent.Children.Remove(this);

                // Add the new UserControl in its place
                parent.Children.Add(newControl);
            }
            else
            {
                MessageBox.Show("Unable to reload the report. Please refresh the application.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




    }

    public class SalesData
    {
        public int SaleID { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public decimal SalesAmount { get; set; }
        public DateTime SaleDate { get; set; }
    }
}
