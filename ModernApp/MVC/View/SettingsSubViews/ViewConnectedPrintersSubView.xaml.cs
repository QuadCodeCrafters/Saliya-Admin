using System;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernApp.MVC.View.SettingsSubViews
{
    public partial class ViewConnectedPrintersSubView : UserControl
    {
        public ObservableCollection<PrinterInfo> Printers { get; set; } = new ObservableCollection<PrinterInfo>();

        public ViewConnectedPrintersSubView()
        {
            InitializeComponent();
            LoadPrinters();
        }

        private void LoadPrinters()
        {
            Printers.Clear();

            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                PrinterSettings printDoc = new PrinterSettings { PrinterName = printer };

                Printers.Add(new PrinterInfo
                {
                    Name = printDoc.PrinterName,
                    IsDefault = printDoc.IsDefaultPrinter ? "✔ Default Printer" : "",
                    Status = GetPrinterStatus(printer)
                });
            }

            PrintersList.ItemsSource = Printers;
        }

        private string GetPrinterStatus(string printerName)
        {
            try
            {
                using (LocalPrintServer printServer = new LocalPrintServer())
                {
                    PrintQueue printerQueue = printServer.GetPrintQueue(printerName);
                    return printerQueue.QueueStatus.ToString();
                }
            }
            catch
            {
                return "Unknown";
            }
        }

        private void RefreshPrinters(object sender, RoutedEventArgs e)
        {
            LoadPrinters();
        }

        private void btnBackSettingsDashboard_Click(object sender, RoutedEventArgs e)
        {
            var parentFrame = FindParent<Frame>(this); // Find the parent Frame
            if (parentFrame != null)
            {
                parentFrame.Content = null; // Remove the UserControl (close it)
            }
        }

        // Helper function to find the parent Frame of the UserControl
        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null) return null;

            if (parentObject is T parent)
            {
                return parent;
            }
            else
            {
                return FindParent<T>(parentObject);
            }
        }
    }

    public class PrinterInfo
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public string IsDefault { get; set; }
    }
}
