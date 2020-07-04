using Janus_Client_V1.Klassen;
using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace Janus_Client_V1
{
    /// <summary>
    /// Interaktionslogik für PfadAngabe.xaml
    /// </summary>
    public partial class PfadAngabe : Window
    {
        MSG msg = new MSG();
        public string startupPath;
        private string initial_ETS;
        private string initial_ATS;
        private string initial_TMP;

        public PfadAngabe()
        {
            InitializeComponent();
            
        }


        private void tmp_suchen_btn_Click(object sender, RoutedEventArgs e)
        {
            var tmp = new Microsoft.Win32.OpenFileDialog() { Filter = "TruckersMP (Launcher.exe)|" };
            tmp.InitialDirectory = initial_TMP;
            var result_tmp = tmp.ShowDialog();
            if (result_tmp == false) return;
            REG.Schreiben("Config", "TMP_PFAD", result_tmp.ToString());
        }

        private void ets_suchen_btn_Click(object sender, RoutedEventArgs e)
        {
            var ets = new Microsoft.Win32.OpenFileDialog() { Filter = "EuroTruck (eurotrucks2.exe)|" };
            ets.InitialDirectory = initial_ETS;
            var result_ets = ets.ShowDialog();
            if (result_ets == false) return;
            REG.Schreiben("Config", "ETS2_PFAD", result_ets.ToString());
        }

        private void ats_suchen_btn_Click(object sender, RoutedEventArgs e)
        {
            var ats = new Microsoft.Win32.OpenFileDialog() { Filter = "EuroTruck (amtrucks.exe)|" };
            ats.InitialDirectory = initial_ATS;
            var result_ats = ats.ShowDialog();
            if (result_ats == false) return;
            REG.Schreiben("Config", "ATS_PFAD", result_ats.ToString());
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (pfad_ets.Text == "")
            {
                MessageBox.Show("Der Pfad zu Eurotruck Simulator 2 wurde nicht angegeben!", "Fehler", MessageBoxButton.OK, (MessageBoxImage)MessageBoxIcon.Error);
                e.Cancel = true;
            }
        }

        private void abbrechen_click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void pfad_tmp_Loaded(object sender, RoutedEventArgs e)
        {
            pfad_ats.Text = REG.Lesen("Config", "ATS_PFAD");
            pfad_ets.Text = REG.Lesen("Config", "ETS2_PFAD");
            pfad_tmp.Text = REG.Lesen("Config", "TMP_PFAD");

            if (Directory.Exists(@"C:\Program Files (x86)\Steam\steamapps\common\Euro Truck Simulator 2")) { initial_ETS = @"C:\Program Files (x86)\Steam\steamapps\common\Euro Truck Simulator 2"; }
            else if (Directory.Exists(@"C:\Program Files\Steam\steamapps\common\Euro Truck Simulator 2")) { initial_ETS = @"C:\Program Files\Steam\steamapps\common\Euro Truck Simulator 2"; }
            else if (Directory.Exists(@"D:\Program Files (x86)\Steam\steamapps\common\Euro Truck Simulator 2")) { initial_ETS = @"D:\Program Files (x86)\Steam\steamapps\common\Euro Truck Simulator 2"; }
            else if (Directory.Exists(@"D:\Program Files\Steam\steamapps\common\Euro Truck Simulator 2")) { initial_ETS = @"D:\Program Files\Steam\steamapps\common\Euro Truck Simulator 2"; }
            else if (Directory.Exists(@"E:\Program Files (x86)\Steam\steamapps\common\Euro Truck Simulator 2")) { initial_ETS = @"E:\Program Files (x86)\Steam\steamapps\common\Euro Truck Simulator 2"; }
            else if (Directory.Exists(@"E:\Program Files\Steam\steamapps\common\Euro Truck Simulator 2")) { initial_ETS = @"E:\Program Files\Steam\steamapps\common\Euro Truck Simulator 2"; }

            else { initial_ETS = @"C:\"; }


            if (Directory.Exists(@"C:\Program Files (x86)\Steam\steamapps\common\American Truck Simulator")) { initial_ATS = @"C:\Program Files (x86)\Steam\steamapps\common\American Truck Simulator"; }
            if (Directory.Exists(@"C:\Program Files\Steam\steamapps\common\American Truck Simulator")) { initial_ATS = @"C:\Program Files\Steam\steamapps\common\American Truck Simulator"; }
            else if (Directory.Exists(@"D:\Program Files (x86)\Steam\steamapps\common\American Truck Simulator")) { initial_ATS = @"D:\Program Files (x86)\Steam\steamapps\common\American Truck Simulator"; }
            if (Directory.Exists(@"D:\Program Files\Steam\steamapps\common\American Truck Simulator")) { initial_ATS = @"D:\Program Files\Steam\steamapps\common\American Truck Simulator"; }
            else if (Directory.Exists(@"E:\Program Files (x86)\Steam\steamapps\common\American Truck Simulator")) { initial_ATS = @"E:\Program Files (x86)\Steam\steamapps\common\American Truck Simulator"; }
            if (Directory.Exists(@"E:\Program Files\Steam\steamapps\common\American Truck Simulator")) { initial_ATS = @"E:\Program Files\Steam\steamapps\common\American Truck Simulator"; }
            else { initial_ATS = @"C:\"; }

            if (Directory.Exists(@"C:\Program Files\TruckersMP Launcher")) { initial_TMP = @"C:\Program Files\TruckersMP Launcher"; }
            else if (Directory.Exists(@"C:\Program Files (x86)\TruckersMP Launcher")) { initial_ATS = @"C:\Program Files (x86)\TruckersMP Launcher"; }
            else if (Directory.Exists(@"D:\Program Files\TruckersMP Launcher")) { initial_ATS = @"D:\Program Files\TruckersMP Launcher"; }
            else if (Directory.Exists(@"D:\Program Files (x86)\TruckersMP Launcher")) { initial_ATS = @"D:\Program Files (x86)\TruckersMP Launcher"; }
            else if (Directory.Exists(@"E:\Program Files\TruckersMP Launcher")) { initial_ATS = @"E:\Program Files\TruckersMP Launcher"; }
            else if (Directory.Exists(@"E:\Program Files (x86)\TruckersMP Launcher")) { initial_ATS = @"E:\Program Files (x86)\TruckersMP Launcher"; }
            else { initial_ATS = @"C:\"; }
        }
    }
}
