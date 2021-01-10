using TrucksLOG.Klassen;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Security.Cryptography;

namespace TrucksLOG
{
    /// <summary>
    /// Interaktionslogik für Pfad_Angeben.xaml
    /// </summary>
    public partial class Pfad_Angeben : Window
    {
        public string startupPath;
        private string initial_ETS;
        private string initial_ATS;
        private string initial_TMP;
        private static string file_original;
        private static byte[] file_orig_bytes;

        public Pfad_Angeben()
        {
            InitializeComponent();

            
                pfad_ets.Text = REG.Lesen("Pfade", "ETS2_PFAD");
                pfad_ets.Text = REG.Lesen("Pfade", "ATS_PFAD");
                pfad_ets.Text = REG.Lesen("Pfade", "ETS2_PFAD");

            if (REG.Lesen("Config", "CLIENT_KEY") != "")
            {
                client_key.Text = REG.Lesen("Config", "CLIENT_KEY");
                client_key.IsEnabled = false;
            }

            if(!string.IsNullOrEmpty(REG.Lesen("Pfade", "ATS_PFAD")) || !string.IsNullOrEmpty(REG.Lesen("Pfade", "ETS2_PFAD")))
            {
                close.Visibility = Visibility.Visible;
            } else
            {
                close.Visibility = Visibility.Hidden;
            }

        }

        private void Tmp_suchen_btn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog tmp = new OpenFileDialog
            {
                InitialDirectory = initial_TMP
            };
            var result_tmp = tmp.ShowDialog();
            if (result_tmp == false) return;
            REG.Schreiben("Pfade", "TMP_PFAD", tmp.FileName);
            pfad_tmp.Text = tmp.FileName;
        }

        private void Ets_suchen_btn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ets = new OpenFileDialog
            {
                Filter = "Euro Truck Simulator 2 (.exe)|eurotrucks2.exe|All Files (*.*)|*.*",
                InitialDirectory = initial_ETS
            };
            var result_ets = ets.ShowDialog();
            if (result_ets == false) return;

            REG.Schreiben("Pfade", "ETS2_PFAD", ets.FileName); ;
            pfad_ets.Text = ets.FileName;

           
           
            string ordner = pfad_ets.Text.Substring(0, pfad_ets.Text.Length - 15);

