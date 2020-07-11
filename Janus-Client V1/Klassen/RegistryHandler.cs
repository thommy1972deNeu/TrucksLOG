using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Janus_Client_V1.Klassen
{
    class RegistryHandler
    {
        //STEAM
        public static string Steam64bitRegistry = @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Valve\Steam";
        public static string SteamInstallPathValueName = "InstallPath";
        public static object Globalread(string registryPath, string valueName)
        {
            try
            {
                if (registryPath.Contains("HKEY_LOCAL_MACHINE"))
                {
                    registryPath = registryPath.Replace(@"HKEY_LOCAL_MACHINE\", "");
                    Console.WriteLine(registryPath);
                    RegistryKey key = Registry.LocalMachine.OpenSubKey(registryPath);
                    return key.GetValue(valueName);
                }
                else
                {
                    return null;
                }
            }
            catch (System.Security.SecurityException ex)
            {
                MessageBox.Show("Failed to load RegValue: " + registryPath + valueName + "\n SecurityException:" + ex.Message);
                return null;
            }
            catch
            {
                return null;
            }
        }


        public static string read(string ordner, string value)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\VTCManager\" + ordner);
                return key.GetValue(value).ToString();
            }
            catch
            {
                return null;
            }

        }
        public static void write(string name, string wert, string ordner)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key.CreateSubKey("VTCManager");
            key = key.OpenSubKey("VTCManager", true);
            key.CreateSubKey(ordner);
            key = key.OpenSubKey(ordner, true);
            key.SetValue(name, wert);
        }
    }
}
