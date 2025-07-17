using System;
using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using ModernApp.Events;
using ModernApp.MVVM.View;
using Prism.Events;


namespace ModernApp.Top_Control_Bar
{
    public partial class Top_Bar_Controls : UserControl
    {
        private readonly IEventAggregator _eventAggregator;

        // Property to manage the visibility of the Notifications popup
        public bool IsNotificationPopupOpen { get; set; }

        // Constructor
        public Top_Bar_Controls()
        {
            InitializeComponent();
            // Do not subscribe here to avoid Db connection issue
        }

        // Event handler when the admin name is received
        private void OnAdminNameReceived(string username)
        {
            // Assuming you have a TextBox or another control named adminName2
            if (adminName2 != null)
            {
                adminName2.Text = username; // Set the admin name to the TextBox
            }
            else
            {
                MessageBox.Show("adminName2 control is not available.");
            }
        }

        // Called after the control is loaded
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Subscribe after control is loaded
            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<GetAdminNameEvents>().Subscribe(OnAdminNameReceived);
            }
            else
            {
                MessageBox.Show("EventAggregator is null.");
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            // Action for Search
            MessageBox.Show("Search clicked!");
        }

        private void DarkModeToggle_Click(object sender, RoutedEventArgs e)
        {
            // Action for Dark Mode
            MessageBox.Show("Dark mode clicked!");
        }

        private void NotificationButton_Click(object sender, RoutedEventArgs e)
        {
            // Toggle the visibility of the Notifications popup
            IsNotificationPopupOpen = !IsNotificationPopupOpen;
            NotificationsPopup.IsOpen = IsNotificationPopupOpen;
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            // Toggle profile popup
            ProfilePopup.IsOpen = !ProfilePopup.IsOpen;
        }

        private void ProfilePopup_LostFocus(object sender, RoutedEventArgs e)
        {
            // Close the profile popup when it loses focus
            ProfilePopup.IsOpen = false;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Publish logout confirmation event
            App.EventAggregator.GetEvent<logoutConfirmationEvent>().Publish("OpenlogoutConfirmDialog");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
         

        }
    }
}
