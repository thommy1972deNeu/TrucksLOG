using Janus_Client_V1.Klassen;
using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace Janus_Client_V1
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

        public Pfad_Angeben()
        {
            InitializeComponent();
            if(string.IsNullOrEmpty(REG.Lesen("Pfade", "ETS2_PFAD")))
            {
                REG.Schreiben("Pfade", "ETS2_PFAD", "");
            } else
            {
                pfad_ets.Text = REG.Lesen("Pfade", "ETS2_PFAD");
            }
           

            if (string.IsNullOrEmpty(REG.Lesen("Pfade", "ATS_PFAD")))
            {
                REG.Schreiben("Pfade", "ATS_PFAD", "");
            }
            else
            {
                pfad_ets.Text = REG.Lesen("Pfade", "ATS_PFAD");
            }


            if (string.IsNullOrEmpty(REG.Lesen("Pfade", "TMP_PFAD")))
            {
                REG.Schreiben("Pfade", "ATS_PFAD", "");
            }
            else
            {
                pfad_ets.Text = REG.Lesen("Pfade", "ETS2_PFAD");
            }

        }

        private void tmp_suchen_btn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog tmp = new OpenFileDialog();
            tmp.InitialDirectory = initial_TMP;
            var result_tmp = tmp.ShowDialog();
            if (result_tmp == false) return;
            REG.Schreiben("Pfade", "TMP_PFAD", tmp.FileName);
            pfad_tmp.Text = tmp.FileName;
        }

        private void ets_suchen_btn_Click(object sender, RoutedEventArgs e)
        {
            var ets = new Microsoft.Win32.OpenFileDialog();
            ets.Filter = "Euro Truck Simulator 2 (.exe)|eurotrucks2.exe|All Files (*.*)|*.*";
            ets.InitialDirectory = initial_ETS;
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


        private void ats_suchen_btn_Click(object sender, RoutedEventArgs e)
        {
            var ats = new Microsoft.Win32.OpenFileDialog();
            ats.Filter = "American Truck Simulator (.exe)|amtrucks.exe|All Files (*.*)|*.*";
            ats.InitialDirectory = initial_ATS;
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


        private void abbrechen_click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void pfad_tmp_Loaded(object sender, RoutedEventArgs e)
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

        private void ok_click(object sender, RoutedEventArgs e)
        {
            if (pfad_ets.Text == "" && pfad_ats.Text == "")
            {
                MessageBox.Show("Der ETS2 oder ATS Pfad muss angegeben werden !", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error); 
                return;
            } else
            {
                REG.Schreiben("Pfade", "ETS2_PFAD", pfad_ets.Text);
                REG.Schreiben("Pfade", "ATS_PFAD", pfad_ats.Text);
                REG.Schreiben("Pfade", "TMP_PFAD", pfad_tmp.Text);

                MessageBox.Show("Der Client wird jetzt einmal neu gestartet!", "Reload", MessageBoxButton.OK, MessageBoxImage.Information);
                System.Windows.Forms.Application.Restart();
                System.Windows.Application.Current.Shutdown();
            }
        }


    }
}
