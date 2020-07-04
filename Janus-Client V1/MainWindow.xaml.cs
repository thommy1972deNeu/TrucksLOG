using ControlzEx.Theming;
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
            Logging.Make_Log_File();


            Logging.WriteClientLog("Version: " + Truck_Daten.CLIENT_VERSION);

            job_update_timer.Interval = TimeSpan.FromSeconds(5);

            if (string.IsNullOrEmpty(REG.Lesen("Config", "CLIENT_KEY")))
            {
                CLIENT_KEY_ABFRAGE form = new CLIENT_KEY_ABFRAGE();
                form.ShowDialog();
                return;
            }
            else
            {

                if (string.IsNullOrEmpty(REG.Lesen("Config", "ETS2_PFAD")))
                {
                     PfadAngabe pf = new PfadAngabe();
                     pf.ShowDialog();
                    return;
                }




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

                if (REG.Lesen("Config", "Systemsounds") == "An")
                    SoundPlayer.Sound_Willkommen();

                this.DataContext = Truck_Daten;



            }
        }




        private void timer_Tick(object sender, EventArgs e)
        {
            Dictionary<string, string> post_param = new Dictionary<string, string>();
            post_param.Add("TOUR_ID", REG.Lesen("Config", "TOUR_ID"));
            post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
            post_param.Add("REST_KM", ((float)Truck_Daten.REST_KM / 1000).ToString());
            post_param.Add("FRACHTSCHADEN", Truck_Daten.FRACHTSCHADEN.ToString());
            string response = API.HTTPSRequestPost(API.job_update, post_param);
            Console.WriteLine(response);
            Logging.WriteClientLog("Tour Update:" + response);
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

            REG.Schreiben("Config", "TOUR_ID", GenerateString());

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
            post_param.Add("GEWICHT", Truck_Daten.GEWICHT.ToString());
            post_param.Add("EINKOMMEN", Truck_Daten.EINKOMMEN.ToString());
            post_param.Add("FRACHTMARKT", Truck_Daten.FRACHTMARKT);
            post_param.Add("LKW_MODELL", Truck_Daten.LKW_MODELL);
            post_param.Add("LKW_HERSTELLER", Truck_Daten.LKW_HERSTELLER);
            post_param.Add("LKW_HERSTELLER_ID", Truck_Daten.LKW_HERSTELLER_ID);
            post_param.Add("GESAMT_KM", ((float)Truck_Daten.GESAMT_KM).ToString());
            post_param.Add("REST_KM", ((float)Truck_Daten.REST_KM / 1000).ToString());
            post_param.Add("SPIEL", Truck_Daten.SPIEL);
            post_param.Add("FRACHTSCHADEN", Truck_Daten.FRACHTSCHADEN.ToString());

            string response = API.HTTPSRequestPost(API.job_started, post_param);
            Console.WriteLine(response);
            REG.Schreiben("Config", "Frachtmarkt", Truck_Daten.FRACHTMARKT);

            if (REG.Lesen("Config", "Systemsounds") == "An") SoundPlayer.Sound_Tour_Gestartet();
            job_update_timer.Tick += timer_Tick;
            job_update_timer.Start();

            Logging.WriteClientLog("Tour gestartet: " + response);
        }

        private void TelemetryJobCancelled(object sender, EventArgs e)
        {
            Dictionary<string, string> post_param = new Dictionary<string, string>();
            post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
            post_param.Add("TOUR_ID", REG.Lesen("Config", "TOUR_ID"));
            post_param.Add("FRACHTMARKT", Truck_Daten.FRACHTMARKT);
            string response = API.HTTPSRequestPost(API.job_cancel, post_param);
            Console.WriteLine(response);
            REG.Schreiben("Config", "TOUR_ID", "");
            if (REG.Lesen("Config", "Systemsounds") == "An") SoundPlayer.Sound_Tour_Abgebrochen();

            job_update_timer.Stop();
            Logging.WriteClientLog("Tour abgebrochen: " + response);
        }

        private void TelemetryJobDelivered(object sender, EventArgs e)
        {
            Dictionary<string, string> post_param = new Dictionary<string, string>();

            post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
            post_param.Add("TOUR_ID", REG.Lesen("Config", "TOUR_ID"));
            post_param.Add("FRACHTSCHADEN", Truck_Daten.FRACHTSCHADEN_ABGABE.ToString());
            post_param.Add("STRECKE", Truck_Daten.GEF_STRECKE.ToString());

            string response = API.HTTPSRequestPost(API.job_finish, post_param);
            Console.WriteLine(response);

            REG.Schreiben("Config", "TOUR_ID", "");
            if (REG.Lesen("Config", "Systemsounds") == "An") SoundPlayer.Sound_Tour_Beendet();

            job_update_timer.Stop();
            Logging.WriteClientLog("Tour Abgeliefert: " + response);
        }


        private void TelemetryFined(object sender, EventArgs e)
        {
            Dictionary<string, string> post_param = new Dictionary<string, string>();
            post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
            post_param.Add("BETRAG", Truck_Daten.STRAF_BETRAG.ToString());
            post_param.Add("GRUND", Truck_Daten.GRUND);
            string response = API.HTTPSRequestPost(API.strafe, post_param);
            if (REG.Lesen("Config", "Systemsounds") == "An") SoundPlayer.Sound_Strafe_Erhalten();
            Logging.WriteClientLog("Strafe erhalten: " + response);
        }

        private void TelemetryTollgate(object sender, EventArgs e)
        {
            Dictionary<string, string> post_param = new Dictionary<string, string>();
            post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
            post_param.Add("BETRAG", Truck_Daten.MAUT_BETRAG.ToString());
            string response = API.HTTPSRequestPost(API.tollgate, post_param);
            Console.WriteLine(response);

            if (REG.Lesen("Config", "Systemsounds") == "An") SoundPlayer.Sound_Mautstation_Passiert();
            Logging.WriteClientLog("Maut durchfahren: " + response);
        }

        private void TelemetryFerry(object sender, EventArgs e)
        {
            Logging.WriteClientLog("Fähre genommen");
        }

        private void TelemetryTrain(object sender, EventArgs e)
        {
            Logging.WriteClientLog("Zug genommen!");
        }

        private void TelemetryRefuel(object sender, EventArgs e)
        {
            Logging.WriteClientLog("TelemetryRefuel-EVENT");
        }


        private void TelemetryRefuelEnd(object sender, EventArgs e)
        {
            Dictionary<string, string> post_param = new Dictionary<string, string>();
            post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
            post_param.Add("LITER", Truck_Daten.LITER_GETANKT.ToString());
            post_param.Add("POS_X", Truck_Daten.POS_X.ToString());
            post_param.Add("POS_Y", Truck_Daten.POS_Y.ToString());
            post_param.Add("POS_Z", Truck_Daten.POS_Z.ToString());
            string response = API.HTTPSRequestPost(API.tanken, post_param);
            Logging.WriteClientLog("TelemetryRefuelEnd-EVENT: " + response);
        }

        private void TelemetryRefuelPayed(object sender, EventArgs e)
        {
            Logging.WriteClientLog("TelemetryRefuelPayed-EVENT: ");
        }

        private void TelemetryRefuelStart(object sender, EventArgs e)
        {
            Logging.WriteClientLog("TelemetryRefuelStart-EVENT");
            BottomFlyOut.IsOpen = !BottomFlyOut.IsOpen;
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
                    Truck_Daten.EURO_DOLLAR = Truck_Daten.SPIEL == "Ets2" ? "€" : "$";
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
                    Truck_Daten.GEWICHT = (data.JobValues.CargoValues.Mass / 1000).ToString();
                    Truck_Daten.GESAMT_KM = (float)data.JobValues.PlannedDistanceKm;
                    Truck_Daten.REST_KM = (float)data.NavigationValues.NavigationDistance;
                    Truck_Daten.EINKOMMEN = (int)data.JobValues.Income;
                    Truck_Daten.FRACHTMARKT = data.JobValues.Market.ToString();
                    Truck_Daten.CARGO_LOADED = data.JobValues.CargoLoaded;
                    Truck_Daten.GEF_STRECKE = (int)data.GamePlay.JobDelivered.DistanceKm;
                    // LKW DATEN
                    Truck_Daten.LKW_MODELL = data.TruckValues.ConstantsValues.Name;
                    Truck_Daten.LKW_HERSTELLER = data.TruckValues.ConstantsValues.Brand;
                    Truck_Daten.LKW_HERSTELLER_ID = data.TruckValues.ConstantsValues.BrandId;
                    Truck_Daten.SPEED = (int)data.TruckValues.CurrentValues.DashboardValues.Speed.Kph;
                    Truck_Daten.SPIEL = data.Game.ToString();
                    Truck_Daten.KMH_MI = Truck_Daten.SPIEL == "Ets2" ? "KM/H" : "MI/H";
                    Truck_Daten.ELEKTRIK_AN = data.TruckValues.CurrentValues.ElectricEnabled;
                    Truck_Daten.MOTOR_AN = data.TruckValues.CurrentValues.EngineEnabled;
                    Truck_Daten.PARKING_BRAKE = data.TruckValues.CurrentValues.MotorValues.BrakeValues.ParkingBrake;
                    Truck_Daten.BLINKER_LINKS = data.TruckValues.CurrentValues.LightsValues.BlinkerLeftOn;
                    Truck_Daten.BLINKER_RECHTS = data.TruckValues.CurrentValues.LightsValues.BlinkerRightOn;
                    Truck_Daten.LICHT_LOW = data.TruckValues.CurrentValues.LightsValues.BeamLow;
                    Truck_Daten.LICHT_HIGH = data.TruckValues.CurrentValues.LightsValues.BeamHigh;
                    Truck_Daten.BREMSLICHT = data.TruckValues.CurrentValues.LightsValues.Brake;
                    Truck_Daten.TRAILER_ANGEHANGEN = data.TrailerValues[0].Attached;
                    Truck_Daten.TEMPOLIMIT = Truck_Daten.SPIEL == "Ets2" ? (int)data.NavigationValues.SpeedLimit.Kph : (int)data.NavigationValues.SpeedLimit.Mph;
                    // FRACHTSCHADEN
                    Truck_Daten.FRACHTSCHADEN = Math.Round(data.TrailerValues[0].DamageValues.Cargo * 100);
                    // STRAFE
                    Truck_Daten.STRAF_BETRAG = (int)data.GamePlay.FinedEvent.Amount;
                    Truck_Daten.GRUND = data.GamePlay.FinedEvent.Offence.ToString();

                    // MAUTSTATION
                    Truck_Daten.MAUT_BETRAG = (int)data.GamePlay.TollgateEvent.PayAmount;

                    // TANKEN
                    Truck_Daten.LITER_GETANKT = data.GamePlay.RefuelEvent.Amount;

                    Truck_Daten.FAHRINFO_1 = "Du fährst mit " + Truck_Daten.GEWICHT + " Tonnen " + Truck_Daten.LADUNG_NAME + " von " + Truck_Daten.STARTORT + " nach " + Truck_Daten.ZIELORT;
                    Truck_Daten.FAHRINFO_2 = "Du musst noch " + (int)Truck_Daten.REST_KM / 1000 + " KM von insgesamt " + Truck_Daten.GESAMT_KM + " KM fahren";

                    // POSITION
                    Truck_Daten.POS_X = data.TruckValues.Positioning.Cabin.X;
                    Truck_Daten.POS_Y = data.TruckValues.Positioning.Cabin.Y;
                    Truck_Daten.POS_Z = data.TruckValues.Positioning.Cabin.Z;

                    // CANCEL TOUR
                    Truck_Daten.CANCEL_STRAFE = data.GamePlay.JobCancelled.Penalty;

                    // Sonstiges
                    Truck_Daten.LKW_SCHADEN = Convert.ToInt32(data.TruckValues.CurrentValues.DamageValues.WheelsAvg * 100);
                    Truck_Daten.TRAILER_SCHADEN = Convert.ToInt32(data.TrailerValues[0].DamageValues.Wheels * 100);



                    // DELIVERED
                    Truck_Daten.FRACHTSCHADEN_ABGABE = data.GamePlay.JobDelivered.CargoDamage;
                    Truck_Daten.AUTOPARKING = data.GamePlay.JobDelivered.AutoParked;
                    Truck_Daten.AUTOLOADING = data.GamePlay.JobDelivered.AutoLoaded;

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

            Systemsounds.SelectedValue = REG.Lesen("Config", "Systemsounds");
            Logging.WriteClientLog("Voreinstellungen geladen !");
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


        private void Preis_eintragen_Click(object sender, RoutedEventArgs e)
        {
            Logging.WriteClientLog("Preis eintragen CLICK-EVENT");
        }


        private void Systemsounds_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if ((string)Systemsounds.SelectedValue == "An")
            {
                REG.Schreiben("Config", "Systemsounds", "An");
            }
            if ((string)Systemsounds.SelectedValue == "Aus")
            {
                REG.Schreiben("Config", "Systemsounds", "Aus");
            }
        }


        public static string GenerateString()
        {
            int length = 70;
            string allowedChars = "abcdefghijklmnopqrstuvwxyz123456789";

            byte[] bytes = new Byte[length];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();

            rng.GetBytes(bytes);

            if (null != allowedChars && String.Empty != allowedChars)
            {
                char[] chars = new char[length];
                int len = allowedChars.Length;

                for (int i = 0; i < length; i++)
                    chars[i] = allowedChars[(int)bytes[i] % len];

                return new string(chars);
            }

            return System.Text.Encoding.Default.GetString(bytes);
        }


        private void LOG_ORDNER_OEFFNEN(object sender, RoutedEventArgs e)
        {
            Process.Start(Config.LogRoot);
        }
    }


}

