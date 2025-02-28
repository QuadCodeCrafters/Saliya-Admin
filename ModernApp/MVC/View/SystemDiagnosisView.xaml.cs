﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Timers;
using System.Net.NetworkInformation;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Threading;
using System.Windows;
using System.Management;
using System.Windows.Media;
using System.ComponentModel;

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
        private Timer processUpdateTimer;
        private Timer serviceUpdateTimer;

        public ChartValues<double> CpuUsageValues { get; set; }
        public ChartValues<double> MemoryUsageValues { get; set; }
        public ChartValues<double> UploadSpeedValues { get; set; }
        public ChartValues<double> DownloadSpeedValues { get; set; }
        public float CpuUsagePercentage { get;  set; }
        public double MemoryUsagePercentage { get; private set; }



        private Dictionary<string, long> previousBytesSent;
        private Dictionary<string, long> previousBytesReceived;



       

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



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

            // Start updating processes
            processUpdateTimer = new Timer(3000); // Update every 3 seconds
            processUpdateTimer.Elapsed += UpdateProcesses;
            processUpdateTimer.Start();

            // Start updating services
            serviceUpdateTimer = new Timer(5000); // Update every 5 seconds
            serviceUpdateTimer.Elapsed += UpdateServices;
            serviceUpdateTimer.Start();

            // Load initial network interfaces
            LoadNetworkInterfaces();


           

        }




        public event PropertyChangedEventHandler PropertyChanged;




        private string GetIntelChipName()
        {
            try
            {
                var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor WHERE Manufacturer LIKE '%Intel%'");
                foreach (var item in searcher.Get())
                {
                    return item["Name"]?.ToString();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving Intel chip name: {ex.Message}");
            }
            return "Unknown";
        }

        private void UpdatePerformanceData(object sender, ElapsedEventArgs e)
        {
            try
            {
                var cpuUsage = cpuCounter.NextValue();
                var availableMemory = memoryCounter.NextValue();
                var totalMemory = GetTotalMemory();
                var memoryUsage = 100 - (availableMemory / totalMemory * 100);
                // Retrieve the Intel chip name
                var intelChipName = GetIntelChipName();
                Dispatcher.Invoke(() =>
                {
                    CpuMemoryOverview.Text = $"CPU: {cpuUsage:F1}% | Memory: {memoryUsage:F1}%";
                });

                Dispatcher.Invoke(() =>
                {
                    CpuUsageValues.Add(cpuUsage);
                    if (CpuUsageValues.Count > 30) CpuUsageValues.RemoveAt(0);

                    MemoryUsageValues.Add(memoryUsage);
                    if (MemoryUsageValues.Count > 30) MemoryUsageValues.RemoveAt(0);

                    CpuUsagePercentage = cpuUsage;
                    MemoryUsagePercentage = memoryUsage;

                    CpuUsageChart.Series[0].Values = CpuUsageValues;
                    MemoryUsageChart.Series[0].Values = MemoryUsageValues;

                    // Update the TextBlock with CPU percentage
                    CpuUsageTextBlock.Text = $"{CpuUsagePercentage:F1}%";

                    // Change the color based on CPU usage percentage
                    if (CpuUsagePercentage > 90)
                    {
                        CpuUsageTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    else
                    {
                        CpuUsageTextBlock.Foreground = new SolidColorBrush(Colors.Green);
                    }

                    IntelChipNameTextBlock.Text = $"Intel Chip: {intelChipName}";
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
                // Check internet connectivity by pinging a reliable external host
                bool isInternetAccessible = false;
                try
                {
                    using (var ping = new Ping())
                    {
                        var reply = ping.Send("8.8.8.8", 1000); // Ping Google DNS with a 1-second timeout
                        isInternetAccessible = reply.Status == IPStatus.Success;
                    }
                }
                catch
                {
                    isInternetAccessible = false;
                }

                Dispatcher.Invoke(() =>
                {
                    NetworkOverview.Text = isInternetAccessible ? "Network: Up" : "Network: Down";
                });

                var uploadSpeed = 0.0;
                var downloadSpeed = 0.0;
                string activeConnectionName = "No Active Connection";

                foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.OperationalStatus == OperationalStatus.Up)
                    {
                        activeConnectionName = ni.Name; // Get the active connection name

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
                }

                Dispatcher.Invoke(() =>
                {
                    UploadSpeedValues.Add(uploadSpeed);
                    if (UploadSpeedValues.Count > 30) UploadSpeedValues.RemoveAt(0);

                    DownloadSpeedValues.Add(downloadSpeed);
                    if (DownloadSpeedValues.Count > 30) DownloadSpeedValues.RemoveAt(0);

                    NetworkConnectivityChart.Series[0].Values = UploadSpeedValues;
                    NetworkConnectivityChart.Series[1].Values = DownloadSpeedValues;

                    // Update textboxes
                    ConnectionNameTextBox.Text = $"Connection Name: {activeConnectionName}";
                    UploadSpeedTextBox.Text = $"Upload Speed (Mbps): {uploadSpeed:F2}";
                    DownloadSpeedTextBox.Text = $"Download Speed (Mbps): {downloadSpeed:F2}";
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating network data: {ex.Message}");
            }
        }



        public void OpenNetworkSettings()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "ms-settings:network",
                UseShellExecute = true
            });
        }


        public void OpenNetworkConnectionsControlPanel()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "ncpa.cpl",
                UseShellExecute = true
            });
        }
        private void OpenNetworkSettings_Click(object sender, RoutedEventArgs e)
        {
            OpenNetworkSettings();
        }

        private void OpenNetworkConnectionsControlPanel_Click(object sender, RoutedEventArgs e)
        {
            OpenNetworkConnectionsControlPanel();
        }




        private void UpdateProcesses(object sender, ElapsedEventArgs e)
        {
            try
            {

                var processCount = Process.GetProcesses().Length;

                Dispatcher.Invoke(() =>
                {
                    ProcessOverview.Text = $"Number of Processes: {processCount}";
                });


                var processes = Process.GetProcesses()
                                       .Select(p =>
                                       {
                                           try
                                           {
                                               return new ProcessInfo
                                               {
                                                   Name = p.ProcessName,
                                                  
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

                Dispatcher.Invoke(() =>
                {
                    ProcessesGrid.ItemsSource = processes;
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating processes: {ex.Message}");
            }
        }

        private void UpdateServices(object sender, ElapsedEventArgs e)
        {
            try
            {

                var runningServicesCount = ServiceController.GetServices()
           .Count(s => s.Status == ServiceControllerStatus.Running);

                Dispatcher.Invoke(() =>
                {
                    ServiceOverview.Text = $"Running Services: {runningServicesCount}";
                });


                var services = ServiceController.GetServices()
                                                .Select(s => new ServiceInfo
                                                {
                                                    Name = s.DisplayName,
                                                    Status = s.Status.ToString()
                                                }).ToList();

                Dispatcher.Invoke(() =>
                {
                    ServicesGrid.ItemsSource = services;
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating services: {ex.Message}");
            }
        }

        private void LoadNetworkInterfaces()
        {
            try
            {

              


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

        private double GetTotalMemory()
        {
            var computerInfo = new Microsoft.VisualBasic.Devices.ComputerInfo();
            return computerInfo.TotalPhysicalMemory / 1_048_576.0; // Convert bytes to MB
        }

        private void UserControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            updateTimer?.Stop();
            updateTimer?.Dispose();
            networkTimer?.Stop();
            networkTimer?.Dispose();
            processUpdateTimer?.Stop();
            processUpdateTimer?.Dispose();
            serviceUpdateTimer?.Stop();
            serviceUpdateTimer?.Dispose();
        }

        private void btnHomeTab_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedIndex = 0;
        }

        private void btnProcessesTab_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedIndex = 1;
        }

        private void btnPerformanceTab_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedIndex = 2;
        }

        private void btnServicesTab_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedIndex = 3;
        }

        private void btnNetworkTab_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedIndex = 4;
        }

       

    }

    public class ProcessInfo
    {
        public string Name { get; set; }
        public double CPU { get; set; }
        public double Memory { get; set; } // In MB
       

    }

    public class ServiceInfo
    {
        public string Name { get; set; }
        public string Status { get; set; }
    }
}
