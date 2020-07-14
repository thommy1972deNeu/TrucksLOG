using ControlzEx.Theming;
using Janus_Client_V1.Klassen;
using Janus_Client_V1.Spieldaten;
using SCSSdkClient;
using SCSSdkClient.Object;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using AutoUpdaterDotNET;


namespace Janus_Client_V1
{
    public partial class MainWindow
    {
        public string CLIENT_VERSION = "1.3.1.0";
        private readonly MSG msg = new MSG();
        public Truck_Daten Truck_Daten = new Truck_Daten();
        public SCSSdkTelemetry Telemetry;
        public int refueling;
        public string tour_id_tanken;

        private DispatcherTimer job_update_timer = new DispatcherTimer();

        public bool InvokeRequired { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            Logging.Make_Log_File();

            Lade_Voreinstellungen();

            credit_text.Content = "Ein Dank geht an meine Tester:" + Environment.NewLine;
            credit_text.Content += " - Quasselboy Patti [COO]" + Environment.NewLine;
            credit_text.Content += " - Daniel1983 [Beta-Tester]" + Environment.NewLine;
            credit_text.Content += " - TOBI_𝟙Ƽ⊘५ [Beta-Tester]" + Environment.NewLine;
            credit_text.Content += " - Angelo Riechmann [WebDesigner]" + Environment.NewLine;
            credit_text.Content += "Einen Extra Dank an Quasselboy / Patti der mich" + Environment.NewLine + "seit Anbeginn der Zeit unterstützt." + Environment.NewLine;
            credit_text.Content += "und natürlich auch an" + Environment.NewLine;
            credit_text.Content += "unsere(n) Live-Streamer:" + Environment.NewLine;

            Logging.WriteClientLog("Version: " + CLIENT_VERSION);

            job_update_timer.Interval = TimeSpan.FromSeconds(5);

            if (string.IsNullOrEmpty(REG.Lesen("Config", "CLIENT_KEY")))
            {
                CLIENT_KEY_ABFRAGE form = new CLIENT_KEY_ABFRAGE();
                form.ShowDialog();
                return;
            }
            else
            {
                if (string.IsNullOrEmpty(REG.Lesen("Pfade", "ETS2_PFAD")))
                {
                    Pfad_Angeben pf = new Pfad_Angeben();
                    pf.ShowDialog();
                    return;
                }

                try
                {
                    TelemetryInstaller.check_ETS();
                    TelemetryInstaller.check_ATS();
                }
                catch { }

                System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();

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

                /* DEAKTIVIERT FÜR PATTI
                if (REG.Lesen("Config", "Systemsounds") == "An")
                    SoundPlayer.Sound_Willkommen();
                */
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

            if (REG.Lesen("Config", "TOUR_ID") == "")
            {
                try
                {
                    REG.Schreiben("Config", "TOUR_ID", GenerateString());
                }
                catch (Exception ex)
                {
                    Logging.WriteClientLog("Fehler beim Schreiben TOUR_ID mit GENERATE STRING(): " + ex.Message);
                }
            }

            try
            {
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

                SoundPlayer.Sound_Tour_Gestartet();

                job_update_timer.Tick += timer_Tick;
                job_update_timer.Start();
                Logging.WriteClientLog("Tour gestartet: " + response);
            }
            catch (Exception ex)
            {
                Logging.WriteClientLog("Fehler beim Schreiben TOUR_ID mit GENERATE STRING(): " + ex.Message);
            }
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
            SoundPlayer.Sound_Tour_Abgebrochen();
            job_update_timer.Stop();

            Logging.WriteClientLog("Tour abgebrochen: " + response);
        }

        private void TelemetryJobDelivered(object sender, EventArgs e)
        {
            Dictionary<string, string> post_param = new Dictionary<string, string>();
            post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
            post_param.Add("TOUR_ID", REG.Lesen("Config", "TOUR_ID"));
            post_param.Add("FRACHTSCHADEN", Truck_Daten.FRACHTSCHADEN.ToString());
            post_param.Add("STRECKE", Truck_Daten.ABGABE_GEF_STRECKE.ToString());

            string response = API.HTTPSRequestPost(API.job_finish, post_param);
            Console.WriteLine(response);

            REG.Schreiben("Config", "TOUR_ID", "");
            SoundPlayer.Sound_Tour_Beendet();

            job_update_timer.Stop();
            Logging.WriteClientLog("Tour Abgeliefert: " + response);
        }

        private void TelemetryFined(object sender, EventArgs e)
        {
            Dictionary<string, string> post_param = new Dictionary<string, string>();
            post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
            post_param.Add("TOUR_ID", REG.Lesen("Config", "TOUR_ID"));
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
            Dictionary<string, string> post_param = new Dictionary<string, string>();
            post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
            post_param.Add("SOURCE_NAME", Truck_Daten.FERRY_SOURCE_NAME);
            post_param.Add("TARGET_NAME", Truck_Daten.FERRY_TARGET_NAME);
            post_param.Add("PAY_AMOUNT", Truck_Daten.FERRY_PAY_AMOUNT.ToString());

            string response = API.HTTPSRequestPost(API.transport, post_param);
            Logging.WriteClientLog("[INFO] TRANSPORT - FÄHRE - EVENT: " + response);
        }

        private void TelemetryTrain(object sender, EventArgs e)
        {
            Dictionary<string, string> post_param = new Dictionary<string, string>();
            post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
            post_param.Add("SOURCE_NAME", Truck_Daten.TRAIN_SOURCE_NAME.ToString());
            post_param.Add("TARGET_NAME", Truck_Daten.TRAIN_TARGET_NAME.ToString());
            post_param.Add("PAY_AMOUNT", Truck_Daten.TRAIN_PAY_AMOUNT.ToString());

            string response = API.HTTPSRequestPost(API.transport, post_param);
            Logging.WriteClientLog("[INFO] TRANSPORT - TRAIN - EVENT: " + response);
        }

        private void TelemetryRefuel(object sender, EventArgs e)
        {
            Logging.WriteClientLog("Refuel-Event - Liter: " + Truck_Daten.LITER_GETANKT.ToString());
        }

        private void TelemetryRefuelEnd(object sender, EventArgs e)
        {
            Logging.WriteClientLog("Refuel-END Event - Liter: " + Truck_Daten.LITER_GETANKT.ToString());
        }

        private void TelemetryRefuelPayed(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(REG.Lesen("Config", "TOUR_ID")))
            {
                tour_id_tanken = "0";
            }
            else
            {
                tour_id_tanken = REG.Lesen("Config", "TOUR_ID");
            }

            Dictionary<string, string> post_param = new Dictionary<string, string>();
            post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
            post_param.Add("TOUR_ID", tour_id_tanken);
            post_param.Add("LITER", Truck_Daten.LITER_GETANKT.ToString());
            post_param.Add("POS_X", Truck_Daten.POS_X.ToString());
            post_param.Add("POS_Y", Truck_Daten.POS_Y.ToString());
            post_param.Add("POS_Z", Truck_Daten.POS_Z.ToString());

            string response = API.HTTPSRequestPost(API.tanken, post_param);
            Logging.WriteClientLog("[INFO] Telemetry Refuel Payed - EVENT: " + response);
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
                    Truck_Daten.SDK_AKTIVE = !data.SdkActive;

                    // PFADE für Seitenmenü
                    Truck_Daten.ETS_PFAD = REG.Lesen("Pfade", "ETS2_PFAD");
                    Truck_Daten.ATS_PFAD = REG.Lesen("Pfade", "ATS_PFAD");
                    Truck_Daten.TMP_PFAD = REG.Lesen("Pfade", "TMP_PFAD");

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

                    // Gewicht
                    Truck_Daten.GEWICHT = (data.JobValues.CargoValues.Mass / 1000).ToString();
                    Truck_Daten.GEWICHT2 = (int)(data.JobValues.CargoValues.Mass / 1000);

                    Truck_Daten.GESAMT_KM = (float)data.JobValues.PlannedDistanceKm;
                    Truck_Daten.REST_KM = (float)data.NavigationValues.NavigationDistance;

                    Truck_Daten.REST_KM_SA = (int)data.NavigationValues.NavigationDistance / 1000;
                    Truck_Daten.GESAMT_KM_SA = (int)data.JobValues.PlannedDistanceKm;

                    Truck_Daten.EINKOMMEN = (int)data.JobValues.Income;
                    Truck_Daten.FRACHTMARKT = data.JobValues.Market.ToString();
                    Truck_Daten.CARGO_LOADED = data.JobValues.CargoLoaded;
                    Truck_Daten.GEF_STRECKE = (int)data.GamePlay.JobDelivered.DistanceKm;

                    // LKW DATEN
                    Truck_Daten.LKW_MODELL = data.TruckValues.ConstantsValues.Name;
                    Truck_Daten.LKW_HERSTELLER = data.TruckValues.ConstantsValues.Brand;
                    Truck_Daten.LKW_HERSTELLER_ID = data.TruckValues.ConstantsValues.BrandId;

                    //Truck_Daten.SPEED =  (int)data.TruckValues.CurrentValues.DashboardValues.Speed.Kph;
                    Truck_Daten.SPEED = Truck_Daten.SPIEL == "Ets2" ? (int)data.TruckValues.CurrentValues.DashboardValues.Speed.Kph : (int)data.TruckValues.CurrentValues.DashboardValues.Speed.Mph;

                    Truck_Daten.SPIEL = data.Game.ToString();
                    Truck_Daten.KMH_MI = Truck_Daten.SPIEL == "Ets2" ? "KM/H" : "MI/H";
                    Truck_Daten.ELEKTRIK_AN = data.TruckValues.CurrentValues.ElectricEnabled;
                    Truck_Daten.MOTOR_AN = data.TruckValues.CurrentValues.EngineEnabled;
                    Truck_Daten.PARKING_BRAKE = data.TruckValues.CurrentValues.MotorValues.BrakeValues.ParkingBrake;
                    Truck_Daten.BLINKER_LINKS = data.TruckValues.CurrentValues.LightsValues.BlinkerLeftOn;
                    Truck_Daten.BLINKER_RECHTS = data.TruckValues.CurrentValues.LightsValues.BlinkerRightOn;
                    Truck_Daten.GEAR = data.TruckValues.CurrentValues.MotorValues.GearValues.Selected;

                    switch (Truck_Daten.GEAR)
                    {
                        case (0):
                            Truck_Daten.GANG = "N";
                            break;

                        case (-1):
                            Truck_Daten.GANG = "R1";
                            break;

                        case (-2):
                            Truck_Daten.GANG = "R2";
                            break;

                        case (-3):
                            Truck_Daten.GANG = "R3";
                            break;

                        case (-4):
                            Truck_Daten.GANG = "R4";
                            break;

                        case (1):
                            Truck_Daten.GANG = "1";
                            break;

                        case (2):
                            Truck_Daten.GANG = "2";
                            break;

                        case (3):
                            Truck_Daten.GANG = "3";
                            break;

                        case (4):
                            Truck_Daten.GANG = "4";
                            break;

                        case (5):
                            Truck_Daten.GANG = "5";
                            break;

                        case (6):
                            Truck_Daten.GANG = "6";
                            break;

                        case (7):
                            Truck_Daten.GANG = "7";
                            break;

                        case (8):
                            Truck_Daten.GANG = "8";
                            break;

                        case (9):
                            Truck_Daten.GANG = "9";
                            break;

                        case (10):
                            Truck_Daten.GANG = "10";
                            break;

                        case (11):
                            Truck_Daten.GANG = "11";
                            break;

                        case (12):
                            Truck_Daten.GANG = "12";
                            break;

                        case (13):
                            Truck_Daten.GANG = "13";
                            break;

                        case (14):
                            Truck_Daten.GANG = "14";
                            break;

                        case (15):
                            Truck_Daten.GANG = "15";
                            break;

                        case (16):
                            Truck_Daten.GANG = "16";
                            break;

                        case (17):
                            Truck_Daten.GANG = "17";
                            break;

                        case (18):
                            Truck_Daten.GANG = "18";
                            break;
                    }

                    Truck_Daten.STANDLICHT = data.TruckValues.CurrentValues.LightsValues.Parking;
                    Truck_Daten.LICHT_LOW = data.TruckValues.CurrentValues.LightsValues.BeamLow;
                    Truck_Daten.FERNLICHT = data.TruckValues.CurrentValues.LightsValues.BeamHigh;

                    Truck_Daten.BREMSLICHT = data.TruckValues.CurrentValues.LightsValues.Brake;
                    Truck_Daten.TRAILER_ANGEHANGEN = data.TrailerValues[0].Attached;
                    Truck_Daten.TEMPOLIMIT = (int)(Truck_Daten.SPIEL == "Ets2" ? Math.Round(data.NavigationValues.SpeedLimit.Kph) : Math.Round(data.NavigationValues.SpeedLimit.Mph));
                    Truck_Daten.AIR_WARNUNG = data.TruckValues.CurrentValues.DashboardValues.WarningValues.AirPressure;

                    // FRACHTSCHADEN
                    Truck_Daten.FRACHTSCHADEN = data.JobValues.CargoValues.CargoDamage * 100;
                    Truck_Daten.FRACHTSCHADEN2 = Math.Round(Truck_Daten.FRACHTSCHADEN, 1);

                    // STRAFE
                    Truck_Daten.STRAF_BETRAG = (int)data.GamePlay.FinedEvent.Amount;
                    Truck_Daten.GRUND = data.GamePlay.FinedEvent.Offence.ToString();

                    // MAUTSTATION
                    Truck_Daten.MAUT_BETRAG = (int)data.GamePlay.TollgateEvent.PayAmount;

                    // Fahrtinfo
                    Truck_Daten.FAHRINFO_1 = "Du fährst mit " + Truck_Daten.GEWICHT2 + " Tonnen " + Truck_Daten.LADUNG_NAME + " von " + Truck_Daten.STARTORT + " nach " + Truck_Daten.ZIELORT;

                    // POSITION
                    Truck_Daten.POS_X = data.TruckValues.Positioning.Cabin.X;
                    Truck_Daten.POS_Y = data.TruckValues.Positioning.Cabin.Y;
                    Truck_Daten.POS_Z = data.TruckValues.Positioning.Cabin.Z;

                    // CANCEL TOUR
                    Truck_Daten.CANCEL_STRAFE = data.GamePlay.JobCancelled.Penalty;

                    // Sonstiges

                    // SCAHDENSBERECHNUNG LKW
                    float schaden1 = data.TruckValues.CurrentValues.DamageValues.Engine * 100;
                    float schaden2 = data.TruckValues.CurrentValues.DamageValues.Transmission * 100;
                    float schaden3 = data.TruckValues.CurrentValues.DamageValues.Chassis * 100;
                    float schaden4 = data.TruckValues.CurrentValues.DamageValues.Cabin * 100;
                    float schaden5 = data.TruckValues.CurrentValues.DamageValues.WheelsAvg * 100;
                    Truck_Daten.LKW_SCHADEN = Math.Round((schaden1 + schaden3 + schaden4) / 3);

                    // SCHADENSBERECHNUNG TRAILER
                    //float tschaden1 = data.TrailerValues[0].DamageValues.Wheels*100;
                    float tschaden2 = data.TrailerValues[0].DamageValues.Chassis * 100;
                    //float tschaden3 = data.TrailerValues[0].DamageValues.Cargo * 100;

                    Truck_Daten.TRAILER_SCHADEN = Math.Round(tschaden2);

                    // DELIVERED
                    Truck_Daten.FRACHTSCHADEN_ABGABE = data.GamePlay.JobDelivered.CargoDamage;
                    Truck_Daten.ABGABE_GEF_STRECKE = data.GamePlay.JobDelivered.DistanceKm;
                    Truck_Daten.AUTOPARKING = data.GamePlay.JobDelivered.AutoParked;
                    Truck_Daten.AUTOLOADING = data.GamePlay.JobDelivered.AutoLoaded;

                    // TANKEN
                    Truck_Daten.LITER_GETANKT = data.GamePlay.RefuelEvent.Amount;
                    Truck_Daten.FUEL_MAX = (int)data.TruckValues.ConstantsValues.CapacityValues.Fuel;
                    Truck_Daten.FUEL_GERADE = (int)data.TruckValues.CurrentValues.DashboardValues.FuelValue.Amount;
                    Truck_Daten.LITER_GALLONEN = (Truck_Daten.SPIEL == "Ets2") ? " L" : " Gal.";

                    // Transport FERRY
                    Truck_Daten.FERRY_SOURCE_NAME = data.GamePlay.FerryEvent.SourceName;
                    Truck_Daten.FERRY_TARGET_NAME = data.GamePlay.FerryEvent.TargetName;
                    Truck_Daten.FERRY_PAY_AMOUNT = (int)data.GamePlay.FerryEvent.PayAmount;

                    // Transport TRAIN
                    Truck_Daten.TRAIN_SOURCE_NAME = data.GamePlay.TrainEvent.SourceName;
                    Truck_Daten.TRAIN_TARGET_NAME = data.GamePlay.TrainEvent.TargetName;
                    Truck_Daten.TRAIN_PAY_AMOUNT = (int)data.GamePlay.TrainEvent.PayAmount;
                }
            }
            catch
            { }
        }

        private void Lade_Voreinstellungen()
        {
            try
            {
                Farbschema.SelectedValue = REG.Lesen("Config", "Farbschema");

                if (string.IsNullOrWhiteSpace(REG.Lesen("Config", "BG_OPACITY")))
                    REG.Schreiben("Config", "BG_OPACITY", "1");

                if (string.IsNullOrWhiteSpace(REG.Lesen("Config", "TOUR_ID")))
                    REG.Schreiben("Config", "TOUR_ID", "");

                if (string.IsNullOrWhiteSpace(REG.Lesen("Config", "CLIENT_KEY")))
                    REG.Schreiben("Config", "CLIENT_KEY", "");

                Systemsounds.SelectedValue = REG.Lesen("Config", "Systemsounds");

                if (string.IsNullOrWhiteSpace(REG.Lesen("Config", "Background")))
                {
                    Background_WEchsler.SelectedValue = "pj_1.png";
                    REG.Schreiben("Config", "Background", "pj_1.png");
                }
                else
                {
                    Background_WEchsler.SelectedValue = REG.Lesen("Config", "Background");
                }

                // Hintergrund Transparenz Beginn
                bg_opacity.Value = Convert.ToDouble(REG.Lesen("Config", "BG_OPACITY"));
                this.Hauptfenster.Background.Opacity = Convert.ToDouble(REG.Lesen("Config", "BG_OPACITY"));
                // Hintergrund Transparenz Ende

                // Pfade Auslesen Beginn
                try
                {
                    ets2_content.Content = REG.Lesen("Pfade", "ETS2_PFAD") != "" ? "OK" : "Fehlt";
                    ats_content.Content = REG.Lesen("Pfade", "ATS_PFAD") != "" ? "OK" : "Fehlt";
                    tmp_content.Content = REG.Lesen("Pfade", "TMP_PFAD") != "" ? "OK" : "Fehlt";
                    status_bar_version.Content = "Client Version: " + CLIENT_VERSION;
                    Logging.WriteClientLog("Pfade aus REG gelesen und in Seitenmenü angezeigt !");
                }
                catch (Exception ex)
                {
                    Logging.WriteClientLog("Fehler beim laden der Pfade aus Registry" + ex.Message);
                }
                // Pfade Auslesen Ende

                try
                {
                    this.DataContext = Truck_Daten;
                }
                catch (Exception ex)
                {
                    Logging.WriteClientLog("Fehler beim setzen des DataContext" + ex.Message);
                }

                Logging.WriteClientLog("Voreinstellungen geladen !");
            }
            catch (Exception ex)
            {
                Logging.WriteClientLog("Fehler beim Laden der Voreinstellungen " + ex.Message);
            }

            /*
                ZEIGE_TMP.IsEnabled = (string.IsNullOrEmpty(REG.Lesen("Pfade", "TMP_PFAD"))) ? false : true;
                ZEIGE_ETS2.IsEnabled = (string.IsNullOrEmpty(REG.Lesen("Pfade", "ETS2_PFAD"))) ? false : true;
                ZEIGE_ATS.IsEnabled = (string.IsNullOrEmpty(REG.Lesen("Pfade", "ATS_PFAD"))) ? false : true;
            */
        }

        private void Beta_Tester(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("https://projekt-janus.de/beta_bewerbung.php");
            }
            catch (Exception ex)
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
                Process.Start("https://paypal.me/ErIstWiederDa/2,00");
            }
            catch (Exception ex)
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

        private void spende_paypal(object sender, RoutedEventArgs e)
        {
            Process.Start("https://paypal.me/ErIstWiederDa/2,00");
        }

        private void patreon_link(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.patreon.com/projektjanus");
        }

        private void Background_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            REG.Schreiben("Config", "Background", (string)(Background_WEchsler.SelectedValue));
            try
            {
                ImageBrush myBrush = new ImageBrush();
                myBrush.ImageSource = new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "Images/" + (string)Background_WEchsler.SelectedValue));
                this.Hauptfenster.Background = myBrush;
            }
            catch (Exception ex)
            {
                Logging.WriteClientLog("Designer: Konnte den Background " + (string)(Background_WEchsler.SelectedValue) + " nicht laden." + ex.Message + ex.StackTrace);
            }
        }

        private void Oeffne_Credits(object sender, RoutedEventArgs e)
        {
        }

        private void Andras_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> post_param = new Dictionary<string, string>();
            post_param.Add("LINK", "Andras.tv");
            post_param.Add("USER", REG.Lesen("Config", "CLIENT_KEY"));
            string response = API.HTTPSRequestPost(API.link_click, post_param);

            Process.Start("https://www.twitch.tv/andras_tv");
        }

        private void bg_opacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                this.Hauptfenster.Background.Opacity = bg_opacity.Value;

                REG.Schreiben("Config", "BG_OPACITY", bg_opacity.Value.ToString());
                Truck_Daten.BG_OPACITY = bg_opacity.Value;
            }
            catch { }
        }

        private void OPEN_BG_FILE(object sender, RoutedEventArgs e)
        {
            var bild = new Microsoft.Win32.OpenFileDialog();
            bild.Filter = "Alle Bilder|*.jpg;*.png;|Alle Dateien|*.*";
            bild.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures);
            var result_bild = bild.ShowDialog();
            if (result_bild == false) return;

            REG.Schreiben("Config", "Background", bild.FileName);

            ImageBrush myBrush = new ImageBrush();
            myBrush.ImageSource = new BitmapImage(new Uri(bild.FileName));
            this.Hauptfenster.Background = myBrush;
        }

        private void Minimize_Window(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void settings_Click(object sender, RoutedEventArgs e)
        {
            Pfad_Angeben pf2 = new Pfad_Angeben();
            pf2.ShowDialog();
        }

        private void tmp_starten_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(REG.Lesen("Pfade", "TMP_PFAD"));
        }

        private void ats_starten_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(REG.Lesen("Pfade", "ATS_PFAD"));
        }

        private void ets_starten_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(REG.Lesen("Pfade", "ETS2_PFAD"));
        }

        private void spielpfade_aendern(object sender, RoutedEventArgs e)
        {
            Pfad_Angeben pf3 = new Pfad_Angeben();
            pf3.ShowDialog();
        }

        private void ContextMenu_Click(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as System.Windows.Controls.MenuItem;
            MessageBox.Show($"{item.Header} was clicked");
        }

        private void Hauptfenster_Loaded(object sender, RoutedEventArgs e)
        {
            AutoUpdater.Start("http://clientupdates.projekt-janus.de/version.xml");
        }
    }
}