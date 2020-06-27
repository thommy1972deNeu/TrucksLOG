﻿using Microsoft.Win32;

namespace Janus_Client_V1.Klassen
{
    public class REG
    {
        public static string Lesen(string ordner, string name)
        {
            RegistryKey myKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Projekt-Janus\\" + ordner);
            return (string)myKey.GetValue(name);
        }

        public static void Schreiben(string ordner, string name, string wert)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key.CreateSubKey("Projekt-Janus");
            key = key.OpenSubKey("Projekt-Janus", true);
            key.CreateSubKey(ordner);
            key = key.OpenSubKey(ordner, true);
            key.SetValue(name, wert);
        }

    }



}
