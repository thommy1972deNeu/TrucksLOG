using Janus_Client_V1.Spieldaten;
using System;
using System.Collections.Generic;
using System.IO;
using VdfParser;

namespace Janus_Client_V1.Klassen
{
    public class TelemetryInstaller
    {
        //ETS
        private static string Folder64 = @"win_x64\";
        private static string Folder86 = @"win_x86\";
        private static string SteamLibraryConfigFile = @"\steamapps\libraryfolders.vdf";

        public static void install()
        {
            String SteamInstallPath = RegistryHandler.Globalread(RegistryHandler.Steam64bitRegistry, RegistryHandler.SteamInstallPathValueName).ToString();
            if (!string.IsNullOrEmpty(SteamInstallPath))
            {
                if (!File.Exists(SteamInstallPath + @"\steamapps\common\Euro Truck Simulator 2\bin\" + Folder86 + "eurotrucks2.exe") && !File.Exists(SteamInstallPath + @"\steamapps\common\Euro Truck Simulator 2\bin\" + Folder64 + "eurotrucks2.exe"))
                {
                    String SteamLibraryConfigPath = SteamInstallPath + SteamLibraryConfigFile;
                    if (File.Exists(SteamLibraryConfigPath))
                    {
                        string testFile = File.ReadAllText(SteamLibraryConfigPath);
                        VdfDeserializer deserializer = new VdfDeserializer();
                        dynamic result = deserializer.Deserialize(testFile);
                        IDictionary<string, object> result_dictionary = result.LibraryFolders;
                        List<string> SteamLibraries = new List<string>();
                        for (int i = 1; i < 5; i++)
                        {
                            if (result_dictionary.ContainsKey(i.ToString()))
                            {
                                SteamLibraries.Add(result_dictionary[i.ToString()].ToString());
                            }
                        }
                        foreach (String Steampath in SteamLibraries)
                        {

                            if (File.Exists(Steampath + @"\steamapps\common\Euro Truck Simulator 2\bin\" + Folder86 + "eurotrucks2.exe") && File.Exists(Steampath + @"\steamapps\common\Euro Truck Simulator 2\bin\" + Folder64 + "eurotrucks2.exe"))
                            {
                                if (!Directory.Exists(Steampath + @"\steamapps\common\Euro Truck Simulator 2\bin\" + Folder64 + "plugins"))
                                    Directory.CreateDirectory(Steampath + @"\steamapps\common\Euro Truck Simulator 2\bin\" + Folder64 + "plugins");
                                if (File.Exists(Steampath + @"\steamapps\common\Euro Truck Simulator 2\bin\" + Folder64 + @"plugins\scs-telemetry.dll"))
                                    File.Delete(Steampath + @"\steamapps\common\Euro Truck Simulator 2\bin\" + Folder64 + @"plugins\scs-telemetry.dll");
                                File.Copy(@"Resources/scs-telemetry.dll", Steampath + @"\steamapps\common\Euro Truck Simulator 2\bin\" + Folder64 + @"plugins\scs-telemetry.dll");

                           

                                if (!Directory.Exists(Steampath + @"\steamapps\common\Euro Truck Simulator 2\bin\" + Folder86 + "plugins"))
                                    Directory.CreateDirectory(Steampath + @"\steamapps\common\Euro Truck Simulator 2\bin\" + Folder86 + "plugins");
                                if (File.Exists(Steampath + @"\steamapps\common\Euro Truck Simulator 2\bin\" + Folder86 + @"plugins\scs-telemetry.dll"))
                                    File.Delete(Steampath + @"\steamapps\common\Euro Truck Simulator 2\bin\" + Folder86 + @"plugins\scs-telemetry.dll");
                                File.Copy(@"Resources/scs-telemetry.dll", Steampath + @"\steamapps\common\Euro Truck Simulator 2\bin\" + Folder86 + @"plugins\scs-telemetry.dll");
                            }
                        }
                    }
                    else
                    {
                        Pfad_Angeben pathwindow = new Pfad_Angeben();
                        pathwindow.Show();
                    }
                }
                else
                {
                    if (!Directory.Exists(SteamInstallPath + @"\steamapps\common\Euro Truck Simulator 2\bin\" + Folder86 + "plugins"))
                        Directory.CreateDirectory(SteamInstallPath + @"\steamapps\common\Euro Truck Simulator 2\bin\" + Folder86 + "plugins");
                    if (File.Exists(SteamInstallPath + @"\steamapps\common\Euro Truck Simulator 2\bin\" + Folder86 + @"plugins\scs-telemetry.dll"))
                        File.Delete(SteamInstallPath + @"\steamapps\common\Euro Truck Simulator 2\bin\" + Folder86 + @"plugins\scs-telemetry.dll");
                    File.Copy(@"Resources/scs-telemetry.dll", SteamInstallPath + @"\steamapps\common\Euro Truck Simulator 2\bin\" + Folder86 + @"plugins\scs-telemetry.dll");
                    REG.Schreiben("Pfade", "ETS2_PFAD", SteamInstallPath + @"\steamapps\common\Euro Truck Simulator 2\bin\" + Folder86 + "eurotrucks2.exe");

                    if (!Directory.Exists(SteamInstallPath + @"\steamapps\common\Euro Truck Simulator 2\bin\" + Folder64 + "plugins"))
                        Directory.CreateDirectory(SteamInstallPath + @"\steamapps\common\Euro Truck Simulator 2\bin\" + Folder64 + "plugins");
                    if (File.Exists(SteamInstallPath + @"\steamapps\common\Euro Truck Simulator 2\bin\" + Folder64 + @"plugins\scs-telemetry.dll"))
                        File.Delete(SteamInstallPath + @"\steamapps\common\Euro Truck Simulator 2\bin\" + Folder64 + @"plugins\scs-telemetry.dll");
                    File.Copy(@"Resources/scs-telemetry.dll", SteamInstallPath + @"\steamapps\common\Euro Truck Simulator 2\bin\" + Folder64 + @"plugins\scs-telemetry.dll");
                    REG.Schreiben("Pfade", "ETS2_PFAD", SteamInstallPath + @"\steamapps\common\Euro Truck Simulator 2\bin\" + Folder64 + "eurotrucks2.exe");

                }
            }
            else
            {
                Pfad_Angeben pathwindow = new Pfad_Angeben();
                pathwindow.Show();
            }
        }

        // ATS
        public static void install_ATS()
        {
            String SteamInstallPath = RegistryHandler.Globalread(RegistryHandler.Steam64bitRegistry, RegistryHandler.SteamInstallPathValueName).ToString();
            if (!string.IsNullOrEmpty(SteamInstallPath))
            {
                if (!File.Exists(SteamInstallPath + @"\steamapps\common\American Truck Simulator\bin\" + Folder86 + "amtrucks.exe") && !File.Exists(SteamInstallPath + @"\steamapps\common\American Truck Simulator\bin\" + Folder64 + "amtrucks.exe"))
                {
                    String SteamLibraryConfigPath = SteamInstallPath + SteamLibraryConfigFile;
                    if (File.Exists(SteamLibraryConfigPath))
                    {
                        string testFile = File.ReadAllText(SteamLibraryConfigPath);
                        VdfDeserializer deserializer = new VdfDeserializer();
                        dynamic result = deserializer.Deserialize(testFile);
                        IDictionary<string, object> result_dictionary = result.LibraryFolders;
                        List<string> SteamLibraries = new List<string>();
                        for (int i = 1; i < 5; i++)
                        {
                            if (result_dictionary.ContainsKey(i.ToString()))
                            {
                                SteamLibraries.Add(result_dictionary[i.ToString()].ToString());
                            }
                        }
                        foreach (String Steampath in SteamLibraries)
                        {

                            if (File.Exists(Steampath + @"\steamapps\common\American Truck Simulator\bin\" + Folder86 + "amtrucks.exe") && File.Exists(Steampath + @"\steamapps\common\American Truck Simulator\bin\" + Folder64 + "amtrucks.exe"))
                            {
                                if (!Directory.Exists(Steampath + @"\steamapps\common\American Truck Simulator\bin\" + Folder64 + "plugins"))
                                    Directory.CreateDirectory(Steampath + @"\steamapps\common\American Truck Simulator\bin\" + Folder64 + "plugins");
                                if (File.Exists(Steampath + @"\steamapps\common\American Truck Simulator\bin\" + Folder64 + @"plugins\scs-telemetry.dll"))
                                    File.Delete(Steampath + @"\steamapps\common\American Truck Simulator\bin\" + Folder64 + @"plugins\scs-telemetry.dll");
                                File.Copy(@"Resources/scs-telemetry.dll", Steampath + @"\steamapps\common\American Truck Simulator\bin\" + Folder64 + @"plugins\scs-telemetry.dll");

                                if (!Directory.Exists(Steampath + @"\steamapps\common\American Truck Simulator\bin\" + Folder86 + "plugins"))
                                    Directory.CreateDirectory(Steampath + @"\steamapps\common\American Truck Simulator\bin\" + Folder86 + "plugins");
                                if (File.Exists(Steampath + @"\steamapps\common\American Truck Simulator\bin\" + Folder86 + @"plugins\scs-telemetry.dll"))
                                    File.Delete(Steampath + @"\steamapps\common\American Truck Simulator\bin\" + Folder86 + @"plugins\scs-telemetry.dll");
                                File.Copy(@"Resources/scs-telemetry.dll", Steampath + @"\steamapps\common\American Truck Simulator\bin\" + Folder86 + @"plugins\scs-telemetry.dll"); 
                            }
                        }
                    }
                    else
                    {
                        Pfad_Angeben pathwindow = new Pfad_Angeben();
                        pathwindow.Show();
                    }
                }
                else
                {

                    if (!Directory.Exists(SteamInstallPath + @"\steamapps\common\American Truck Simulator\bin\" + Folder86 + "plugins"))
                        Directory.CreateDirectory(SteamInstallPath + @"\steamapps\common\American Truck Simulator\bin\" + Folder86 + "plugins");
                    if (File.Exists(SteamInstallPath + @"\steamapps\common\American Truck Simulator\bin\" + Folder86 + @"plugins\scs-telemetry.dll"))
                        File.Delete(SteamInstallPath + @"\steamapps\common\American Truck Simulator\bin\" + Folder86 + @"plugins\scs-telemetry.dll");
                    File.Copy(@"Resources/scs-telemetry.dll", SteamInstallPath + @"\steamapps\common\American Truck Simulator\bin\" + Folder86 + @"plugins\scs-telemetry.dll");
                    REG.Schreiben("Pfade", "ATS_PFAD", SteamInstallPath + @"\steamapps\common\American Truck Simulator\bin\" + Folder86 + "amtrucks.exe");

                    if (!Directory.Exists(SteamInstallPath + @"\steamapps\common\American Truck Simulator\bin\" + Folder64 + "plugins"))
                        Directory.CreateDirectory(SteamInstallPath + @"\steamapps\common\American Truck Simulator\bin\" + Folder64 + "plugins");
                    if (File.Exists(SteamInstallPath + @"\steamapps\common\American Truck Simulator\bin\" + Folder64 + @"plugins\scs-telemetry.dll"))
                        File.Delete(SteamInstallPath + @"\steamapps\common\American Truck Simulator\bin\" + Folder64 + @"plugins\scs-telemetry.dll");
                    File.Copy(@"Resources/scs-telemetry.dll", SteamInstallPath + @"\steamapps\common\American Truck Simulator\bin\" + Folder64 + @"plugins\scs-telemetry.dll");
                    REG.Schreiben("Pfade", "ATS_PFAD", SteamInstallPath + @"\steamapps\common\American Truck Simulator\bin\" + Folder64 + "amtrucks.exe");


                }
            }
            else
            {
                Pfad_Angeben pathwindow = new Pfad_Angeben();
                pathwindow.Show();
            }
        }

        // ATS ENDE



        public static void check_ETS()
        {
            String telemetryVersion = "";
            String telemetryETS2Path = "";
            try
            {
                telemetryETS2Path = REG.Lesen("Pfade", "ETS2_PFAD");
                telemetryVersion = REG.Lesen("Pfade", "Telemetry_Version");
            }
            catch { }

            if (String.IsNullOrWhiteSpace(telemetryETS2Path) || String.IsNullOrWhiteSpace(telemetryVersion))
            {
                Console.WriteLine("running installation of Telemetry");
                install();
            }
            else if (REG.Lesen("Pfade", "Telemetry_Version") != Config.dll_Version || !File.Exists(REG.Lesen("Pfade", "ETS2_PFAD") + @"bin\" + Folder86 + "eurotrucks2.exe") || !File.Exists(REG.Lesen("Pfade", "ETS2_PFAD") + @"bin\" + Folder64 + "eurotrucks2.exe"))
            {
                install();
            }
            else if (!File.Exists(REG.Lesen("Pfade", "ETS2_PFAD") + @"bin\" + Folder86 + @"plugins\scs-telemetry.dll") || !File.Exists(REG.Lesen("Pfade", "ETS2_PFAD") + @"bin\" + Folder64 + @"plugins\scs-telemetry.dll"))
            {
                install();
            }

        }

        public static void check_ATS()
        {
            String telemetryVersion = "";
            String telemetryATSPath = "";
            try
            {
                telemetryATSPath = REG.Lesen("Pfade", "ATS_PFAD");
                telemetryVersion = REG.Lesen("Pfade", "Telemetry_Version");
            }
            catch { }

            if (String.IsNullOrWhiteSpace(telemetryATSPath) || String.IsNullOrWhiteSpace(telemetryVersion))
            {
                install_ATS();
            }
            else if (REG.Lesen("Pfade", "Telemetry_Version") != Config.dll_Version || !File.Exists(REG.Lesen("Pfade", "ATS_PFAD") + @"bin\" + Folder86 + "amtrucks.exe") || !File.Exists(REG.Lesen("Pfade", "ATS_PFAD") + @"bin\" + Folder64 + "amtrucks.exe"))
            {
                install_ATS();
            }
            else if (!File.Exists(REG.Lesen("Pfade", "ATS_PFAD") + @"bin\" + Folder86 + @"plugins\scs-telemetry.dll") || !File.Exists(REG.Lesen("Pfade", "ATS_PFAD") + @"bin\" + Folder64 + @"plugins\scs-telemetry.dll"))
            {
                install_ATS();
            }

        }


    }
}
