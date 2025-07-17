using MaterialDesignThemes.Wpf;
using ModernApp.Core;
using ModernApp.Events;
using ModernApp.MVC.Model;
using MySql.Data.MySqlClient;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Prism.Events;
namespace ModernApp
{
    /// <summary>
    /// Interaction logic for loginPage.xaml
    /// </summary>
    public partial class loginPage : Window
    {
        private readonly DBconnection dBconnection;
        public string loggedUsernamw;
        public loginPage()
        {
            InitializeComponent();
          
            dBconnection = new DBconnection();
        }

        private void btnlogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtusername.Text.Trim();
            string password = txtpassword.Password.Trim();
            loggedUsernamw= username;

            if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
            {
                accError.Text = "Please fill in the required fields";
                return;
            }

            if (string.IsNullOrEmpty(username))
            {
                accError.Text = "Please fill the Username";
                ShowUsernameErrorAnimation();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                accError.Text = "Please fill the password";
                ShowPasswordErrorAnimation();
                return;
            }

            // Validate user credentials
            if (ValidateAdminCredentials(username, password))
            {
                App.EventAggregator.GetEvent<GetAdminNameEvents>().Publish(username);
                MainWindow mainWin = new MainWindow();
                mainWin.Show();
                this.Close();

               
            }
            else
            {
                accError.Text = "Incorrect username or password";
                ShowUsernameErrorAnimation();
                ShowPasswordErrorAnimation();
            }
        }

      

        private bool ValidateAdminCredentials(string username, string password)
        {
            try
            {
                using (var conn = dBconnection.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT password_hash FROM admin_credentials WHERE username = @username";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            string storedPassword = result.ToString();
                            return password == storedPassword; // Direct comparison since it's not hashed
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to database: " + ex.Message);
            }

            return false;
        }


        private void ShowUsernameErrorAnimation()
        {
            txtusername.BorderBrush = Brushes.Red;

            TranslateTransform translateTransform = new TranslateTransform();
            txtusername.RenderTransform = translateTransform;

            DoubleAnimation animation = new DoubleAnimation
            {
                From = 0,
                To = 10,
                Duration = TimeSpan.FromMilliseconds(50),
                AutoReverse = true,
                RepeatBehavior = new RepeatBehavior(5) // Shake 5 times
            };

            translateTransform.BeginAnimation(TranslateTransform.XProperty, animation);

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(2);
            timer.Tick += (s, e) =>
            {
                txtusername.BorderBrush = (Brush)new BrushConverter().ConvertFromString("#FFDDDDDD"); // Reset to default border color
                timer.Stop();
            };
            timer.Start();
        }

        private void ShowPasswordErrorAnimation()
        {
            txtpassword.BorderBrush = Brushes.Red;

            TranslateTransform translateTransform = new TranslateTransform();
            txtpassword.RenderTransform = translateTransform;

            DoubleAnimation animation = new DoubleAnimation
            {
                From = 0,
                To = 10,
                Duration = TimeSpan.FromMilliseconds(50),
                AutoReverse = true,
                RepeatBehavior = new RepeatBehavior(5) // Shake 5 times
            };

            translateTransform.BeginAnimation(TranslateTransform.XProperty, animation);

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(2);
            timer.Tick += (s, e) =>
            {
                txtpassword.BorderBrush = (Brush)new BrushConverter().ConvertFromString("#FFDDDDDD");
                timer.Stop();
            };
            timer.Start();
        }

        public bool IsDarkTheme { get; set; }
        private readonly PaletteHelper paletteHelper = new PaletteHelper();

        private void themetoggle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ITheme theme = paletteHelper.GetTheme();

                if (IsDarkTheme = theme.GetBaseTheme() == BaseTheme.Dark)
                {
                    IsDarkTheme = false;
                    theme.SetBaseTheme(Theme.Light);
                }
                else
                {
                    IsDarkTheme = true;
                    theme.SetBaseTheme(Theme.Dark);
                }

                paletteHelper.SetTheme(theme);
            }
            catch (Exception obj)
            {
                MessageBox.Show(obj.Message.ToString());
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DisableLoginPageElements();

            loginhelp loginhelp = new loginhelp();
            loginhelp.Closed += Loginhelp_Closed;
            loginhelp.Show();
        }


        private void Loginhelp_Closed(object sender, EventArgs e)
        {
            EnableLoginPageElements();
        }

        private void DisableLoginPageElements()
        {
            txtusername.IsEnabled = false;
            txtpassword.IsEnabled = false;
            btnlogin.IsEnabled = false;
            DialogHost1.IsEnabled = false;
        }

        private void EnableLoginPageElements()
        {
            txtusername.IsEnabled = true;
            txtpassword.IsEnabled = true;
            btnlogin.IsEnabled = true;
            DialogHost1.IsEnabled = true;
        }


        private void txtusername_TextChanged(object sender, TextChangedEventArgs e)
        {
            accError.Text = ""; // Clear the error message TextChanged property on xaml
        }

        private void txtpassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            accError.Text = ""; // Clear the error message TextChanged property on xaml
        }

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseRightButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);
        }

        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.ChangedButton == System.Windows.Input.MouseButton.Middle)
            {
                // Add middle-click behavior if needed
            }
        }
    }
}
