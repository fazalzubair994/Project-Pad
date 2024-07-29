using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;

namespace projectPad.Controls
{
    internal class StartupRegistration
    {
        public static void RegisterInStartup()
        {
            

            try
            {
                string exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Project_Pad.exe");
                using (RegistryKey reg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true)!)
                {
                    string existingValue = (string)reg.GetValue("Project_Pad")!;
                    if (existingValue != $"\"{exePath}\"")
                    {
                        reg.SetValue("Project_Pad", $"\"{exePath}\"");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while registering the application: {ex.Message}");
            }
        }

        public static bool IsRegisteredInStartup()
        {
            try
            {
                string exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Project_Pad.exe");
                using (RegistryKey reg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true)!)
                {
                    if (reg == null)
                    {
                        return false;
                    }

                    string existingValue = (string)reg.GetValue("Project_Pad")!;
                    if (existingValue == null)
                    {
                        return false;
                    }

                    return existingValue.Equals($"\"{exePath}\"", StringComparison.OrdinalIgnoreCase);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while checking the registration: {ex.Message}");
                return false;
            }
        }

        public static void UnregisterFromStartup()
        {
            try
            {
                using (RegistryKey reg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true)!)
                {
                    if (reg != null)
                    {
                        reg.DeleteValue("Project_Pad", false);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while unregistering the application: {ex.Message}");
            }
        }
    }
}
