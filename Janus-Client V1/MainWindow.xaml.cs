using ControlzEx.Theming;
using TrucksLOG.Klassen;
using TrucksLOG.Spieldaten;
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
using System.Reflection;
using System.Linq;
using System.Windows.Input;
using WindowsInput.Native;
using WindowsInput;
using DiscordRPC;
using Microsoft.Win32;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Net;

namespace TrucksLOG
{
    public partial class MainWindow
    {
       
        public DiscordRpcClient client;
        private static RichPresence jobRPC;
        private const string DiscordAppID = "730374187025170472";
        private const string DefaultDiscordLargeImageKey = "pj_512";
        private string UpdateString = "http://client.truckslog.org/version.xml";

        public string CLIENT_VERSION = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        private readonly MSG msg = new MSG();
        public Truck_Daten Truck_Daten = new Truck_Daten();
        public SCSSdkTelemetry Telemetry;
        public int refueling;
        public string tour_id_tanken;
        InputSimulator sim = new InputSimulator();

        private DispatcherTimer job_update_timer = new DispatcherTimer();
        private DispatcherTimer anti_afk_timer = new DispatcherTimer();
        private DispatcherTimer useronline_timer = new DispatcherTimer();
        private DispatcherTimer zu_schnell = new DispatcherTimer();

        public bool InvokeRequired { get; private set; }
        public static Window ActivatedWindow { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            OnlineCheck();

            if (ServerCheck("https://truckslog.org") == false)
            {
                MessageBox.Show("Der Server ist leider Offline\n Das Programm wird jetzt beendet!", "Fehler bei Verbindung zum Server", MessageBoxButton.OK, MessageBoxImage.Error);
               /// Logging.WriteClientLog("[ERROR] - Verbindung zum Server unterbrochen -> Programm exit();");
                Application.Current.Shutdown();
            }

            //Logging.Make_Log_File();

            Lade_Voreinstellungen();
            lade_GameVersionen();

            // DISCORD
            client = new DiscordRpcClient(DiscordAppID);
            client.Initialize();
            var timer = new System.Timers.Timer(150);
            timer.Elapsed += (sender, args) => { client.Invoke(); };
            timer.Start();
            // DISCORD ENDE

            credit_text.Content = "Ein Dank geht an meine Tester:" + Environment.NewLine;
            credit_text.Content += " - Quasselboy Patti [COO]" + Environment.NewLine;
            credit_text.Content += " - Daniel1983 [Main-Support][Beta-Tester]" + Environment.NewLine;
            credit_text.Content += " - TOBI_𝟙Ƽ⊘५ [Main-Support][Beta-Tester]" + Environment.NewLine;
            credit_text.Content += " - Bandit|Basti [Beta-Tester]" + Environment.NewLine;
            credit_text.Content += "Einen Super-Dank an Quasselboy / Patti der mich" + Environment.NewLine + "seit Anbeginn der PJ-Zeit unterstützt." + Environment.NewLine;
            credit_text.Content += "Und natürlich auch an" + Environment.NewLine;
            credit_text.Content += "unseren One & Only-Live-Streamer:" + Environment.NewLine;

            useronline_timer.Interval = TimeSpan.FromSeconds(5);
            useronline_timer.Tick += Useronline_Tick;
            useronline_timer.Start();

            zu_schnell.Interval = TimeSpan.FromSeconds(1);
            zu_schnell.Tick += zu_schnell_tick;

        
            // JOB UPDATE TIMER
            job_update_timer.Interval = TimeSpan.FromSeconds(5);

            // ANTI_AFK TIMER
            anti_afk_timer.Interval = TimeSpan.FromMinutes(Convert.ToInt32(REG.Lesen("Config", "ANTI_AFK_TIMER")));

            if (string.IsNullOrEmpty(REG.Lesen("Config", "CLIENT_KEY")))
            {
                Pfad_Angeben pf = new Pfad_Angeben();
                pf.ShowDialog();
                return;
            } else
            {
                Bann_Check();
            }

            if (REG.Lesen("Pfade", "ETS2_PFAD") == "" && REG.Lesen("Pfade", "ATS_PFAD") == "")
            {
                Pfad_Angeben pf = new Pfad_Angeben();
                pf.ShowDialog();
                return;
            }
            try {
                TelemetryInstaller.check_ETS();
                TelemetryInstaller.check_ATS();
            } catch { }

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

            if (MainWindow.AlreadyRunning())
            {
                Application.Current.Shutdown();
                return;
            }

           
        }

        public static async void Bann_Check()
        {
            Dictionary<string, string> post_param = new Dictionary<string, string>();
            post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
            string response = API.HTTPSRequestPost(API.bann_check, post_param);
            string[] ausgabe = response.Split(':');

            
                if (Convert.ToInt32(ausgabe[0]) == 0)
                {
                    var metroWindow = (Application.Current.MainWindow as MetroWindow);
                    await metroWindow.ShowMessageAsync("Account Freischaltung", "Dein Account wurde noch nicht Freigeschaltet.\n\nBitte wende dich an unseren Discord-Support");
                    Application.Current.Shutdown();
                }
                if (Convert.ToInt32(ausgabe[0]) == 3)
                {
                    var metroWindow = (Application.Current.MainWindow as MetroWindow);
                    await metroWindow.ShowMessageAsync("Account Gesperrt", "Dein Account wurde von =- " + ausgabe[1] + " -= temporär Gesperrt\nBegründung: " + ausgabe[2] + "\n\nFür weitere Fragen wende dich an unseren Discord-Support.\nDas Programm wird jetzt beendet.");
                    Application.Current.Shutdown();
                }
                if (Convert.ToInt32(ausgabe[0]) == 6)
                {
                    var metroWindow = (Application.Current.MainWindow as MetroWindow);
                    await metroWindow.ShowMessageAsync("Account Gebannt", "Dein Account wurde von =- " + ausgabe[1] + " -= permanent Gebannt\n\nBegründung: " + ausgabe[2] + "\n\nFür weitere Fragen wende dich an unseren Discord-Support.\nDas Programm wird jetzt beendet.");
                    Application.Current.Shutdown();
                }
            
        }

