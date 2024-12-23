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

namespace ModernApp.Notifications
{
    /// <summary>
    /// Interaction logic for CustomMessageBox.xaml
    /// </summary>
    public partial class CustomMessageBox : UserControl
    {
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            "Message", typeof(string), typeof(CustomMessageBox), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty ConfirmButtonTextProperty = DependencyProperty.Register(
            "ConfirmButtonText", typeof(string), typeof(CustomMessageBox), new PropertyMetadata("OK"));

        public static readonly DependencyProperty CancelButtonTextProperty = DependencyProperty.Register(
            "CancelButtonText", typeof(string), typeof(CustomMessageBox), new PropertyMetadata("Cancel"));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public string ConfirmButtonText
        {
            get { return (string)GetValue(ConfirmButtonTextProperty); }
            set { SetValue(ConfirmButtonTextProperty, value); }
        }

        public string CancelButtonText
        {
            get { return (string)GetValue(CancelButtonTextProperty); }
            set { SetValue(CancelButtonTextProperty, value); }
        }

        public event EventHandler ConfirmClicked;
        public event EventHandler CancelClicked;

        public CustomMessageBox()
        {
            InitializeComponent();

            // Event Handlers for Buttons
            ConfirmButton.Click += (s, e) => ConfirmClicked?.Invoke(this, EventArgs.Empty);
            CancelButton.Click += (s, e) => CancelClicked?.Invoke(this, EventArgs.Empty);
        }

        public void ShowMessage(string message, string confirmText = "OK", string cancelText = "Cancel")
        {
            MessageText.Text = message;
            ConfirmButton.Content = confirmText;
            CancelButton.Content = cancelText;
        }
    }
}
