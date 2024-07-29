using Microsoft.Win32;
using projectPad.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Project_Pad.Pages
{
    /// <summary>
    /// Interaction logic for AppSettingsPage.xaml
    /// </summary>
    public partial class AppSettingsPage : Page
    {
        public AppSettingsPage()
        {
            InitializeComponent();
            // Apply theme based on saved setting
            if ((bool)Properties.Settings.Default["DarkTheme"])
                DarkRadioBtn.IsChecked = true;
            else
                LightRadioBtn.IsChecked = true;

            // Apply startup setting based on saved setting
            if ((bool)Properties.Settings.Default["RunOnStartup"])
                StartupRunner.IsChecked = true;
            else
                OfRunner.IsChecked = true;

            if ((bool)Properties.Settings.Default["ShowNotifications"])
                OnNotification.IsChecked = true;
            else
                OfNotification.IsChecked = true;
        }

        public event EventHandler<string>? ThemeChanged;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
           
        }


        private void DarkRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            // Update the settings
           
            ThemeChanged?.Invoke(this, "NightMode");
            Properties.Settings.Default["DarkTheme"] = true;
            Properties.Settings.Default.Save();
        }

        private void LightRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            // Update the settings
           
            ThemeChanged?.Invoke(this, "LightMode");
            Properties.Settings.Default["DarkTheme"] = false;
            Properties.Settings.Default.Save();
        }
        private void StartupRunner_Checked(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(StartupRegistration.IsRegisteredInStartup().ToString());
           
            //Verify that the application is not registered
            if (!StartupRegistration.IsRegisteredInStartup())
            {
                //Register the application to run at startup
                StartupRegistration.RegisterInStartup();
                // Update the settings
                Properties.Settings.Default["RunOnStartup"] = true;
                Properties.Settings.Default.Save();
            }
        }
        private void OfRunner_Checked(object sender, RoutedEventArgs e)
        {
            //Unregister the application from startup
            if(StartupRegistration.IsRegisteredInStartup())
            {
                StartupRegistration.UnregisterFromStartup();
                // Update the settings
                Properties.Settings.Default["RunOnStartup"] = false;
                Properties.Settings.Default.Save();
            }
           
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = e.Uri.AbsoluteUri,
                    UseShellExecute = true
                });
                e.Handled = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening link: {ex.Message}");
            }
        }

        private void DownloadUstaad_Click(object sender, RoutedEventArgs e)
        {
        
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://drive.google.com/drive/folders/12Cl6GqvP90VQkwaiIz2Rx1ecFzWBkuFk?usp=drive_link",
                    UseShellExecute = true
                });
                e.Handled = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening link: {ex.Message}");
            }
        }

        private void GitRepo_Click(object sender, RoutedEventArgs e)
        {
        
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://github.com/fazalzubair994/Project-Pad.git",
                    UseShellExecute = true
                });
                e.Handled = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening link: {ex.Message}");
            }

        }

        private void OnNotification_Checked(object sender, RoutedEventArgs e)
        {

            Properties.Settings.Default["ShowNotifications"] = true;
            Properties.Settings.Default.Save();
        }

        private void OfNotification_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default["ShowNotifications"] = false;
            Properties.Settings.Default.Save();
        }
    }
}
