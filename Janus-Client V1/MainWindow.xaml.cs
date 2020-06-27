using ControlzEx.Theming;
using Janus_Client_V1.Klassen;
using Janus_Client_V1.Spieldaten;
using SCSSdkClient;
using SCSSdkClient.Object;
using System;
using System.Diagnostics;
using System.Windows;
using Metro.Dialogs;

namespace Janus_Client_V1
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        MSG msg = new MSG();
        public Truck_Daten Truck_Daten = new Truck_Daten();
        public SCSSdkTelemetry Telemetry;

        public bool InvokeRequired { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            Lade_Voreinstellungen();
            Telemetry = new SCSSdkTelemetry();
            Telemetry.Data += Telemetry_Data;
            Telemetry.JobStarted += TelemetryOnJobStarted;

            Telemetry.JobCancelled += TelemetryJobCancelled;
            Telemetry.JobDelivered += TelemetryJobDelivered;
            Telemetry.Fined += TelemetryFined;
            Telemetry.Tollgate += TelemetryTollgate;
            Telemetry.Ferry += TelemetryFerry;
            Telemetry.Train += TelemetryTrain;
            Telemetry.RefuelStart += TelemetryRefuel;
            Telemetry.RefuelEnd += TelemetryRefuelEnd;
            Telemetry.RefuelPayed += TelemetryRefuelPayed;

            this.DataContext = Truck_Daten;

            Truck_Daten.CLIENT_VERSION = "Client: 1.0.0";

        }


        private void TelemetryOnJobStarted(object sender, EventArgs e) =>
    MessageBox.Show("Just started job OR loaded game with active.");

        private void TelemetryJobCancelled(object sender, EventArgs e) =>
            MessageBox.Show("Job Cancelled");

        private void TelemetryJobDelivered(object sender, EventArgs e) =>
            MessageBox.Show("Job Delivered");

        private void TelemetryFined(object sender, EventArgs e) =>
            MessageBox.Show("Fined");

        private void TelemetryTollgate(object sender, EventArgs e) =>
            MessageBox.Show("Tollgate");

        private void TelemetryFerry(object sender, EventArgs e) =>
            MessageBox.Show("Ferry");

        private void TelemetryTrain(object sender, EventArgs e) =>
            MessageBox.Show("Train");
        private void TelemetryRefuel(object sender, EventArgs e) =>
            MessageBox.Show("Test");

        private void TelemetryRefuelEnd(object sender, EventArgs e) =>
            MessageBox.Show("Test2");

        private void TelemetryRefuelPayed(object sender, EventArgs e)
        {
            MessageBox.Show("Fuel Payed");
        }
        private void Telemetry_Data(SCSTelemetry data, bool updated)
        {
            try
            {
                if (!InvokeRequired)
                {
                    // ALLGEMEIN
                    Truck_Daten.TELEMETRY_VERSION = "Telemetry: " + data.TelemetryVersion.Major.ToString() + "." + data.TelemetryVersion.Minor.ToString();
                    Truck_Daten.DLL_VERSION = "DLL: " + data.DllVersion.ToString();
                    

                }
            }
            catch
            { }
        }



        private void Lade_Voreinstellungen()
        {
            Farbschema.SelectedValue = REG.Lesen("Config", "Farbschema");
        }

        private void LaunchGitHubSite(object sender, System.Windows.RoutedEventArgs e)
        {
            msg.Schreiben("Fehler", "Diese Funktion wird bald eingebaut...");
        }

        private void Beta_Tester(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("https://projekt-janus.de/beta_bewerbung.php");
            } catch (Exception ex)
            {
                msg.Schreiben("Fehler", "Es ist ein Fehler aufgetreten. Fehlernummer (F1010). Bitte versuche es später erneut oder gib uns diesen Fehlercode in Discord." + ex.Message);
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Spende_einen_Kaffee(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("https://projekt-janus.de/?q=qite8qjA36J3EYMRAffT04CVpymrigEwWqLHJ6dYTJzGTPNbYhTKBiXMV8OVFuXtMg3BPsoNpehvZMpuCous8axNJBjQ6jQWiSeD");
            } catch (Exception ex)
            {
                msg.Schreiben("Fehler", "Diese Funktion wird bald eingebaut..." + ex.Message);
            }
            
        }

        private void MainWindow_Oeffnen(object sender, RoutedEventArgs e)
        {

        }

        private void Seite_Kontakt_oeffnen(object sender, RoutedEventArgs e)
        {

        }

        private void Hauptmenu_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Farbschema_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                ThemeManager.Current.ChangeTheme(this, (string)(Farbschema.SelectedValue));
                REG.Schreiben("Config", "Farbschema", (string)(Farbschema.SelectedValue));
                LeftFlyOut.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                msg.Schreiben("Fehler", "Es ist ein Fehler aufgetreten. Fehlernummer (F1020). Bitte versuche es später erneut oder gib uns diesen Fehlercode in Discord." + ex.Message);
            }
        }


      



    }
}