            if (!File.Exists(ordner + @"\plugins\scs-telemetry.dll"))
            {
                if (Directory.Exists(ordner + "\\plugins"))
                {
                    File.Copy(@"Resources\scs-telemetry.dll", ordner + @"\plugins\scs-telemetry.dll", true);
                }
                else
                {
                    Directory.CreateDirectory(ordner + "\\plugins");
                    File.Copy("Resources\\scs-telemetry.dll", ordner + "\\plugins\\scs-telemetry.dll", true);
                }
            }
        }


        private void Ats_suchen_btn_Click(object sender, RoutedEventArgs e)
        {
            var ats = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "American Truck Simulator (.exe)|amtrucks.exe|All Files (*.*)|*.*",
                InitialDirectory = initial_ATS
            };
            var result_ats = ats.ShowDialog();
            if (result_ats == false) return;

            REG.Schreiben("Pfade", "ATS_PFAD", ats.FileName);
            pfad_ats.Text = ats.FileName;

            
            string ordner = pfad_ats.Text.Substring(0, pfad_ats.Text.Length - 12);


            if (!File.Exists(ordner + @"\plugins\scs-telemetry.dll"))
            {

                if (Directory.Exists(ordner + "\\plugins"))
                {
                    File.Copy("Resources\\scs-telemetry.dll", ordner + "\\plugins\\scs-telemetry.dll", true);
                }
                else
                {
                    Directory.CreateDirectory(ordner + "\\plugins");
                    File.Copy("Resources\\scs-telemetry.dll", ordner + "\\plugins\\scs-telemetry.dll", true);
                }
            }

        }


        private void Abbrechen_click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Pfad_tmp_Loaded(object sender, RoutedEventArgs e)
        {
            pfad_ats.Text = REG.Lesen("Pfade", "ATS_PFAD");
            pfad_ets.Text = REG.Lesen("Pfade", "ETS2_PFAD");
            pfad_tmp.Text = REG.Lesen("Pfade", "TMP_PFAD");

            if (Directory.Exists(@"C:\Program Files (x86)\Steam\steamapps\common\Euro Truck Simulator 2")) { initial_ETS = @"C:\Program Files (x86)\Steam\steamapps\common\Euro Truck Simulator 2"; }
            else if (Directory.Exists(@"C:\Program Files\Steam\steamapps\common\Euro Truck Simulator 2")) { initial_ETS = @"C:\Program Files\Steam\steamapps\common\Euro Truck Simulator 2"; }
            else if (Directory.Exists(@"D:\Program Files (x86)\Steam\steamapps\common\Euro Truck Simulator 2")) { initial_ETS = @"D:\Program Files (x86)\Steam\steamapps\common\Euro Truck Simulator 2"; }
            else if (Directory.Exists(@"D:\Program Files\Steam\steamapps\common\Euro Truck Simulator 2")) { initial_ETS = @"D:\Program Files\Steam\steamapps\common\Euro Truck Simulator 2"; }
            else if (Directory.Exists(@"E:\Program Files (x86)\Steam\steamapps\common\Euro Truck Simulator 2")) { initial_ETS = @"E:\Program Files (x86)\Steam\steamapps\common\Euro Truck Simulator 2"; }
            else if (Directory.Exists(@"E:\Program Files\Steam\steamapps\common\Euro Truck Simulator 2")) { initial_ETS = @"E:\Program Files\Steam\steamapps\common\Euro Truck Simulator 2"; }
            else { initial_ETS = @"C:\"; }

            if (Directory.Exists(@"C:\Program Files (x86)\Steam\steamapps\common\American Truck Simulator")) { initial_ATS = @"C:\Program Files (x86)\Steam\steamapps\common\American Truck Simulator"; }
            else if (Directory.Exists(@"C:\Program Files\Steam\steamapps\common\American Truck Simulator")) { initial_ATS = @"C:\Program Files\Steam\steamapps\common\American Truck Simulator"; }
            else if (Directory.Exists(@"D:\Program Files (x86)\Steam\steamapps\common\American Truck Simulator")) { initial_ATS = @"D:\Program Files (x86)\Steam\steamapps\common\American Truck Simulator"; }
            else if (Directory.Exists(@"D:\Program Files\Steam\steamapps\common\American Truck Simulator")) { initial_ATS = @"D:\Program Files\Steam\steamapps\common\American Truck Simulator"; }
            else if (Directory.Exists(@"E:\Program Files (x86)\Steam\steamapps\common\American Truck Simulator")) { initial_ATS = @"E:\Program Files (x86)\Steam\steamapps\common\American Truck Simulator"; }
            else if (Directory.Exists(@"E:\Program Files\Steam\steamapps\common\American Truck Simulator")) { initial_ATS = @"E:\Program Files\Steam\steamapps\common\American Truck Simulator"; }
            else { initial_ATS = @"C:\"; }

            if (Directory.Exists(@"C:\Program Files\TruckersMP Launcher")) { initial_TMP = @"C:\Program Files\TruckersMP Launcher"; }
            else if (Directory.Exists(@"C:\Program Files (x86)\TruckersMP Launcher")) { initial_TMP = @"C:\Program Files (x86)\TruckersMP Launcher"; }
            else if (Directory.Exists(@"D:\Program Files\TruckersMP Launcher")) { initial_TMP = @"D:\Program Files\TruckersMP Launcher"; }
            else if (Directory.Exists(@"D:\Program Files (x86)\TruckersMP Launcher")) { initial_TMP = @"D:\Program Files (x86)\TruckersMP Launcher"; }
            else if (Directory.Exists(@"E:\Program Files\TruckersMP Launcher")) { initial_TMP = @"E:\Program Files\TruckersMP Launcher"; }
            else if (Directory.Exists(@"E:\Program Files (x86)\TruckersMP Launcher")) { initial_TMP = @"E:\Program Files (x86)\TruckersMP Launcher"; }
            else { initial_TMP = @"C:\"; }
        }

        private void Ok_click(object sender, RoutedEventArgs e)
        {
            if (client_key.Text == "")
            {
                MessageBox.Show("Der Client-Key muss angegeben werden !", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error); 
                return;
            } else
            {
                if (pfad_ets.Text == "" && pfad_ats.Text == "")
                {
                    MessageBox.Show("Entweder ETS2 oder ATS muss angegeben werden !", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    REG.Schreiben("Pfade", "ETS2_PFAD", pfad_ets.Text);
                    REG.Schreiben("Pfade", "ATS_PFAD", pfad_ats.Text);
                    REG.Schreiben("Pfade", "TMP_PFAD", pfad_tmp.Text);


                    Dictionary<string, string> post_param4 = new Dictionary<string, string>
                    {
                        { "CLIENT_KEY", client_key.Text }
                    };
                    string response4 = API.HTTPSRequestPost(API.key_check, post_param4);
                    if (response4 == "NOK")
                    {
                        MessageBox.Show("Der Client-Key kann nicht verifiziert werden !" + Environment.NewLine + "Bitte versuche es Erneut !", "Fehler Client Key", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        REG.Schreiben("Config", "CLIENT_KEY", client_key.Text);
                        MessageBox.Show("Der Client wird jetzt einmal zum Speichern der Daten neu gestartet!", "Reload...", MessageBoxButton.OK, MessageBoxImage.Information);
                        System.Windows.Forms.Application.Restart();
                        System.Windows.Application.Current.Shutdown();
                    }
                }
            }
        }



        private void Where_img_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MessageBox.Show("Du findest deinen Client-Key" + Environment.NewLine + "auf unserer Webseite " + Environment.NewLine + "https://projekt-janus.de" + Environment.NewLine + "in deinem Profil.", "Hilfe...", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Cancel_click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Autosave_suchen_btn_Click(object sender, RoutedEventArgs e)
        {
            var autosave_dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "SII (.sii)|game.sii|All Files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Euro Truck Simulator 2\profiles"
            };

            var result_autosave = autosave_dialog.ShowDialog();
            if (result_autosave == false) return;

            REG.Schreiben("Pfade", "Autosave_Path", autosave_dialog.FileName);
            pfad_autosave.Text = autosave_dialog.FileName;
            string pfad_backup = autosave_dialog.FileName + ".backup";

            File.Copy(autosave_dialog.FileName, pfad_backup);

        }



    }
}
