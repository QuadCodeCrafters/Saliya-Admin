using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ServiceProcess;
using System.Diagnostics;


namespace ModernApp.MVVM.View
{
    /// <summary>
    /// Interaction logic for SystemDiagnosisView.xaml
    /// </summary>
    public partial class SystemDiagnosisView : UserControl
    {
        public SystemDiagnosisView()
        {
            InitializeComponent();
        }



        private void LoadProcesses()
        {
            try
            {
                var processes = Process.GetProcesses()
                                       .Select(p => new ProcessInfo
                                       {
                                           Name = p.ProcessName,
                                           CPU = 0, // Placeholder: CPU usage requires PerformanceCounter
                                           Memory = Math.Round(p.WorkingSet64 / 1024.0 / 1024.0, 2) // Memory in MB
                                       }).ToList();

                ProcessesGrid.ItemsSource = processes;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading processes: {ex.Message}");
            }
        }

        private void LoadServices()
        {
            try
            {
                var services = ServiceController.GetServices()
                                                .Select(s => new ServiceInfo
                                                {
                                                    Name = s.DisplayName,
                                                    Status = s.Status.ToString()
                                                }).ToList();

                ServicesGrid.ItemsSource = services;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading services: {ex.Message}");
            }
        }
    }
}
