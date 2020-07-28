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
using System.Reflection;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Input;
using WindowsInput.Native;
using WindowsInput;
using System.Threading;
using ControlzEx.Native;
using DiscordRPC;
using DiscordRPC.Logging;
using Microsoft.Win32;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Net.NetworkInformation;

namespace Janus_Client_V1
{
    public partial class MainWindow
    {
        public DiscordRpcClient client;
        private static RichPresence jobRPC;
        private const string DiscordAppID = "730374187025170472";
        private const string DefaultDiscordLargeImageKey = "pj_512";


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
            Logging.Make_Log_File();
            // BETA_CHECK();

            Lade_Voreinstellungen();

        

            // DISCORD
            client = new DiscordRpcClient(DiscordAppID);
            client.Initialize();
            var timer = new System.Timers.Timer(150);
            timer.Elapsed += (sender, args) => { client.Invoke(); };
            timer.Start();
            // DISCORD ENDE

            credit_text.Content = "Ein Dank geht an meine Tester:" + Environment.NewLine;
            credit_text.Content += " - Quasselboy Patti [COO]" + Environment.NewLine;
            credit_text.Content += " - Daniel1983 [Beta-Tester]" + Environment.NewLine;
            credit_text.Content += " - TOBI_𝟙Ƽ⊘५ [Beta-Tester]" + Environment.NewLine;
            credit_text.Content += " - Angelo Riechmann [WebDesigner]" + Environment.NewLine;
            credit_text.Content += "Einen Extra Dank an Quasselboy / Patti der mich" + Environment.NewLine + "seit Anbeginn der Zeit unterstützt." + Environment.NewLine;
            credit_text.Content += "und natürlich auch an" + Environment.NewLine;
            credit_text.Content += "unsere(n) Live-Streamer:" + Environment.NewLine;

            Logging.WriteClientLog("Version: " + CLIENT_VERSION);

            useronline_timer.Interval = TimeSpan.FromSeconds(5);
            useronline_timer.Tick += Useronline_Tick;
            useronline_timer.Start();

            zu_schnell.Interval = TimeSpan.FromSeconds(1);
            zu_schnell.Tick += zu_schnell_tick;

            // JOB UPDATE TIMER
            job_update_timer.Interval = TimeSpan.FromSeconds(2);

            // ANTI_AFK TIMER
            anti_afk_timer.Interval = TimeSpan.FromMinutes(Convert.ToInt32(REG.Lesen("Config", "ANTI_AFK_TIMER")));
            // anti_afk_timer.Interval = TimeSpan.FromMinutes(1);

            if (string.IsNullOrEmpty(REG.Lesen("Config", "CLIENT_KEY")))
            {
                Pfad_Angeben pf = new Pfad_Angeben();
                pf.ShowDialog();
                return;
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
                if (Truck_Daten.SPEED == 0)
                {
                    if (Truck_Daten.SPIEL == "Ets2")
                    {
                        BringMainWindowToFront("eurotrucks2");
                    } else
                    {
                        BringMainWindowToFront("amtrucks");
                    }
                    sim.Keyboard.KeyPress(VirtualKeyCode.VK_Y);
                    sim.Keyboard.TextEntry("PJ-BOT:");
                    sim.Keyboard.TextEntry(REG.Lesen("Config", "ANTI_AFK_TEXT"));
                    sim.Keyboard.KeyPress(VirtualKeyCode.RETURN);
                    Logging.WriteClientLog("[INFO] Keys gesendet! Geschwindigkeit: " + Truck_Daten.SPEED);
                } else
                {
                    Logging.WriteClientLog("[INFO] Keys nicht gesendet! Geschwindigkeit: " + Truck_Daten.SPEED);
                }

            } catch (Exception ex)
            {
                Logging.WriteClientLog("[ERROR] Fehler bei SendKeys " + ex.Message);
            }

        }

