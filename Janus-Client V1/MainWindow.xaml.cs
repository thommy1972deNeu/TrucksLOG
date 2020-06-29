﻿using ControlzEx.Theming;
using Janus_Client_V1.Klassen;
using Janus_Client_V1.Spieldaten;
using SCSSdkClient;
using SCSSdkClient.Object;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

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
        DispatcherTimer job_update_timer = new DispatcherTimer();
        public bool InvokeRequired { get; private set; }

        public MainWindow()
        {
            InitializeComponent();


            job_update_timer.Interval = TimeSpan.FromSeconds(5);
            job_update_timer.Tick += timer_Tick;


            if (string.IsNullOrEmpty(REG.Lesen("Config", "CLIENT_KEY")))
            {
                CLIENT_KEY_ABFRAGE form = new CLIENT_KEY_ABFRAGE();
                form.ShowDialog();
                return;
            }
            else
            {

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
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            Dictionary<string, string> post_param = new Dictionary<string, string>();
            post_param.Add("TOUR_ID", REG.Lesen("Config", "TOUR_ID"));
            post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
            post_param.Add("REST_KM", ((float)Truck_Daten.REST_KM / 1000).ToString());
            string response = API.HTTPSRequestPost(API.job_update, post_param);
            Console.WriteLine(response);
        }

        private void TelemetryOnJobStarted(object sender, EventArgs e)
        {
            
            if (!Truck_Daten.CARGO_LOADED)
            {
                Stopwatch stopWatch = new Stopwatch(); //as timeout
                stopWatch.Start();
                while (String.IsNullOrWhiteSpace(Truck_Daten.STARTORT) && stopWatch.ElapsedMilliseconds < 5000)
                {
                    Console.WriteLine("Waiting for tour data to init");
                }
                stopWatch.Stop();
                Console.WriteLine("Wait for data took " + stopWatch.ElapsedMilliseconds + " ms");
                if (String.IsNullOrWhiteSpace(Truck_Daten.STARTORT) && stopWatch.ElapsedMilliseconds < 5000)
                {
                    MessageBox.Show("Could not get required data. Job couldn't start.", "ERROR", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            var ckey = REG.Lesen("Config", "CLIENT_KEY");
            REG.Schreiben("Config", "TOUR_ID", Truck_Daten.STARTORT_ID + Truck_Daten.ZIELFIRMA_ID + Truck_Daten.ZIELORT_ID);

            Dictionary<string, string> post_param = new Dictionary<string, string>();
            post_param.Add("TOUR_ID", REG.Lesen("Config", "TOUR_ID"));
            post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
            post_param.Add("STARTORT", Truck_Daten.STARTORT.ToString());
            post_param.Add("STARTORT_ID", Truck_Daten.STARTORT_ID);
            post_param.Add("STARTFIRMA", Truck_Daten.STARTFIRMA);
            post_param.Add("STARTFIRMA_ID", Truck_Daten.STARTFIRMA_ID);
            post_param.Add("ZIELORT", Truck_Daten.ZIELORT);
            post_param.Add("ZIELORT_ID", Truck_Daten.ZIELORT_ID);
            post_param.Add("ZIELFIRMA", Truck_Daten.ZIELFIRMA);
            post_param.Add("ZIELFIRMA_ID", Truck_Daten.ZIELFIRMA_ID);
            post_param.Add("LADUNG", Truck_Daten.LADUNG_NAME);
            post_param.Add("LADUNG_ID", Truck_Daten.LADUNG_ID);
            post_param.Add("GEWICHT", ((float)Truck_Daten.GEWICHT/1000).ToString());
            post_param.Add("EINKOMMEN", Truck_Daten.EINKOMMEN.ToString());
            post_param.Add("FRACHTMARKT", Truck_Daten.FRACHTMARKT);
            post_param.Add("LKW_MODELL", Truck_Daten.LKW_MODELL);
            post_param.Add("LKW_HERSTELLER", Truck_Daten.LKW_HERSTELLER);
            post_param.Add("LKW_HERSTELLER_ID", Truck_Daten.LKW_HERSTELLER_ID);
            post_param.Add("GESAMT_KM", ((float)Truck_Daten.GESAMT_KM).ToString());
            post_param.Add("REST_KM", ((float)Truck_Daten.REST_KM/1000).ToString());
            post_param.Add("STATUS", "In Auslieferung");
            post_param.Add("SPIEL", Truck_Daten.SPIEL);

            string response = API.HTTPSRequestPost(API.job_started, post_param);
            job_update_timer.Start();
        }

        private void TelemetryJobCancelled(object sender, EventArgs e)
        {
            Dictionary<string, string> post_param = new Dictionary<string, string>();

            post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
            post_param.Add("TOUR_ID", REG.Lesen("Config", "TOUR_ID"));

            string response = API.HTTPSRequestPost(API.job_cancel, post_param);
            REG.Schreiben("Config", "TOUR_ID", "");
            job_update_timer.Stop();

        }

        private void TelemetryJobDelivered(object sender, EventArgs e)
        {
            Dictionary<string, string> post_param = new Dictionary<string, string>();

            post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
            post_param.Add("TOUR_ID", REG.Lesen("Config", "TOUR_ID"));

            string response = API.HTTPSRequestPost(API.job_finish, post_param);
            REG.Schreiben("Config", "TOUR_ID", "");
            job_update_timer.Stop();
        }
           

        private void TelemetryFined(object sender, EventArgs e)
        {
            Dictionary<string, string> post_param = new Dictionary<string, string>();

            post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
            post_param.Add("BETRAG", Truck_Daten.BETRAG.ToString());
            post_param.Add("GRUND", Truck_Daten.GRUND);
            string response = API.HTTPSRequestPost(API.strafe, post_param);
            msg.Schreiben("Strafe erhalten", "Du musstest " + Truck_Daten.BETRAG + " € Strafe bezahlen");

        }

        private void TelemetryTollgate(object sender, EventArgs e)
        {
            Dictionary<string, string> post_param = new Dictionary<string, string>();

            post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
            post_param.Add("BETRAG", Truck_Daten.MAUT_BETRAG.ToString());
            string response = API.HTTPSRequestPost(API.tollgate, post_param);
            msg.Schreiben("Mautstation", "Du musstest an der Mautstation" + Truck_Daten.MAUT_BETRAG + " € bezahlen");
        }

        private void TelemetryFerry(object sender, EventArgs e)
        {

        }

        private void TelemetryTrain(object sender, EventArgs e)
        {
        }

        private void TelemetryRefuel(object sender, EventArgs e) 
        {
        }


        private void TelemetryRefuelEnd(object sender, EventArgs e)
        {
            Dictionary<string, string> post_param = new Dictionary<string, string>();

            post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
            post_param.Add("BETRAG", Truck_Daten.TANK_BETRAG.ToString());
            string response = API.HTTPSRequestPost(API.tanken, post_param);
            MessageBox.Show(Truck_Daten.TANK_BETRAG.ToString());
        }

        private void TelemetryRefuelPayed(object sender, EventArgs e)
        {
            Dictionary<string, string> post_param = new Dictionary<string, string>();

            post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
            post_param.Add("BETRAG", Truck_Daten.TANK_BETRAG.ToString());
            string response = API.HTTPSRequestPost(API.tanken, post_param);
            MessageBox.Show(Truck_Daten.TANK_BETRAG.ToString());
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

                    // Tour TEST
                    Truck_Daten.STARTORT = data.JobValues.CitySource;
                    Truck_Daten.STARTORT_ID = data.JobValues.CitySourceId;
                    Truck_Daten.STARTFIRMA = data.JobValues.CompanySource;
                    Truck_Daten.STARTFIRMA_ID = data.JobValues.CompanySourceId;
                    Truck_Daten.ZIELORT = data.JobValues.CityDestination;
                    Truck_Daten.ZIELORT_ID = data.JobValues.CityDestinationId;
                    Truck_Daten.ZIELFIRMA = data.JobValues.CompanyDestination;
                    Truck_Daten.ZIELFIRMA_ID = data.JobValues.CompanyDestinationId;
                    Truck_Daten.LADUNG_NAME = data.JobValues.CargoValues.Name;
                    Truck_Daten.LADUNG_ID = data.JobValues.CargoValues.Id;
                    Truck_Daten.GEWICHT = (float)data.JobValues.CargoValues.Mass/1000;
                    Truck_Daten.GESAMT_KM = (float)data.JobValues.PlannedDistanceKm;
                    Truck_Daten.REST_KM = (float)data.NavigationValues.NavigationDistance;
                    Truck_Daten.EINKOMMEN = (int)data.JobValues.Income;
                    Truck_Daten.FRACHTMARKT = data.JobValues.Market.ToString();
                    Truck_Daten.LKW_MODELL = data.TruckValues.ConstantsValues.Name;
                    Truck_Daten.LKW_HERSTELLER = data.TruckValues.ConstantsValues.Brand;
                    Truck_Daten.LKW_HERSTELLER_ID = data.TruckValues.ConstantsValues.BrandId;
                    Truck_Daten.SPEED = (int)data.TruckValues.CurrentValues.DashboardValues.Speed.Kph;
                    Truck_Daten.SPIEL = data.Game.ToString();

                    Truck_Daten.BETRAG = (int)data.GamePlay.FinedEvent.Amount;
                    Truck_Daten.GRUND = data.GamePlay.FinedEvent.Offence.ToString();

                    Truck_Daten.MAUT_BETRAG = (int)data.GamePlay.TollgateEvent.PayAmount;
                    Truck_Daten.TANK_BETRAG = data.GamePlay.RefuelEvent.Amount;

                    Truck_Daten.FAHRINFO_1 = "Du fährst mit " + Truck_Daten.GEWICHT + " Tonnen " + Truck_Daten.LADUNG_NAME + " von " + Truck_Daten.STARTORT + " nach " + Truck_Daten.ZIELORT;
                    Truck_Daten.FAHRINFO_2 = "Du musst noch " + (int)Truck_Daten.REST_KM/1000 + " KM von insgesamt " + Truck_Daten.GESAMT_KM + " KM fahren";

                }
            }
            catch
            { }
        }



        private void Lade_Voreinstellungen()
        {
            Farbschema.SelectedValue = REG.Lesen("Config", "Farbschema");

            if (string.IsNullOrWhiteSpace(REG.Lesen("Config", "TOUR_ID")))
                REG.Schreiben("Config", "TOUR_ID", "");
            if (string.IsNullOrWhiteSpace(REG.Lesen("Config", "CLIENT_KEY")))
                REG.Schreiben("Config", "CLIENT_KEY", ""); 

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