        public static bool ServerCheck(string host)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(host);
                request.Method = "HEAD";
                request.Timeout = 5000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception)
            {
                return false;
            }
        }


        private static bool AlreadyRunning()
        {
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);

            foreach (Process process in processes)
            {
                if (process.Id != current.Id)
                {
                    if (Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == current.MainModule.FileName)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool CheckIfAProcessIsRunning(string processname)
        {
            return Process.GetProcessesByName(processname).Length > 0;
        }



        [System.Runtime.InteropServices.DllImport("user32.dll")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hwnd);


        private enum ShowWindowEnum
        {
            Hide = 0,
            ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
            Maximize = 3, ShowNormalNoActivate = 4, Show = 5,
            Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
            Restore = 9, ShowDefault = 10, ForceMinimized = 11
        };
        public void BringMainWindowToFront(string processName)
        {
            Process bProcess = Process.GetProcessesByName(processName).FirstOrDefault();
            if (bProcess != null)
            {
                if (bProcess.MainWindowHandle == IntPtr.Zero)
                {
                    ShowWindow(bProcess.Handle, ShowWindowEnum.Restore);
                }
                SetForegroundWindow(bProcess.MainWindowHandle);
            }
        }

        private void anti_afk_timer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (Truck_Daten.SDK_AKTIVE == false)
                {
                    if (Truck_Daten.SPEED == 0 && Truck_Daten.SDK_AKTIVE == false)
                    {
                        if (Truck_Daten.SPIEL == "Ets2")
                        {
                            BringMainWindowToFront("eurotrucks2");
                        }
                        else
                        {
                            BringMainWindowToFront("amtrucks");
                        }
                        sim.Keyboard.KeyPress(VirtualKeyCode.VK_Y);
                        sim.Keyboard.TextEntry("PJ-BOT:");
                        sim.Keyboard.TextEntry(REG.Lesen("Config", "ANTI_AFK_TEXT"));
                        sim.Keyboard.KeyPress(VirtualKeyCode.RETURN);
                        Logging.WriteClientLog("[INFO] Keys gesendet! Geschwindigkeit: " + Truck_Daten.SPEED);
                    }
                    else
                    {
                        Logging.WriteClientLog("[INFO] Keys nicht gesendet! Kein Spiel gestartet oder Geschwindigkeit: " + Truck_Daten.SPEED);
                    }
                } else
                {
                    Logging.WriteClientLog("[INFO] ANT-AFK ist an, aber kein Spiel gestartet...");
                }
            } catch (Exception ex)
            {
                Logging.WriteClientLog("[ERROR] Fehler bei anti_afk_timer " + ex.Message);
            }

        }

        private void zu_schnell_tick(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, string> post_param = new Dictionary<string, string>();
                post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
                post_param.Add("SPEED", Truck_Daten.SPEED.ToString());
                string response = API.HTTPSRequestPost(API.user_zu_schnell, post_param);
            }
            catch (Exception ex)
            {
                Logging.WriteClientLog("[ERROR] Fehler bei zu_schnell_tick " + ex.Message);
            }

        }



        private void setzt_antiAFK()
        {
            if (Truck_Daten.PATREON_LEVEL == 0)
            {
                REG.Schreiben("Config", "ANTI_AFK_TEXT", "TrucksLOG wünscht allen Truckern eine angenehme und sichere Fahrt!");
                anti_ak_text.Text = "TrucksLOG wünscht allen Truckern eine angenehme und sichere Fahrt!";
                anti_ak_text.MaxLength = 150;
                laenge_antiafk_text.Content = "Max. 150 Zeichen";
            }
            else if (Truck_Daten.PATREON_LEVEL == 1)
            {
                anti_ak_text.MaxLength = 150;
                laenge_antiafk_text.Content = "Max. 50 Zeichen";
            }
            else if (Truck_Daten.PATREON_LEVEL == 2)
            {
                anti_ak_text.MaxLength = 150;
                laenge_antiafk_text.Content = "Max. 100 Zeichen";
            }
            else if (Truck_Daten.PATREON_LEVEL == 3)
            {
                anti_ak_text.MaxLength = 150;
                laenge_antiafk_text.Content = "Max. 150 Zeichen";
            }
        }
        private void Useronline_Tick(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, string> post_param = new Dictionary<string, string>();
                post_param.Add("SECRET", "fZhdgte4fdgDDgfet567ufghf");
                string response_online = API.HTTPSRequestPost(API.useronline_url, post_param);
                Truck_Daten.ONLINEUSER = "Fahrer: " + response_online;
               
                lade_fahrer();
                lade_Punktekonto();
                set_online();

            } catch{
                Logging.WriteClientLog("[ERROR] Fehler beim Useronline-Tick !");
            }
        }


        private void lade_fahrer()
        {
            Dictionary<string, string> post_param2 = new Dictionary<string, string>();
            post_param2.Add("VERSION", CLIENT_VERSION);
            string response = API.HTTPSRequestPost(API.fahreronline_url, post_param2);
            string respons_br = response.Replace("<br/>", "\n");
            fahrer_text.Text = respons_br.ToString();
        }


        private void timer_Tick(object sender, EventArgs e)
            {
            string restkilometer;
            try
            {
                ETS_TOUR_delete.IsEnabled = !string.IsNullOrEmpty(REG.Lesen("Config", "TOUR_ID_ETS2"));
                ATS_TOUR_delete.IsEnabled = !string.IsNullOrEmpty(REG.Lesen("Config", "TOUR_ID_ATS"));

                lade_Patreon();
                setzt_antiAFK();

                Dictionary<string, string> post_param = new Dictionary<string, string>();
                if(Truck_Daten.SPIEL == "Ets2")
                {
                    post_param.Add("TOUR_ID", REG.Lesen("Config", "TOUR_ID_ETS2"));
                } else
                {
                    post_param.Add("TOUR_ID", REG.Lesen("Config", "TOUR_ID_ATS"));
                }
                
                post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
                if(Truck_Daten.SPIEL == "Ets2")
                {
                    post_param.Add("REST_KM", Truck_Daten.REST_KM_SA.ToString());
                    restkilometer = Truck_Daten.REST_KM_SA.ToString();
                } else
                {
                    post_param.Add("REST_KM", Truck_Daten.REST_KM_SA.ToString());
                    restkilometer = Truck_Daten.REST_KM_SA.ToString();
                }
                post_param.Add("FRACHTMARKT", Truck_Daten.FRACHTMARKT.ToString());
                post_param.Add("RESTZEIT", Truck_Daten.RESTZEIT_INT.ToString());
                post_param.Add("FRACHTSCHADEN", Truck_Daten.FRACHTSCHADEN.ToString());
                string response = API.HTTPSRequestPost(API.job_update, post_param);

                Logging.WriteClientLog("[UPDATE] Tour-Update: " + Truck_Daten.FRACHTSCHADEN.ToString() + ", Rest-KM: " + restkilometer);

            } catch (Exception ex)
            {
                Logging.WriteClientLog("[ERROR] Fehler beim Tour-Update " + ex.Message);
            }

     

        }

        private void lade_Patreon()
        {
            try
            {
                Dictionary<string, string> post_param = new Dictionary<string, string>();
                post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
                string response = API.HTTPSRequestPost(API.patreon_state, post_param);
                Truck_Daten.PATREON_LEVEL = Convert.ToInt32(response);

            }
            catch
            {
                Truck_Daten.PATREON_LEVEL = 0;
            }
        }

        private void lade_Punktekonto()
        {
            try
            {
                Dictionary<string, string> post_param = new Dictionary<string, string>();
                post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
                string response = API.HTTPSRequestPost(API.punktekonto, post_param);
                Truck_Daten.PUNKTEKONTO = "Punkte: " + response;
                
            }
            catch
            { 
                Logging.WriteClientLog("[ERROR] Fehler beim laden des Punktekontos !"); 
            }
        }


        private void lade_GameVersionen()
        {
            try
            {
                string unixTimestamp = Convert.ToString((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);

                Dictionary<string, string> post_param = new Dictionary<string, string>();
                post_param.Add("TIMESTAMP", unixTimestamp);
                string response = API.HTTPSRequestPost(API.tmp_versionen, post_param);
                Truck_Daten.TMP_VERSIONEN = response;
                ZEIGE_TMP.Content += "\n" + Truck_Daten.TMP_VERSIONEN;
            }
            catch
            {
                Truck_Daten.TMP_VERSIONEN = "Ladefehler...";
            }
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
                    System.Windows.MessageBox.Show("Could not get required data. Job couldn't start.", "ERROR", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            if (Truck_Daten.SPIEL == "Ets2")
            {

                if (REG.Lesen("Config", "TOUR_ID_ETS2") == "")
                {
                    try
                    {
                        REG.Schreiben("Config", "TOUR_ID_ETS2", GenerateString());
                    }
                    catch (Exception ex)
                    {
                        Logging.WriteClientLog("[ERROR] Fehler beim Schreiben TOUR_ID_ETS2 mit GENERATE STRING(): " + ex.Message);
                    }
                }
            } else
            {
                if (REG.Lesen("Config", "TOUR_ID_ATS") == "")
                {
                    try
                    {
                        REG.Schreiben("Config", "TOUR_ID_ATS", GenerateString());
                    }
                    catch (Exception ex)
                    {
                        Logging.WriteClientLog("[ERROR] Fehler beim Schreiben TOUR_ID_ATS mit GENERATE STRING(): " + ex.Message);
                    }
                }
            }


            try
            {
                Dictionary<string, string> post_param = new Dictionary<string, string>();
                if(Truck_Daten.SPIEL == "Ets2")
                {
                    post_param.Add("TOUR_ID", REG.Lesen("Config", "TOUR_ID_ETS2"));
                } else
                {
                    post_param.Add("TOUR_ID", REG.Lesen("Config", "TOUR_ID_ATS"));
                }
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
                Logging.WriteClientLog("[INFO] Tour gestartet: " + response);
            }
            catch (Exception ex)
            {
                Logging.WriteClientLog("[ERROR] Fehler beim Starten der Tour: " + ex.Message);
            }
        }

        private void TelemetryJobCancelled(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, string> post_param = new Dictionary<string, string>();
                post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));

                if (Truck_Daten.SPIEL == "Ets2")
                {
                    post_param.Add("TOUR_ID", REG.Lesen("Config", "TOUR_ID_ETS2"));
                }
                else
                {
                    post_param.Add("TOUR_ID", REG.Lesen("Config", "TOUR_ID_ATS"));
                }

                post_param.Add("FRACHTMARKT", Truck_Daten.FRACHTMARKT);
                string response = API.HTTPSRequestPost(API.job_cancel, post_param);
                Console.WriteLine(response);

                if(Truck_Daten.SPIEL == "Ets2")
                {
                    REG.Schreiben("Config", "TOUR_ID_ETS2", "");
                } else
                {
                    REG.Schreiben("Config", "TOUR_ID_ATS", "");
                }

                SoundPlayer.Sound_Tour_Abgebrochen();
                job_update_timer.Stop();

                Logging.WriteClientLog("[INFO] Tour abgebrochen: " + response);
            } catch (Exception ex)
            {
                Logging.WriteClientLog("[ERROR] Fehler beim Tour abbrechen !" + ex.Message);
            }

        }



        private void TelemetryJobDelivered(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, string> post_param = new Dictionary<string, string>();
                post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));

                if (Truck_Daten.SPIEL == "Ets2")
                {
                    post_param.Add("TOUR_ID", REG.Lesen("Config", "TOUR_ID_ETS2"));
                }
                else
                {
                    post_param.Add("TOUR_ID", REG.Lesen("Config", "TOUR_ID_ATS"));
                }

                //post_param.Add("FRACHTSCHADEN", Truck_Daten.FRACHTSCHADEN.ToString());
                post_param.Add("STRECKE", Truck_Daten.ABGABE_GEF_STRECKE.ToString());
                post_param.Add("EARNED_XP", Truck_Daten.EARNED_XP.ToString());
                post_param.Add("AUTOPARKING", Truck_Daten.AUTOPARKING.ToString());
                post_param.Add("AUTOLOADING", Truck_Daten.AUTOLOADING.ToString());
                string response = API.HTTPSRequestPost(API.job_finish, post_param);

                    if (Truck_Daten.SPIEL == "Ets2") {
                        REG.Schreiben("Config", "TOUR_ID_ETS2", "");
                        } else
                        {
                            REG.Schreiben("Config", "TOUR_ID_ATS", "");
                    }

                SoundPlayer.Sound_Tour_Beendet();

                job_update_timer.Stop();
                Logging.WriteClientLog("[INFO] Tour Abgeliefert: " + response);
            } catch(Exception ex)
            {
                Logging.WriteClientLog("[ERROR] Fehler beim Abgeben der Tour ! - " + ex.Message);
            }

        }

        private void TelemetryFined(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, string> post_param = new Dictionary<string, string>();
                post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));

                if (Truck_Daten.SPIEL == "Ets2")
                {
                    post_param.Add("TOUR_ID", REG.Lesen("Config", "TOUR_ID_ETS2"));
                }
                else
                {
                    post_param.Add("TOUR_ID", REG.Lesen("Config", "TOUR_ID_ATS"));
                }

                post_param.Add("BETRAG", Truck_Daten.STRAF_BETRAG.ToString());
                post_param.Add("GRUND", Truck_Daten.GRUND);
                string response = API.HTTPSRequestPost(API.strafe, post_param);
                if (REG.Lesen("Config", "Systemsounds") == "An") SoundPlayer.Sound_Strafe_Erhalten();
                Logging.WriteClientLog("[INFO] Strafe erhalten: " + response);
            } catch (Exception ex)
            {
                Logging.WriteClientLog("[ERROR] Fehler bei Strafzuweisung" + ex.Message);
            }

        }

        private void TelemetryTollgate(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, string> post_param = new Dictionary<string, string>();
                post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
                if(Truck_Daten.SPIEL == "Ets2")
                {
                    post_param.Add("TOUR_ID", REG.Lesen("Config", "TOUR_ID_ETS2"));
                } else {
                    post_param.Add("TOUR_ID", REG.Lesen("Config", "TOUR_ID_ATS"));
                }
                post_param.Add("BETRAG", Truck_Daten.MAUT_BETRAG.ToString());
                string response = API.HTTPSRequestPost(API.tollgate, post_param);
                Console.WriteLine(response);

                if (REG.Lesen("Config", "Systemsounds") == "An") SoundPlayer.Sound_Mautstation_Passiert();
                Logging.WriteClientLog("[INFO] Maut durchfahren: " + response);
            } catch(Exception ex)
            {
                Logging.WriteClientLog("[ERROR] Fehler beim Mautdurchfahrt " + ex.Message);
            }

        }

        private void TelemetryFerry(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, string> post_param = new Dictionary<string, string>();
                post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
                post_param.Add("SOURCE_NAME", Truck_Daten.FERRY_SOURCE_NAME);
                post_param.Add("TARGET_NAME", Truck_Daten.FERRY_TARGET_NAME);
                post_param.Add("PAY_AMOUNT", Truck_Daten.FERRY_PAY_AMOUNT.ToString());

                string response = API.HTTPSRequestPost(API.transport, post_param);
                Logging.WriteClientLog("[INFO] TRANSPORT - FÄHRE - EVENT: " + response);
            } catch (Exception ex)
            {
                Logging.WriteClientLog("[ERROR] Fehler bei Transport (Schiff)" + ex.Message);
            }

        }

        private void TelemetryTrain(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, string> post_param = new Dictionary<string, string>();
                post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
                post_param.Add("SOURCE_NAME", Truck_Daten.TRAIN_SOURCE_NAME.ToString());
                post_param.Add("TARGET_NAME", Truck_Daten.TRAIN_TARGET_NAME.ToString());
                post_param.Add("PAY_AMOUNT", Truck_Daten.TRAIN_PAY_AMOUNT.ToString());

                string response = API.HTTPSRequestPost(API.transport, post_param);
                Logging.WriteClientLog("[INFO] TRANSPORT - TRAIN - EVENT: " + response);
            } catch (Exception ex)
            {
                Logging.WriteClientLog("[ERROR] Fehler bei Transport (Zug) " + ex.Message);
            }

        }

        private void TelemetryRefuel(object sender, EventArgs e)
        {
            Logging.WriteClientLog("[INFO] Refuel-Event - Liter: " + Truck_Daten.LITER_GETANKT.ToString());
        }

        private void TelemetryRefuelEnd(object sender, EventArgs e)
        {
            Logging.WriteClientLog("[INFO] Refuel-END Event - Liter: " + Truck_Daten.LITER_GETANKT.ToString());
        }

        private void TelemetryRefuelPayed(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(REG.Lesen("Config", "TOUR_ID_ETS2")) && string.IsNullOrEmpty(REG.Lesen("Config", "TOUR_ID_ATS")))
                {
                    tour_id_tanken = "0";
                }
                else
                {
                    if (Truck_Daten.SPIEL == "Ets2")
                    {
                        tour_id_tanken = REG.Lesen("Config", "TOUR_ID_ETS2");
                    } else
                    {
                        tour_id_tanken = REG.Lesen("Config", "TOUR_ID_ATS");
                    }
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
            } catch (Exception ex)
            {
                Logging.WriteClientLog("[ERROR] Fehler bei RefuelPayed " + ex.Message);
            }
           
        }

        private void Telemetry_Data(SCSTelemetry data, bool updated)
        {
            try
            {
                if (!InvokeRequired)
                {
                    // ALLGEMEIN
                    Truck_Daten.SDK_AKTIVE = !data.SdkActive;
                    Truck_Daten.SPIEL = data.Game.ToString();

                    Truck_Daten.TELEMETRY_VERSION = "Telemetry: " + data.TelemetryVersion.Major.ToString() + "." + data.TelemetryVersion.Minor.ToString();
                    Truck_Daten.DLL_VERSION = "DLL: " + data.DllVersion.ToString();
                    Truck_Daten.EURO_DOLLAR = Truck_Daten.SPIEL == "Ets2" ? "€" : "$";
                    Truck_Daten.TONNEN_LBS = Truck_Daten.SPIEL == "Ets2" ? " t " : " lb ";
                    Truck_Daten.LITER_GALLONEN = (Truck_Daten.SPIEL == "Ets2") ? " L" : " Gal.";
                    Truck_Daten.KMH_MI = Truck_Daten.SPIEL == "Ets2" ? "KM/H" : "mp/h";
                    Truck_Daten.KM_MI = Truck_Daten.SPIEL == "Ets2" ? "KM" : "mi";

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


                    if(Truck_Daten.SPIEL == "Ats")
                    {
                        Truck_Daten.GEWICHT = (data.JobValues.CargoValues.Mass * 2.205 / 1000).ToString();
                        Truck_Daten.GEWICHT2 = (int)(data.JobValues.CargoValues.Mass * 2.205 / 1000);
                       

                        Truck_Daten.GESAMT_KM = (float)(data.JobValues.PlannedDistanceKm / 1.609);
                        Truck_Daten.REST_KM = (float)data.JobValues.PlannedDistanceKm / 1609;

                        Truck_Daten.GESAMT_KM_SA = (int)(data.JobValues.PlannedDistanceKm / 1.609);
                        Truck_Daten.REST_KM_SA = (int)data.NavigationValues.NavigationDistance / 1609;
                    } else
                    {
                        Truck_Daten.GEWICHT = (data.JobValues.CargoValues.Mass / 1000).ToString();
                        Truck_Daten.GEWICHT2 = (int)(data.JobValues.CargoValues.Mass / 1000);

                        Truck_Daten.GESAMT_KM = (float)data.JobValues.PlannedDistanceKm;
                        Truck_Daten.REST_KM = (float)data.JobValues.PlannedDistanceKm;

                        Truck_Daten.GESAMT_KM_SA = (int)data.JobValues.PlannedDistanceKm;
                        Truck_Daten.REST_KM_SA = (int)data.NavigationValues.NavigationDistance / 1000;
                    }



                    //MessageBox.Show(Truck_Daten.REST_KM_SA.ToString());
                   
           
                    //Truck_Daten.GESAMT_KM_SA = (int)data.JobValues.PlannedDistanceKm;

                    Truck_Daten.EINKOMMEN = (int)data.JobValues.Income;
                    Truck_Daten.FRACHTMARKT = data.JobValues.Market.ToString();
                    Truck_Daten.CARGO_LOADED = data.JobValues.CargoLoaded;
                    Truck_Daten.GEF_STRECKE = (int)data.GamePlay.JobDelivered.DistanceKm;
                    
                    // LKW DATEN
                    Truck_Daten.LKW_MODELL = data.TruckValues.ConstantsValues.Name;
                    Truck_Daten.LKW_HERSTELLER = data.TruckValues.ConstantsValues.Brand;
                    Truck_Daten.LKW_HERSTELLER_ID = data.TruckValues.ConstantsValues.BrandId;
                    Truck_Daten.WARNLICHT = data.TruckValues.CurrentValues.LightsValues.Beacon;

                    //Truck_Daten.SPEED =  (int)data.TruckValues.CurrentValues.DashboardValues.Speed.Kph;
                    Truck_Daten.SPEED = Truck_Daten.SPIEL == "Ets2" ? (int)data.TruckValues.CurrentValues.DashboardValues.Speed.Kph : (int)data.TruckValues.CurrentValues.DashboardValues.Speed.Mph;

                    Truck_Daten.ELEKTRIK_AN = data.TruckValues.CurrentValues.ElectricEnabled;
                    Truck_Daten.MOTOR_AN = data.TruckValues.CurrentValues.EngineEnabled;
                    Truck_Daten.PARKING_BRAKE = data.TruckValues.CurrentValues.MotorValues.BrakeValues.ParkingBrake;
                    Truck_Daten.BLINKER_LINKS = data.TruckValues.CurrentValues.LightsValues.BlinkerLeftOn;
                    Truck_Daten.BLINKER_RECHTS = data.TruckValues.CurrentValues.LightsValues.BlinkerRightOn;
                    Truck_Daten.GEAR = data.TruckValues.CurrentValues.MotorValues.GearValues.Selected;
              
                    // LICHTER
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
                    Truck_Daten.FAHRINFO_1 = "Du fährst mit " + Truck_Daten.GEWICHT2 + Truck_Daten.TONNEN_LBS + Truck_Daten.LADUNG_NAME + " von " + Truck_Daten.STARTORT + " nach " + Truck_Daten.ZIELORT;
                    Truck_Daten.REMAININGTIME = TimeSpan.FromSeconds(data.JobValues.RemainingDeliveryTime.Value);
                    Truck_Daten.RESTZEIT_INT = data.JobValues.RemainingDeliveryTime.Value; // FÜR FAHRTENABRECHNUNG POSITIVE UND NEGATIVE SEKUNDEN

                    if(Truck_Daten.FRACHTMARKT == "external_contracts")
                    {
                        Truck_Daten.RESTZEIT_INT = 0;
                        Truck_Daten.FAHRINFO_2 = "World of Trucks-Auftrag";
                    } else
                    {
                        if (Truck_Daten.REMAININGTIME >= TimeSpan.FromSeconds(1))
                        {
                            Truck_Daten.FAHRINFO_2 = "Restzeit: " + Truck_Daten.REMAININGTIME.Minutes + " Std. " + Truck_Daten.REMAININGTIME.Seconds + " Min.";
                        }
                        else
                        {
                            Truck_Daten.FAHRINFO_2 = "Restzeit: Abgelaufen.";
                        }
                    }


                    
                    // POSITION
                    Truck_Daten.POS_X = data.TruckValues.Positioning.Cabin.X;
                    Truck_Daten.POS_Y = data.TruckValues.Positioning.Cabin.Y;
                    Truck_Daten.POS_Z = data.TruckValues.Positioning.Cabin.Z;

                    // CANCEL TOUR
                    Truck_Daten.CANCEL_STRAFE = data.GamePlay.JobCancelled.Penalty;

                    // Sonstiges

                    // SCAHDENSBERECHNUNG LKW
                    float schaden1 = data.TruckValues.CurrentValues.DamageValues.Cabin * 100;
                    float schaden2 = data.TruckValues.CurrentValues.DamageValues.WheelsAvg * 100;
                    float schaden3 = data.TruckValues.CurrentValues.DamageValues.Chassis * 100;
                    float schaden4 = data.TruckValues.CurrentValues.DamageValues.Engine * 100;
                    float schaden5 = data.TruckValues.CurrentValues.DamageValues.Transmission * 100;
                    float schaden_total = schaden1 + schaden2 + schaden3 + schaden4 + schaden5;
                    Truck_Daten.LKW_SCHADEN = Math.Round((double)Enumerable.Max( new[] { schaden1, schaden2, schaden3, schaden4, schaden5 } ), 2);


                    // SCHADENSBERECHNUNG TRAILER
                    float tschaden1 = data.TrailerValues[0].DamageValues.Wheels * 100;
                    float tschaden2 = data.TrailerValues[0].DamageValues.Chassis * 100;
                    float tschaden3 = data.TrailerValues[0].DamageValues.Cargo * 100;
                    Truck_Daten.TRAILER_SCHADEN = Math.Round((double)Enumerable.Max(new[] { tschaden1, tschaden2, tschaden3 }),2);

                    // DELIVERED
                    Truck_Daten.FRACHTSCHADEN_ABGABE = data.GamePlay.JobDelivered.CargoDamage;
                    Truck_Daten.ABGABE_GEF_STRECKE = data.GamePlay.JobDelivered.DistanceKm;
                    Truck_Daten.AUTOPARKING = data.GamePlay.JobDelivered.AutoParked;
                    Truck_Daten.AUTOLOADING = data.GamePlay.JobDelivered.AutoLoaded;
                    Truck_Daten.EARNED_XP = data.GamePlay.JobDelivered.EarnedXp;
                    // TANKEN

                    Truck_Daten.LITER_GETANKT = data.GamePlay.RefuelEvent.Amount;
                    if(Truck_Daten.SPIEL == "Ets2")
                    {
                        Truck_Daten.FUEL_MAX = (int)data.TruckValues.ConstantsValues.CapacityValues.Fuel;
                        Truck_Daten.FUEL_GERADE = (int)data.TruckValues.CurrentValues.DashboardValues.FuelValue.Amount;
                        Truck_Daten.RESTSTRECKE = (int)data.TruckValues.CurrentValues.DashboardValues.FuelValue.Range;
                    } else
                    {
                        Truck_Daten.RESTSTRECKE = (int)(data.TruckValues.CurrentValues.DashboardValues.FuelValue.Range / 1.609);
                        Truck_Daten.FUEL_MAX = (int)(data.TruckValues.ConstantsValues.CapacityValues.Fuel / 3.785);
                        Truck_Daten.FUEL_GERADE = (int)(data.TruckValues.CurrentValues.DashboardValues.FuelValue.Amount / 3.785);
                    }
                
                    

                    // Transport FERRY
                    Truck_Daten.FERRY_SOURCE_NAME = data.GamePlay.FerryEvent.SourceName;
                    Truck_Daten.FERRY_TARGET_NAME = data.GamePlay.FerryEvent.TargetName;
                    Truck_Daten.FERRY_PAY_AMOUNT = (int)data.GamePlay.FerryEvent.PayAmount;

                    // Transport TRAIN
                    Truck_Daten.TRAIN_SOURCE_NAME = data.GamePlay.TrainEvent.SourceName;
                    Truck_Daten.TRAIN_TARGET_NAME = data.GamePlay.TrainEvent.TargetName;
                    Truck_Daten.TRAIN_PAY_AMOUNT = (int)data.GamePlay.TrainEvent.PayAmount;


                    // DISCORD
                    Update_Discord(Truck_Daten.LKW_HERSTELLER, Truck_Daten.LKW_MODELL, Truck_Daten.LADUNG_NAME, Truck_Daten.GEWICHT2, Truck_Daten.STARTORT, Truck_Daten.ZIELORT, Truck_Daten.LKW_HERSTELLER_ID, Truck_Daten.LKW_SCHADEN);

                    // GESCHWINDIGKEITS_LOGGEN
                    if (Truck_Daten.SPEED >= 101)
                    {
                        zu_schnell.Start();
                    }
                    else
                    {
                        zu_schnell.Stop();
                    }   
                }
            }
            catch
            { }
        }



        private void Update_Discord(string HERSTELLER, string MODELL, string FRACHT, int GEWICHT, string STARTORT, string ZIELORT, string BRAND_ID, double SCHADEN)
        {
            string truckdaten;
            string tourdaten;
            string DiscordLargeImageKey = "pj_512";

            if (string.IsNullOrEmpty(HERSTELLER))
            {
                truckdaten = "---";
                
            } else
            {
                truckdaten = HERSTELLER + " " + MODELL + "(" + SCHADEN + " %)";
               
            }
            if(string.IsNullOrEmpty(FRACHT))
            {
                tourdaten = "Gerade keine Tour...";
            } else
            {
                tourdaten = GEWICHT + " T " + FRACHT + " von " + STARTORT + " nach " + ZIELORT;
            }

            jobRPC = new RichPresence()
            {
               
                Details = "Ladung: " + FRACHT + "(" + GEWICHT + "t)",
                State = STARTORT + "->" + ZIELORT,

                Assets = new Assets()
                {
                    LargeImageKey = DiscordLargeImageKey,
                    LargeImageText = HERSTELLER + " " + MODELL,
                    SmallImageKey = "lkw",
                    SmallImageText = "v" + CLIENT_VERSION
                }
            };
         
            client.SetPresence(jobRPC);
        }

        private static string getTruckImageKey(string truck_brand_id)
        {
            string DiscordLargeImageKey = DefaultDiscordLargeImageKey;
            if (truck_brand_id == "renault")
            {
                DiscordLargeImageKey = "brand-renault";
            }
            else if (truck_brand_id == "scania")
            {
                DiscordLargeImageKey = "brand-scania";
            }
            else if (truck_brand_id == "mercedes")
            {
                DiscordLargeImageKey = "brand-mercedes";
            }
            else if (truck_brand_id == "volvo")
            {
                DiscordLargeImageKey = "brand-volvo";
            }
            return DiscordLargeImageKey;
        }






        private void Lade_Voreinstellungen()
        {
            try
            {

                if (string.IsNullOrWhiteSpace(REG.Lesen("Config", "BG_OPACITY")))
                    REG.Schreiben("Config", "BG_OPACITY", "1.0"); Truck_Daten.BG_OPACITY = "1.0"; bg_opacity.SelectedValue = "1.0";
                Truck_Daten.BG_OPACITY = REG.Lesen("Config", "BG_OPACITY"); bg_opacity.SelectedValue = REG.Lesen("Config", "BG_OPACITY");

                Farbschema.SelectedValue = REG.Lesen("Config", "Farbschema");

                if (string.IsNullOrWhiteSpace(REG.Lesen("Config", "FIRST_RUN")))
                    REG.Schreiben("Config", "FIRST_RUN", "0");

                if (string.IsNullOrWhiteSpace(REG.Lesen("Config", "ANTI_AFK_TEXT")))
                    REG.Schreiben("Config", "ANTI_AFK_TEXT", "TrucksLOG wünscht allen Truckern eine gute und sichere Fahrt!");

                if (string.IsNullOrWhiteSpace(REG.Lesen("Config", "ANTI_AFK_TIMER")))
                    REG.Schreiben("Config", "ANTI_AFK_TIMER", "4");

                antiafk_zeit.Value = Convert.ToInt32(REG.Lesen("Config", "ANTI_AFK_TIMER"));

                if (string.IsNullOrWhiteSpace(REG.Lesen("Config", "TOUR_ID_ETS2")))
                    REG.Schreiben("Config", "TOUR_ID_ETS2", "");

                if (string.IsNullOrWhiteSpace(REG.Lesen("Config", "TOUR_ID_ATS")))
                    REG.Schreiben("Config", "TOUR_ID_ATS", "");

                if (string.IsNullOrWhiteSpace(REG.Lesen("Config", "CLIENT_KEY")))
                    REG.Schreiben("Config", "CLIENT_KEY", "");

                if (string.IsNullOrWhiteSpace(REG.Lesen("Config", "ANTI_AFK_TIMER")))
                    REG.Schreiben("Config", "ANTI_AFK_TIMER", "4");

                if (string.IsNullOrWhiteSpace(REG.Lesen("Config", "Systemsounds")))
                    REG.Schreiben("Config", "Systemsounds", "An");
                Systemsounds.SelectedValue = REG.Lesen("Config", "Systemsounds");

                if (string.IsNullOrWhiteSpace(REG.Lesen("Config", "Farbschema")))
                    REG.Schreiben("Config", "Farbschema", "Dark.Blue");
                Farbschema.SelectedValue = REG.Lesen("Config", "Farbschema");


                if (string.IsNullOrWhiteSpace(REG.Lesen("Config", "Background")))
                {
                    Background_WEchsler.SelectedValue = "pj_7.jpg";
                    REG.Schreiben("Config", "Background", "pj_7.jpg");
                }
                else
                {
                    Background_WEchsler.SelectedValue = REG.Lesen("Config", "Background");
                }


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

                ETS_TOUR_delete.IsEnabled = !string.IsNullOrEmpty(REG.Lesen("Config", "TOUR_ID_ETS2"));
                ATS_TOUR_delete.IsEnabled = !string.IsNullOrEmpty(REG.Lesen("Config", "TOUR_ID_ATS"));

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
            try
            {
                Dictionary<string, string> post_param = new Dictionary<string, string>();
                post_param.Add("VERSION", CLIENT_VERSION);
                string response = API.HTTPSRequestPost(API.updatetext_uri, post_param);
                string response2 = response.Replace("<br/>", "");
                string response3 = response2.Replace("<hr/>", "");
                update_text.Text = response3;

            } catch (Exception ex)
            {
                Logging.WriteClientLog("[ERROR] Fehler beim Anzeigen der Update News " + ex.Message);
            }

            lade_Patreon();

            if (Truck_Daten.PATREON_LEVEL == 0)
            {
                REG.Schreiben("Config", "ANTI_AFK_TEXT", "TrucksLOG wünscht allen Truckern eine angenehme und sichere Fahrt!");
                anti_ak_text.Text = "TrucksLOG wünscht allen Truckern eine angenehme und sichere Fahrt!";
                anti_ak_text.MaxLength = 0;
                laenge_antiafk_text.Content = "Keine Änderung möglich";
            }
            else if (Truck_Daten.PATREON_LEVEL == 1)
            {
                anti_ak_text.Text = "TrucksLOG wünscht eine gute Fahrt!";
                anti_ak_text.MaxLength = 50;
                laenge_antiafk_text.Content = "Max. 50 Zeichen";
            }
            else if (Truck_Daten.PATREON_LEVEL == 2)
            {
                anti_ak_text.Text = "TrucksLOG wünscht allen Truckern eine angenehme und sichere Fahrt!";
                anti_ak_text.MaxLength = 100;
                laenge_antiafk_text.Content = "Max. 100 Zeichen";
            }
            else if (Truck_Daten.PATREON_LEVEL == 3)
            {
                anti_ak_text.Text = "TrucksLOG wünscht allen Truckern eine angenehme und sichere Fahrt!";
                anti_ak_text.MaxLength = 150;
                laenge_antiafk_text.Content = "Max. 150 Zeichen";
            }

            anti_afk.SelectedValue = "Aus";
            anti_afk_timer.Stop();

        }


        private void Beta_Tester(object sender, RoutedEventArgs e)
        {
            try
            {
                //Process.Start("https://projekt-janus.de/beta_bewerbung.php");
            }
            catch (Exception ex)
            {
                Logging.WriteClientLog("[ERROR] Fehler Beta_Tester: " + ex.Message);
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
                Logging.WriteClientLog("[INFO] PayPal Spenden Button geklickt");
            }
            catch (Exception ex)
            {
                Logging.WriteClientLog("[ERROR] Fehler beim Spende_Kaffee: " + ex.Message);
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
            Logging.WriteClientLog("[INFO] Spende_PayPal wurde angeklickt!");
            Process.Start("https://paypal.me/ErIstWiederDa/2,00");
        }

        private void patreon_link(object sender, RoutedEventArgs e)
        {
            Logging.WriteClientLog("[INFO] Patreon Link wurde angeklickt!");
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
            Logging.WriteClientLog("[INFO] Andras.TV wurde angeklickt!");
            Process.Start("https://www.twitch.tv/andras_tv");
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
            Hauptfenster.WindowState = WindowState.Minimized;
        
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Logging.WriteClientLog("[INFO] Programm wurde beendet!");
            try
            {
                System.Windows.Application.Current.Shutdown();
            } catch(Exception ex)
            {
                Logging.WriteClientLog("[ERROR] Fehler beim herunterfahren des Client: " + ex.Message);
            }
          
        }

        private void settings_Click(object sender, RoutedEventArgs e)
        {
            Pfad_Angeben pf2 = new Pfad_Angeben();
            pf2.ShowDialog();
        }

        private void tmp_starten_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(REG.Lesen("Pfade", "TMP_PFAD")))
                {
                    Process.Start(REG.Lesen("Pfade", "TMP_PFAD"));
                }
                else
                {
                    msg.Schreiben("Fehler bei der Pfadangabe - TruckersMP", "Der Pfad zu TruckersMP wurde nicht angegeben !" + Environment.NewLine + "Bitte klicke unter den Buttons auf das Zahnradsymbol.");
                    Logging.WriteClientLog("[ERROR] Fehler beim Starten von TMP. Kein Pfad angegeben");
                }
            }
            catch (Exception ex)
            {
                Logging.WriteClientLog("[ERROR] Exception throw " + ex.Message + ex.StackTrace);
            }
        }

        private void ats_starten_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(!string.IsNullOrEmpty(REG.Lesen("Pfade", "ATS_PFAD")))
                {
                    Process.Start(REG.Lesen("Pfade", "ATS_PFAD"));
                } else
                {
                    msg.Schreiben("Fehler bei der Pfadangabe ATS", "Der Pfad zu ATS wurde nicht angegeben !" + Environment.NewLine + "Bitte klicke unter den Buttons auf das Zahnradsymbol.");
                    Logging.WriteClientLog("[ERROR] Fehler beim Starten von ATS im SinglePlayer");
                }
            } catch (Exception ex)
            {
                Logging.WriteClientLog("[ERROR] Fehler beim Starten von ATS im SinglePlayer" + ex.Message + ex.StackTrace);
            }
           
        }

        private void ets_starten_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(REG.Lesen("Pfade", "ETS2_PFAD")))
                {
                    Process.Start(REG.Lesen("Pfade", "ETS2_PFAD"));
                }
                else
                {
                    msg.Schreiben("Fehler bei der Pfadangabe - ETS2", "Der Pfad zu Euro Truck Simulator 2 wurde nicht angegeben !" + Environment.NewLine + "Bitte klicke unter den Buttons auf das Zahnradsymbol.");
                    Logging.WriteClientLog("[ERROR] Fehler beim Starten von ETS2. Kein Pfad angegeben");
                }
            }
            catch (Exception ex)
            {
                Logging.WriteClientLog("[ERROR] Exception throw " + ex.Message + ex.StackTrace);
            }
        }

        private void spielpfade_aendern(object sender, RoutedEventArgs e)
        {
            Pfad_Angeben pf3 = new Pfad_Angeben();
            pf3.ShowDialog();
        }

        private void ContextMenu_Click(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as System.Windows.Controls.MenuItem;
            System.Windows.MessageBox.Show($"{item.Header} was clicked");
        }

        private void Hauptfenster_Loaded(object sender, RoutedEventArgs e)
        {
            if(Updates_FUER() == "ALLE")
            {
                AutoUpdater.ShowSkipButton = false;
                AutoUpdater.ShowRemindLaterButton = false;
                AutoUpdater.Start(UpdateString);
            }

            if (Updates_FUER() == "BETA")
            {
                if (Convert.ToInt32(Ist_BETA_Tester()) >= 1)
                {
                    AutoUpdater.ShowSkipButton = false;
                    AutoUpdater.ShowRemindLaterButton = false;
                    AutoUpdater.Start(UpdateString);
                }
            }

            set_online();
            lade_Punktekonto();
        }

        private async void OnlineCheck()
        {

            //Logging.WriteClientLog("Client ist das erste mal geöffnet worden...");

            var metroWindow = (Application.Current.MainWindow as MetroWindow);

            Dictionary<string, string> post_param = new Dictionary<string, string>();
            post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
            string response = API.HTTPSRequestPost(API.onlinecheck, post_param);
            if (Convert.ToInt32(response) >= 1)
            {
                await metroWindow.ShowMessageAsync("Mehrmaliges Ausführen des Clients...", "Der Client darf nur 1x gestartet werden und wird deshalb jetzt beendet.");
                //Logging.WriteClientLog("[ERROR] -> Spielclient wurde mehrmals Ausgeführt");
                Application.Current.Shutdown();
            }
        }

        private static string Updates_FUER()
        {
            Dictionary<string, string> post_param = new Dictionary<string, string>();
            post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
            return API.HTTPSRequestPost(API.updates, post_param);
        }

        private static string Ist_BETA_Tester()
        {
            Dictionary<string, string> post_param = new Dictionary<string, string>();
            post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
            return API.HTTPSRequestPost(API.beta_tester, post_param);
        }


        private void set_online()
        {
            Dictionary<string, string> post_param = new Dictionary<string, string>();
            post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
            post_param.Add("STATUS", "ONLINE");
            post_param.Add("GAME", Truck_Daten.SPIEL);
            post_param.Add("POS_X", Truck_Daten.POS_X.ToString());
            post_param.Add("POS_Y", Truck_Daten.POS_Y.ToString());
            post_param.Add("POS_Z", Truck_Daten.POS_Z.ToString());
            string response = API.HTTPSRequestPost(API.c_online, post_param);
        }

        private void AntiAFK_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if ((string)anti_afk.SelectedValue == "An")
            {
                anti_afk_timer.Tick += anti_afk_timer_Tick;
                if(Truck_Daten.SDK_AKTIVE == false)
                {
                    anti_afk_timer.Start(); 
                } else
                {
                    msg.Schreiben("Fehler", "Starte zuerst ein Spiel !");
                    anti_afk.SelectedValue = "Aus";
                    anti_afk_timer.Stop();
                }
               

                Logging.WriteClientLog("[INFO] SendKeys gestartet...");
            } else
            {
                anti_afk_timer.Stop();
                Logging.WriteClientLog("[INFO] ANTI_AFK gestoppt");
            }
        }

        private void anti_afk_text_save_Click(object sender, RoutedEventArgs e)
        {
            if(Truck_Daten.PATREON_LEVEL >= 0)
            {
                REG.Schreiben("Config", "ANTI_AFK_TEXT", anti_ak_text.Text);
                REG.Schreiben("Config", "ANTI_AFK_TIMER", antiafk_zeit.Value.ToString());
                REG.Schreiben("Config", "ANTI_AFK_TIMER", antiafk_zeit.Value.ToString());
                anti_ak_text.Text = anti_ak_text.Text;
            }
            
        }

        private void Hauptfenster_Closed(object sender, EventArgs e)
        {
            Dictionary<string, string> post_param = new Dictionary<string, string>();
            post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
            post_param.Add("STATUS", "OFFLINE");
            string response = API.HTTPSRequestPost(API.c_online, post_param);
        }

        private void Oeffne_Andras_TV(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.twitch.tv/andras_tv");
        }

        private async void Werksteinstellungen_Click(object sender, RoutedEventArgs e)
        {
            var metroWindow = (Application.Current.MainWindow as MetroWindow);
        
            var result = await metroWindow.ShowMessageAsync("Client Reset durchführen ?", "Soll der Client wirklich Zurückgesetzt werden ?" + Environment.NewLine + "Du musst alle Pfade und den Client Key neu eintragen !", MessageDialogStyle.AffirmativeAndNegative);
            
            if (result == MessageDialogResult.Affirmative)
            {
                if (SubKeyExist(@"Software\Projekt-Janus"))
                {
                    Registry.CurrentUser.DeleteSubKeyTree(@"Software\Projekt-Janus");
                }
                await metroWindow.ShowMessageAsync("Client Reset Erfolgreich durchgeführt !", "Der Client wird nun beendet. Bitte starte den Client einfach neu.", MessageDialogStyle.Affirmative);
                Application.Current.Shutdown();
            }
            else
            {
                await metroWindow.ShowMessageAsync("Reset", "Der Reset wurde nicht durchgeführt !");
            }


    
        }

        private bool SubKeyExist(string Subkey)
        {
            RegistryKey myKey = Registry.CurrentUser.OpenSubKey(Subkey);
            if (myKey == null)
                return false;
            else
                return true;
        }

        private void bg_opacity_ValueChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                REG.Schreiben("Config", "BG_OPACITY", (string)bg_opacity.SelectedValue);

                Truck_Daten.BG_OPACITY = (string)bg_opacity.SelectedValue;

                this.Hauptfenster.Opacity = (double)bg_opacity.SelectedValue;

                MessageBox.Show(this.Hauptfenster.Opacity.ToString());

            } catch
            {

            }
        }

        private void Hauptfenster_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Dictionary<string, string> post_param = new Dictionary<string, string>();
            post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
            post_param.Add("STATUS", "OFFLINE");
            string response = API.HTTPSRequestPost(API.c_online, post_param);
        }

        private void ets_tour_delete(object sender, RoutedEventArgs e)
        {

            Dictionary<string, string> post_param = new Dictionary<string, string>();
            post_param.Add("TOUR_ID", REG.Lesen("Config", "TOUR_ID_ETS2"));
            post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
            string response = API.HTTPSRequestPost(API.delete_tour, post_param);

                REG.Schreiben("Config", "TOUR_ID_ETS2", "");
            ETS_TOUR_delete.IsEnabled = false;
                msg.Schreiben("Tour ETS2 gelöscht", "Die Tour wurde in ETS2 entfernt. Du kannst jetzt einfach eine neue Tour starten...");

            
        }

        private void ats_tour_delete(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> post_param = new Dictionary<string, string>();
            post_param.Add("TOUR_ID", REG.Lesen("Config", "TOUR_ID_ATS"));
            post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
            string response = API.HTTPSRequestPost(API.delete_tour, post_param);
    
                REG.Schreiben("Config", "TOUR_ID_ATS", "");
            ATS_TOUR_delete.IsEnabled = false;
                msg.Schreiben("Tour ATS gelöscht", "Die Tour wurde in ATS entfernt. Du kannst jetzt einfach eine neue Tour starten...");

       }

        private void Client_Update_Manuell(object sender, RoutedEventArgs e)
        {
                AutoUpdater.Start(UpdateString);
        }

        private void spedv_anzeige_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("SpedV läuft bei dir mit unserem Client zusammen.\n\nDies ist im Normalfall kein Problem. Sollte es dennoch zu Problemen mit der Tour-Annahme oder Abgabe kommen, versuche bitte zuerst unseren Client ohne SpedV laufen zu lassen.\n\nVielen Dank dass du unser Tool benutzt!\nThommy", "Dual-Apps erkannt", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void trucksbook_anzeige_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Der TrucksBook Client läuft bei dir mit unserem Client zusammen.\n\nIn der Vergangenheit kam es dadurch zu Problemen mit der Annahme oder Abgabe der Touren. Sollte es zu solchen Problemen kommen, versuche bitte zuerst unseren Client ohne TrucksBook laufen zu lassen.\nFalls dies nicht Funktioniert, kontaktiere uns bitte über Discord.\n\nVielen Dank dass du unser Tool benutzt!\nThommy", "Dual-Apps erkannt", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}