        private void zu_schnell_tick(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, string> post_param = new Dictionary<string, string>();
                post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
                string response = API.HTTPSRequestPost(API.user_zu_schnell, post_param);
            }
            catch (Exception ex)
            {
                Logging.WriteClientLog("[ERROR] Fehler bei SendKeys " + ex.Message);
            }

        }



        private void setzt_antiAFK()
        {
            if (Truck_Daten.PATREON_LEVEL == 0)
            {
                REG.Schreiben("Config", "ANTI_AFK_TEXT", "Projekt-Janus.de wünscht allen Truckern eine angenehme und sichere Fahrt!");
                anti_ak_text.Text = "Projekt-Janus.de wünscht allen Truckern eine angenehme und sichere Fahrt!";
                anti_ak_text.MaxLength = 0;
                laenge_antiafk_text.Content = "Keine Änderung möglich";
            }
            else if (Truck_Daten.PATREON_LEVEL == 1)
            {
                anti_ak_text.MaxLength = 50;
                laenge_antiafk_text.Content = "Max. 50 Zeichen";
            }
            else if (Truck_Daten.PATREON_LEVEL == 2)
            {
                anti_ak_text.MaxLength = 100;
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
                Truck_Daten.ONLINEUSER = "User: " + response_online;
            } catch { }
        }

        private void timer_Tick(object sender, EventArgs e)
            {
            try
            {
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
                    post_param.Add("REST_KM", ((float)Truck_Daten.REST_KM / 1000).ToString());
                } else
                {
                    post_param.Add("REST_KM", ((float)Truck_Daten.REST_KM / 1609).ToString());
                }
               
                post_param.Add("FRACHTSCHADEN", Truck_Daten.FRACHTSCHADEN.ToString());
                string response = API.HTTPSRequestPost(API.job_update, post_param);

               // Logging.WriteClientLog("[UPDATE] Tour-Update: " + Truck_Daten.FRACHTSCHADEN.ToString() + ((float)Truck_Daten.REST_KM / 1000).ToString());

            } catch (Exception ex)
            {
                Logging.WriteClientLog("[ERROR] Fehler beim Tour-Update " + ex.Message);
            }


            Server_PingAsync();
        }

        protected void Server_PingAsync()
        {
            Ping ping = new Ping();
            try
            {
                PingReply pingresult_Xenonserver = ping.Send("rdbs5.xenonserver.de");
                Server_1_Icon_Online.Visibility = Visibility.Visible;

            } catch
            {
                Server_1_Icon_Offline.Visibility = Visibility.Visible;

                MessageBox.Show("Die Server sind Offline.\n\nDer Client wird jetzt beendet !", "Fehler in der Kommunikation mit den Servern!", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
            
            
        }


        private void BETA_CHECK()
        {
            try
            {
                Dictionary<string, string> post_param = new Dictionary<string, string>();
                post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
                int response = Convert.ToInt32(API.HTTPSRequestPost(API.beta, post_param));

                if (response == 0)
                {
                    MessageBox.Show("Du bist leider kein BETA-TESTER oder noch nicht freigeschaltet." + Environment.NewLine + "Bitte wende dich auf unserem Discord Server: https://discord.gg/jvD2Y7H an einen der Verantwortlichen." + Environment.NewLine + Environment.NewLine + "Diese Anwendung wird jetzt beendet !", "Fehler BETA-Tester", MessageBoxButton.OK, MessageBoxImage.Error);

                    string key = REG.Lesen("Config", "CLIENT_KEY");
                    Logging.WriteClientLog("[DISMISS] Fehlversuch sich als Beta-Tester anzumelden wurde an die Administration gemeldet !");
                    EmailHandler mail = new EmailHandler();
                    mail.Email_BETA_TESTER_FEHLER(key, "Unzulässige Beta-Tester Anmeldung");
                } else
                {
                    Logging.WriteClientLog("[DISMISS] BETA-Tester Nutzerstatus: " + response);
                }
            } catch (Exception ex)
            {
                Logging.WriteClientLog("[DISMISS] Fehler beim Abfrage des BETA-Tester-Status: " + ex.Message + ex.StackTrace);
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
                //MessageBox.Show("Fehler beim Abrufen PS!" + ex.Message + ex.StackTrace);
                Truck_Daten.PATREON_LEVEL = 0;
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
                    Logging.WriteClientLog("[INFO ETS] Tour-ID ETS ist leer !");
                    try
                    {
                        REG.Schreiben("Config", "TOUR_ID_ETS2", GenerateString());
                        Logging.WriteClientLog("[INFO ETS] Tour-ID ETS wurde neu Generiert !");
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
                    Logging.WriteClientLog("[INFO ATS] Tour-ID ATS ist leer !");
                    try
                    {
                        REG.Schreiben("Config", "TOUR_ID_ATS", GenerateString());
                        Logging.WriteClientLog("[INFO ATS] Tour-ID ATS wurde neu Generiert !");
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

                if(response == "Neue")
                {
            
                    if (Truck_Daten.SPIEL == "Ets2")
                    {
                        REG.Schreiben("Config", "TOUR_ID_ETS2", GenerateString());
                    }
                    else
                    {
                        REG.Schreiben("Config", "TOUR_ID_ATS", GenerateString());
                    }
                } else
                {
               
                    if (Truck_Daten.SPIEL == "Ets2")
                    {
                        REG.Schreiben("Config", "TOUR_ID_ETS2", response);
                    }
                    else
                    {
                        REG.Schreiben("Config", "TOUR_ID_ATS", response);
                    }
                } 

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

                if(Truck_Daten.SPIEL == "Ets2")
                {
                    REG.Schreiben("Config", "TOUR_ID_ETS2", "");
                } else
                {
                    REG.Schreiben("Config", "TOUR_ID_ATS", "");
                }

                REG.Schreiben("Config", "Frachtmarkt", "");

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
                //post_param.Add("FRACHTSCHADEN_ABGABE", Truck_Daten.FRACHTSCHADEN_ABGABE.ToString());
                post_param.Add("STRECKE", Truck_Daten.ABGABE_GEF_STRECKE.ToString());
                post_param.Add("EARNED_XP", Truck_Daten.EARNED_XP.ToString());
                string response = API.HTTPSRequestPost(API.job_finish, post_param);
                    if (Truck_Daten.SPIEL == "Ets2") {
                            REG.Schreiben("Config", "TOUR_ID_ETS2", "");
                        } else
                        {
                            REG.Schreiben("Config", "TOUR_ID_ATS", "");
                    }

                REG.Schreiben("Config", "Frachtmarkt", "");

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
                    tour_id_tanken = "";
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

                    Truck_Daten.TELEMETRY_VERSION = "Telemetry: " + data.TelemetryVersion.Major.ToString() + "." + data.TelemetryVersion.Minor.ToString();
                    Truck_Daten.DLL_VERSION = "DLL: " + data.DllVersion.ToString();
                    Truck_Daten.GAMEVERSION = data.GameVersion.Major + "." + data.GameVersion.Minor;

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

                    // Gewicht
                    Truck_Daten.GEWICHT = (data.JobValues.CargoValues.Mass / 1000).ToString();
                    Truck_Daten.GEWICHT2 = (int)(data.JobValues.CargoValues.Mass / 1000);

                    
                    

                    
                    if(Truck_Daten.SPIEL == "Ets2")
                    {
                        Truck_Daten.GESAMT_KM = (float)data.JobValues.PlannedDistanceKm;
                        Truck_Daten.GESAMT_KM_SA = (int)data.JobValues.PlannedDistanceKm;

                        Truck_Daten.REST_KM = (float)data.NavigationValues.NavigationDistance;
                        Truck_Daten.REST_KM_SA = (int)data.NavigationValues.NavigationDistance / 1000;
                    } else
                    {
                        Truck_Daten.GESAMT_KM = (float)data.JobValues.PlannedDistanceKm / 1609;
                        Truck_Daten.GESAMT_KM_SA = (int)data.JobValues.PlannedDistanceKm / 1609;

                        Truck_Daten.REST_KM = (float)data.NavigationValues.NavigationDistance / 1609;
                        Truck_Daten.REST_KM_SA = (int)data.NavigationValues.NavigationDistance / 1609;

                    }

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
        
                    Truck_Daten.ELEKTRIK_AN = data.TruckValues.CurrentValues.ElectricEnabled;
                    Truck_Daten.MOTOR_AN = data.TruckValues.CurrentValues.EngineEnabled;
                    Truck_Daten.PARKING_BRAKE = data.TruckValues.CurrentValues.MotorValues.BrakeValues.ParkingBrake;
                    Truck_Daten.BLINKER_LINKS = data.TruckValues.CurrentValues.LightsValues.BlinkerLeftOn;
                    Truck_Daten.BLINKER_RECHTS = data.TruckValues.CurrentValues.LightsValues.BlinkerRightOn;
                    Truck_Daten.GEAR = data.TruckValues.CurrentValues.MotorValues.GearValues.Selected;
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
                    

                    if(Truck_Daten.SPIEL == "Ets2")
                    {
                        Truck_Daten.LITER_GETANKT = data.GamePlay.RefuelEvent.Amount;
                        Truck_Daten.FUEL_MAX = (int)data.TruckValues.ConstantsValues.CapacityValues.Fuel;
                        Truck_Daten.FUEL_GERADE = (int)data.TruckValues.CurrentValues.DashboardValues.FuelValue.Amount;
                        Truck_Daten.STRECKE_MIT_FUEL = (int)data.TruckValues.CurrentValues.DashboardValues.FuelValue.Range;
                    } else
                    {
                        Truck_Daten.LITER_GETANKT = (float)(data.GamePlay.RefuelEvent.Amount / 3.785);
                        Truck_Daten.FUEL_MAX = (int)(data.TruckValues.ConstantsValues.CapacityValues.Fuel / 3.785);
                        Truck_Daten.FUEL_GERADE = (int)(data.TruckValues.CurrentValues.DashboardValues.FuelValue.Amount / 3.785);
                        Truck_Daten.STRECKE_MIT_FUEL = (int)(data.TruckValues.CurrentValues.DashboardValues.FuelValue.Range / 1.609);
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
                    if(Truck_Daten.TEMPOLIMIT != 0)
                    {
                        if (Truck_Daten.SPEED >= (Truck_Daten.TEMPOLIMIT + 10))
                        {
                            zu_schnell.Start();
                        }
                        else
                        {
                            zu_schnell.Stop();
                        }
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


        static int Max(params int[] numbers)
        {
            return numbers.Max();
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
                    REG.Schreiben("Config", "ANTI_AFK_TEXT", "Projekt-Janus.de wünscht allen Truckern eine gute und sichere Fahrt!");

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

                // Hintergrund Transparenz Beginn


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
                REG.Schreiben("Config", "ANTI_AFK_TEXT", "Projekt-Janus.de wünscht allen Truckern eine angenehme und sichere Fahrt!");
                anti_ak_text.Text = "Projekt-Janus.de wünscht allen Truckern eine angenehme und sichere Fahrt!";
                anti_ak_text.MaxLength = 0;
                laenge_antiafk_text.Content = "Keine Änderung möglich";
            }
            else if (Truck_Daten.PATREON_LEVEL == 1)
            {
                anti_ak_text.Text = "Projekt-Janus.de wünscht eine gute Fahrt!";
                anti_ak_text.MaxLength = 50;
                laenge_antiafk_text.Content = "Max. 50 Zeichen";
            }
            else if (Truck_Daten.PATREON_LEVEL == 2)
            {
                anti_ak_text.Text = "Projekt-Janus.de wünscht allen Truckern eine angenehme und sichere Fahrt!";
                anti_ak_text.MaxLength = 100;
                laenge_antiafk_text.Content = "Max. 100 Zeichen";
            }
            else if (Truck_Daten.PATREON_LEVEL == 3)
            {
                anti_ak_text.Text = "Projekt-Janus.de wünscht allen Truckern eine angenehme und sichere Fahrt!";
                anti_ak_text.MaxLength = 150;
                laenge_antiafk_text.Content = "Max. 150 Zeichen";
            }

            anti_afk.SelectedValue = "Aus";
            anti_afk_timer.Stop();

  
            if(!string.IsNullOrEmpty(REG.Lesen("Pfade", "ETS2_PFAD"))) {
                System.Diagnostics.FileVersionInfo fi = System.Diagnostics.FileVersionInfo.GetVersionInfo(REG.Lesen("Pfade", "ETS2_PFAD"));
                REG.Schreiben("Config", "VERSION_ETS2", fi.ProductVersion);
            }
            if (!string.IsNullOrEmpty(REG.Lesen("Pfade", "ATS_PFAD")))
            {
                System.Diagnostics.FileVersionInfo fi = System.Diagnostics.FileVersionInfo.GetVersionInfo(REG.Lesen("Pfade", "ATS_PFAD"));
                REG.Schreiben("Config", "VERSION_ATS", fi.ProductVersion);
            }

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
            System.Windows.Application.Current.Shutdown();
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
            AutoUpdater.Start("http://clientupdates.projekt-janus.de/version.xml");

            Dictionary<string, string> post_param = new Dictionary<string, string>();
            post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
            post_param.Add("STATUS", "ONLINE");
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
            if(Truck_Daten.PATREON_LEVEL >= 1)
            {
                REG.Schreiben("Config", "ANTI_AFK_TEXT", anti_ak_text.Text);
                REG.Schreiben("Config", "ANTI_AFK_TIMER", antiafk_zeit.Value.ToString());
                REG.Schreiben("Config", "ANTI_AFK_TIMER", antiafk_zeit.Value.ToString());
                anti_ak_text.Text = anti_ak_text.Text;
            }
            else
            {
                msg.Schreiben("Fehler!", "Du kannst mit deinem Patreon-Level den Text nicht ändern !" + Environment.NewLine + "Änderung des Textes und der Zeit erst ab Patreon Level 1 möglich.");
                anti_ak_text.Text = "Projekt-Janus.de wünscht allen Truckern eine angenehme und sichere Fahrt!";
                antiafk_zeit.Value = 4;
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
        
            var result = await metroWindow.ShowMessageAsync("Client Reset durchführen ?", "Soll der Client wirklich Resettet werden ?" + Environment.NewLine + "Du musst alle Pfade und den Client Key neu eintragen !", MessageDialogStyle.AffirmativeAndNegative);
            
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

    }
}