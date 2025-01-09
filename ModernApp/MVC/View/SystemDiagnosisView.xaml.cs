using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Timers;
using System.Web;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using System.Net.NetworkInformation;
using System.Windows.Threading;

namespace ModernApp.MVVM.View
{
    /// <summary>
    /// Interaction logic for SystemDiagnosisView.xaml
    /// </summary>
    public partial class SystemDiagnosisView : UserControl
    {
        private PerformanceCounter cpuCounter;
        private PerformanceCounter memoryCounter;
        private Timer updateTimer;
        private Timer networkTimer;

        public ChartValues<double> CpuUsageValues { get; set; }
        public ChartValues<double> MemoryUsageValues { get; set; }
        public ChartValues<double> UploadSpeedValues { get; set; }
        public ChartValues<double> DownloadSpeedValues { get; set; }


        private Dictionary<string, long> previousBytesSent;
        private Dictionary<string, long> previousBytesReceived;

        public SystemDiagnosisView()
        {
            InitializeComponent();

            // Initialize performance counters
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            memoryCounter = new PerformanceCounter("Memory", "Available MBytes");

            CpuUsageValues = new ChartValues<double>();
            MemoryUsageValues = new ChartValues<double>();
            UploadSpeedValues = new ChartValues<double>();
            DownloadSpeedValues = new ChartValues<double>();

            previousBytesSent = new Dictionary<string, long>();
            previousBytesReceived = new Dictionary<string, long>();

            // Start updating performance data
            updateTimer = new Timer(1000); // Update every second
            updateTimer.Elapsed += UpdatePerformanceData;
            updateTimer.Start();

            // Start updating network data
            networkTimer = new Timer(1000); // Update every second
            networkTimer.Elapsed += UpdateNetworkData;
            networkTimer.Start();

            // Load processes, services, and network interfaces
            LoadProcesses();
            LoadServices();
            LoadNetworkInterfaces();
        }

        private void LoadProcesses()
        {
            try
            {
                var processes = Process.GetProcesses()
                                       .Select(p =>
                                       {
                                           try
                                           {
                                               return new ProcessInfo
                                               {
                                                   Name = p.ProcessName,
                                                   CPU = 0, // Placeholder: Fetching CPU usage is limited
                                                   Memory = Math.Round(p.WorkingSet64 / 1024.0 / 1024.0, 2) // Memory in MB
                                               };
                                           }
                                           catch (Exception ex) when (ex is UnauthorizedAccessException || ex is InvalidOperationException)
                                           {
                                               Debug.WriteLine($"Process {p.ProcessName} access denied: {ex.Message}");
                                               return null;
                                           }
                                       })
                                       .Where(p => p != null)
                                       .ToList();

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

        private void LoadNetworkInterfaces()
        {
            try
            {
                var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces()
                                                        .Where(ni => ni.OperationalStatus == OperationalStatus.Up)
                                                        .Select(ni => new NetworkInfo
                                                        {
                                                            Name = ni.Name,
                                                            Speed = ni.Speed / 1_000_000.0, // Convert to Mbps
                                                            Description = ni.Description
                                                        })
                                                        .ToList();

                NetworkInterfacesGrid.ItemsSource = networkInterfaces;

                // Initialize previous bytes sent and received
                foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    previousBytesSent[ni.Id] = ni.GetIPv4Statistics().BytesSent;
                    previousBytesReceived[ni.Id] = ni.GetIPv4Statistics().BytesReceived;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading network interfaces: {ex.Message}");
            }
        }

        private void UpdatePerformanceData(object sender, ElapsedEventArgs e)
        {
            try
            {
                var cpuUsage = cpuCounter.NextValue();
                var availableMemory = memoryCounter.NextValue();
                var totalMemory = GetTotalMemory();
                var memoryUsage = 100 - (availableMemory / totalMemory * 100);

                Dispatcher.Invoke(() =>
                {
                    CpuUsageValues.Add(cpuUsage);
                    if (CpuUsageValues.Count > 30) CpuUsageValues.RemoveAt(0);

                    MemoryUsageValues.Add(memoryUsage);
                    if (MemoryUsageValues.Count > 30) MemoryUsageValues.RemoveAt(0);

                    CpuUsageChart.Series[0].Values = CpuUsageValues;
                    MemoryUsageChart.Series[0].Values = MemoryUsageValues;
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating performance data: {ex.Message}");
            }
        }

        private void UpdateNetworkData(object sender, ElapsedEventArgs e)
        {
            try
            {
                var uploadSpeed = 0.0;
                var downloadSpeed = 0.0;

                foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    var stats = ni.GetIPv4Statistics();
                    var bytesSent = stats.BytesSent;
                    var bytesReceived = stats.BytesReceived;

                    if (previousBytesSent.ContainsKey(ni.Id))
                    {
                        uploadSpeed += (bytesSent - previousBytesSent[ni.Id]) / 1024.0 / 1024.0 * 8; // Mbps
                        downloadSpeed += (bytesReceived - previousBytesReceived[ni.Id]) / 1024.0 / 1024.0 * 8; // Mbps
                    }

                    previousBytesSent[ni.Id] = bytesSent;
                    previousBytesReceived[ni.Id] = bytesReceived;
                }

                Dispatcher.Invoke(() =>
                {
                    UploadSpeedValues.Add(uploadSpeed);
                    if (UploadSpeedValues.Count > 30) UploadSpeedValues.RemoveAt(0);

                    DownloadSpeedValues.Add(downloadSpeed);
                    if (DownloadSpeedValues.Count > 30) DownloadSpeedValues.RemoveAt(0);

                    NetworkConnectivityChart.Series[0].Values = UploadSpeedValues;
                    NetworkConnectivityChart.Series[1].Values = DownloadSpeedValues;
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating network data: {ex.Message}");
            }
        }

        private double GetTotalMemory()
        {
            var computerInfo = new Microsoft.VisualBasic.Devices.ComputerInfo();
            return computerInfo.TotalPhysicalMemory / 1_048_576.0;
        }

        private void UserControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            updateTimer?.Stop();
            updateTimer?.Dispose();
            networkTimer?.Stop();
            networkTimer?.Dispose();
        }
    }

    public class ProcessInfo
    {
        public string Name { get; set; }
        public double CPU { get; set; }
        public double Memory { get; set; }

    }

    public class ServiceInfo
    {
        public string Name { get; set; }
        public string Status { get; set; }
    }

    public class NetworkInfo
    {
        public string Name { get; set; }
        public double Speed { get; set; }
        public string Description { get; set; }
    }
}
