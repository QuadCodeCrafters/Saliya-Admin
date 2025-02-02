using System;
using System.Drawing;
using System.Windows;
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Saliya_auto_care_Cashier.Notifications;
using ModernApp.MVC.Controller;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace ModernApp.MVC.View.AttendenceSubViews
{
    public partial class MarkAttendece : Window
    {
        private VideoCaptureDevice videoSource;
        private BarcodeReader barcodeReader;
        private readonly MarkAttendanceController markAttendenceController;
        public MarkAttendece()
        {
            InitializeComponent();
            InitializeCamera();
            InitializeBarcodeReader(); // Correctly initialize barcode reader
        }

        private void InitializeCamera()
        {
            try
            {
                var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (videoDevices.Count == 0)
                {
                    MessageBox.Show("No cameras detected!");
                    return;
                }

                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);

                // Set resolution to a mid-level value
                if (videoSource.VideoCapabilities.Length > 0)
                {
                    // Sort resolutions by area (Width * Height)
                    var sortedResolutions = videoSource.VideoCapabilities
                        .OrderBy(res => res.FrameSize.Width * res.FrameSize.Height)
                        .ToList();

                    // Choose the resolution near the middle of the list
                    int midIndex = sortedResolutions.Count / 2;
                    videoSource.VideoResolution = sortedResolutions[midIndex];
                }

                videoSource.NewFrame += VideoSource_NewFrame;
                videoSource.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing camera: {ex.Message}");
            }
        }


        private bool isProcessingFrame = false; // Prevent multiple simultaneous scans

        private async void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            if (isProcessingFrame) return; // Skip processing if a scan is already running

            isProcessingFrame = true; // Mark as processing

            try
            {
                if (eventArgs.Frame == null) return;

                using (Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone())
                {
                    if (bitmap.Width == 0 || bitmap.Height == 0) return;

                    // Run barcode scanning in a background task to avoid UI lag
                    var result = await Task.Run(() => barcodeReader?.Decode(bitmap));

                    if (result != null)
                    {
                        string scannedBarcode = result.Text;

                        Dispatcher.Invoke(() =>
                        {
                            txtScannedBarcode.Text = scannedBarcode;
                            MarkAttendanceController.Instance.ProcessAttendance(scannedBarcode);
                            Notificationbox.ShowSuccess();
                        });
                    }

                    Dispatcher.Invoke(() =>
                    {
                        cameraFeed.Source = BitmapToImageSource(bitmap);
                    });
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Error processing video frame: {ex.Message}");
                });
            }
            finally
            {
                isProcessingFrame = false; // Allow next frame to be processed
            }
        }


        private void InitializeBarcodeReader()
        {
            barcodeReader = new BarcodeReader
            {
                AutoRotate = true,
                TryInverted = true,
                Options = new ZXing.Common.DecodingOptions
                {
                    TryHarder = true, // Makes decoding more reliable for low-quality images
                    PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE, BarcodeFormat.CODE_128 } // Limit formats for faster scanning
                }
            };
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            // Clean up camera resources when the window is closed
            if (videoSource != null)
            {
                videoSource.NewFrame -= VideoSource_NewFrame;
                videoSource.SignalToStop();
                videoSource.WaitForStop();
                videoSource.Stop();
            }
        }


        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (var memory = new System.IO.MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        

        private void btnBackAttendenceDashboard_Click(object sender, RoutedEventArgs e)
        {
            // Clean up camera resources when the window is closed
            if (videoSource != null)
            {
                videoSource.NewFrame -= VideoSource_NewFrame;
                videoSource.SignalToStop();
                videoSource.WaitForStop();
                videoSource.Stop();
            }
            this.Close();
        }


        
    }
}
