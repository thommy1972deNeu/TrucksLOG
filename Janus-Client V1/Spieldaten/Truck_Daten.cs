using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Janus_Client_V1.Spieldaten
{
    public class Truck_Daten : INotifyPropertyChanged
    {
        // Allgemeines
        private string telemetry_version;
        private string dll_version;
        private string client_version;
        private string fahrinfo_1;
        private string fahrinfo_2;
        private string spiel;
        private bool soundan;
        private string euro_dollar;
        private string ets_pfad;
        private string ats_pfad;
        private string tmp_pfad;

        // Truck Daten
        private string lkw_hersteller;
        private string lkw_hersteller_id;
        private string lkw_modell;
        private string licenseplate;
        private string licenseplate_country;
        private string licenseplate_country_id;
        private int gaenge_vor_max;
        private int gaenge_zurueck_max;
        private int rpm_max;
        private string shifter_typ;
        private bool elektrik_an;
        private bool motor_an;
        private bool licht_low;
        private bool licht_high;
        private bool parking_brake;
        private bool tank_warnung;
        private bool adblue_warnung;
        private bool air_warnung;
        private bool oil_warnung;
        private bool water_warnung;
        private bool battery_warnung;
        private int gear;
        private int speed;
        private string kmh_mi;
        private int tempomat;
        private int rpm;
        private bool scheibenwischer;
        private int tempolimit;
        private bool trailer_angehangen;
        // SCHÄDEN
        private double frachtschaden;
        private double frachtschaden2;
        private double lkw_schaden;
        private double trailer_schaden;

        // LICHTER
        private bool beam_low;
        private bool beam_high;
        private bool bremslicht;
        private bool reverse_light;
        private bool blinker_links;
        private bool blinker_rechts;
        private bool parking_light;

        // FUEL / ADBLUE
        private int fuel;
        private int fuel_max;

        // Warnings


        // JOB_CANCEL
        private double cancel_strafe;

        // JOB
        private bool cargo_loaded;
        private bool spezial_job;
        private string frachtmarkt;
        private string startort;
        private string startort_id;
        private string startfirma;
        private string startfirma_id;

        private string zielort;
        private string zielort_id;
        private string zielfirma;
        private string zielfirma_id;
        private int einkommen;
        private float gesamt_km;
        private float rest_km;
        private string gewicht;
        private string ladung_name;
        private string ladung_id;
        private int gef_strecke;
        // POSITION
        private double pos_x;
        private double pos_y;
        private double pos_z;

        // Job Abgabe
        private double abgabe_gef_strecke;

        // STRAFEN
        private int straf_betrag;
        private string grund;

        // Tollgate
        private int maut_betrag;

        // Tanken
        private float liter_getankt;


        // JOB ABGABE
        private double frachtschaden_abgabe;
        private bool autoparking;
        private bool autoloading;


        // TRANSPORT FERRY
        private int ferry_pay_amount;
        private string ferry_source_name;
        private string ferry_target_name;
        // TRANSPORT TRAIN
        private int train_pay_amount;
        private string train_source_name;
        private string train_target_name;

        // -----------------------------------------------------------------------------------

        public string TRAIN_TARGET_NAME
        {
            get { return train_target_name; }
            set
            {
                if (train_target_name != value)
                {
                    train_target_name = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public string TRAIN_SOURCE_NAME
        {
            get { return train_source_name; }
            set
            {
                if (train_source_name != value)
                {
                    train_source_name = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public int TRAIN_PAY_AMOUNT
        {
            get { return train_pay_amount; }
            set
            {
                if (train_pay_amount != value)
                {
                    train_pay_amount = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string FERRY_TARGET_NAME
        {
            get { return ferry_target_name; }
            set
            {
                if (ferry_target_name != value)
                {
                    ferry_target_name = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public string FERRY_SOURCE_NAME
        {
            get { return ferry_source_name; }
            set
            {
                if (ferry_source_name != value)
                {
                    ferry_source_name = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public int FERRY_PAY_AMOUNT
        {
            get { return ferry_pay_amount; }
            set
            {
                if (ferry_pay_amount != value)
                {
                    ferry_pay_amount = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public double ABGABE_GEF_STRECKE
        {
            get { return abgabe_gef_strecke; }
            set
            {
                if (abgabe_gef_strecke != value)
                {
                    abgabe_gef_strecke = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public string TMP_PFAD
        {
            get { return tmp_pfad; }
            set
            {
                if (tmp_pfad != value)
                {
                    tmp_pfad = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public string ATS_PFAD
        {
            get { return ats_pfad; }
            set
            {
                if (ats_pfad != value)
                {
                    ats_pfad = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public string ETS_PFAD
        {
            get { return ets_pfad; }
            set
            {
                if (ets_pfad != value)
                {
                    ets_pfad = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int GEF_STRECKE
        {
            get { return gef_strecke; }
            set
            {
                if (gef_strecke != value)
                {
                    gef_strecke = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool TRAILER_ANGEHANGEN
        {
            get { return trailer_angehangen; }
            set
            {
                if (trailer_angehangen != value)
                {
                    trailer_angehangen = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public string EURO_DOLLAR
        {
            get { return euro_dollar; }
            set
            {
                if (euro_dollar != value)
                {
                    euro_dollar = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public double CANCEL_STRAFE
        {
            get { return cancel_strafe; }
            set
            {
                if (cancel_strafe != value)
                {
                    cancel_strafe = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool SOUNDAN
        {
            get { return soundan; }
            set
            {
                if (soundan != value)
                {
                    soundan = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public double POS_Z
        {
            get { return pos_z; }
            set
            {
                if (pos_z != value)
                {
                    pos_z = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public double POS_Y
        {
            get { return pos_y; }
            set
            {
                if (pos_y != value)
                {
                    pos_y = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public double POS_X
        {
            get { return pos_x; }
            set
            {
                if (pos_x != value)
                {
                    pos_x = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public int TEMPOLIMIT
        {
            get { return tempolimit; }
            set
            {
                if (tempolimit != value)
                {
                    tempolimit = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public bool PARKING_BRAKE
        {
            get { return parking_brake; }
            set
            {
                if (parking_brake != value)
                {
                    parking_brake = value;
                    NotifyPropertyChanged();
                }
            }
        }



        public double TRAILER_SCHADEN
        {
            get { return trailer_schaden; }
            set
            {
                if (trailer_schaden != value)
                {
                    trailer_schaden = value;
                    NotifyPropertyChanged();
                }
            }
        }



        public double LKW_SCHADEN
        {
            get { return lkw_schaden; }
            set
            {
                if (lkw_schaden != value)
                {
                    lkw_schaden = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public bool BATTERY_WARNUNG
        {
            get { return battery_warnung; }
            set
            {
                if (battery_warnung != value)
                {
                    battery_warnung = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool WATER_WARNUNG
        {
            get { return water_warnung; }
            set
            {
                if (water_warnung != value)
                {
                    water_warnung = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool OIL_WARNUNG
        {
            get { return oil_warnung; }
            set
            {
                if (oil_warnung != value)
                {
                    oil_warnung = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool AIR_WARNUNG
        {
            get { return air_warnung; }
            set
            {
                if (air_warnung != value)
                {
                    air_warnung = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public bool ADBLUE_WARNUNG
        {
            get { return adblue_warnung; }
            set
            {
                if (adblue_warnung != value)
                {
                    adblue_warnung = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool TANK_WARNUNG
        {
            get { return tank_warnung; }
            set
            {
                if (tank_warnung != value)
                {
                    tank_warnung = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool LICHT_HIGH
        {
            get { return licht_high; }
            set
            {
                if (licht_high != value)
                {
                    licht_high = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool LICHT_LOW
        {
            get { return licht_low; }
            set
            {
                if (licht_low != value)
                {
                    licht_low = value;
                    NotifyPropertyChanged();
                }
            }
        }



        public string KMH_MI
        {
            get { return kmh_mi; }
            set
            {
                if (kmh_mi != value)
                {
                    kmh_mi = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool AUTOLOADING
        {
            get { return autoloading; }
            set
            {
                if (autoloading != value)
                {
                    autoloading = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public bool AUTOPARKING
        {
            get { return autoparking; }
            set
            {
                if (autoparking != value)
                {
                    autoparking = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public double FRACHTSCHADEN_ABGABE
        {
            get { return frachtschaden_abgabe; }
            set
            {
                if (frachtschaden_abgabe != value)
                {
                    frachtschaden_abgabe = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public float LITER_GETANKT
        {
            get { return liter_getankt; }
            set
            {
                if (liter_getankt != value)
                {
                    liter_getankt = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int MAUT_BETRAG
        {
            get { return maut_betrag; }
            set
            {
                if (maut_betrag != value)
                {
                    maut_betrag = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public string GRUND
        {
            get { return grund; }
            set
            {
                if (grund != value)
                {
                    grund = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int STRAF_BETRAG
        {
            get { return straf_betrag; }
            set
            {
                if (straf_betrag != value)
                {
                    straf_betrag = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string SPIEL
        {
            get { return spiel; }
            set
            {
                if (spiel != value)
                {
                    spiel = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string FAHRINFO_1
        {
            get { return fahrinfo_1; }
            set
            {
                if (fahrinfo_1 != value)
                {
                    fahrinfo_1 = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string FAHRINFO_2
        {
            get { return fahrinfo_2; }
            set
            {
                if (fahrinfo_2 != value)
                {
                    fahrinfo_2 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public float REST_KM
        {
            get { return rest_km; }
            set
            {
                if (rest_km != value)
                {
                    rest_km = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public double FRACHTSCHADEN
        {
            get { return frachtschaden; }
            set
            {
                if (frachtschaden != value)
                {
                    frachtschaden = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public double FRACHTSCHADEN2
        {
            get { return frachtschaden2; }
            set
            {
                if (frachtschaden2 != value)
                {
                    frachtschaden2 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string LADUNG_ID
        {
            get { return ladung_id; }
            set
            {
                if (ladung_id != value)
                {
                    ladung_id = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string LADUNG_NAME
        {
            get { return ladung_name; }
            set
            {
                if (ladung_name != value)
                {
                    ladung_name = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public string GEWICHT
        {
            get { return gewicht; }
            set
            {
                if (gewicht != value)
                {
                    gewicht = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public float GESAMT_KM
        {
            get { return gesamt_km; }
            set
            {
                if (gesamt_km != value)
                {
                    gesamt_km = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int EINKOMMEN
        {
            get { return einkommen; }
            set
            {
                if (einkommen != value)
                {
                    einkommen = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string ZIELFIRMA_ID
        {
            get { return zielfirma_id; }
            set
            {
                if (zielfirma_id != value)
                {
                    zielfirma_id = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string ZIELFIRMA
        {
            get { return zielfirma; }
            set
            {
                if (zielfirma != value)
                {
                    zielfirma = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public string ZIELORT_ID
        {
            get { return zielort_id; }
            set
            {
                if (zielort_id != value)
                {
                    zielort_id = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string ZIELORT
        {
            get { return zielort; }
            set
            {
                if (zielort != value)
                {
                    zielort = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public string STARTFIRMA_ID
        {
            get { return startfirma_id; }
            set
            {
                if (startfirma_id != value)
                {
                    startfirma_id = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public string STARTFIRMA
        {
            get { return startfirma; }
            set
            {
                if (startfirma != value)
                {
                    startfirma = value;
                    NotifyPropertyChanged();
                }
            }
        }



        public string STARTORT_ID
        {
            get { return startort_id; }
            set
            {
                if (startort_id != value)
                {
                    startort_id = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string STARTORT
        {
            get { return startort; }
            set
            {
                if (startort != value)
                {
                    startort = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public string FRACHTMARKT
        {
            get { return frachtmarkt; }
            set
            {
                if (frachtmarkt != value)
                {
                    frachtmarkt = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool SPEZIAL_JOB
        {
            get { return spezial_job; }
            set
            {
                if (spezial_job != value)
                {
                    spezial_job = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool CARGO_LOADED
        {
            get { return cargo_loaded; }
            set
            {
                if (cargo_loaded != value)
                {
                    cargo_loaded = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public bool PARKING_LIGHT
        {
            get { return parking_light; }
            set
            {
                if (parking_light != value)
                {
                    parking_light = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool BLINKER_RECHTS
        {
            get { return blinker_rechts; }
            set
            {
                if (blinker_rechts != value)
                {
                    blinker_rechts = value;
                    NotifyPropertyChanged();
                }
            }

        }
        public bool BLINKER_LINKS
        {
            get { return blinker_links; }
            set
            {
                if (blinker_links != value)
                {
                    blinker_links = value;
                    NotifyPropertyChanged();
                }
            }

        }



        public bool REVERSE_LIGHT
        {
            get { return reverse_light; }
            set
            {
                if (reverse_light != value)
                {
                    reverse_light = value;
                    NotifyPropertyChanged();
                }
            }

        }

        public bool BREMSLICHT
        {
            get { return bremslicht; }
            set
            {
                if (bremslicht != value)
                {
                    bremslicht = value;
                    NotifyPropertyChanged();
                }
            }

        }
        public bool BEAM_HIGH
        {
            get { return beam_high; }
            set
            {
                if (beam_high != value)
                {
                    beam_high = value;
                    NotifyPropertyChanged();
                }
            }

        }
        public bool BEAM_LOW
        {
            get { return beam_low; }
            set
            {
                if (beam_low != value)
                {
                    beam_low = value;
                    NotifyPropertyChanged();
                }
            }

        }


        public bool SCHEIBENWISCHER
        {
            get { return scheibenwischer; }
            set
            {
                if (scheibenwischer != value)
                {
                    scheibenwischer = value;
                    NotifyPropertyChanged();
                }
            }

        }

        public int TEMPOMAT
        {
            get { return tempomat; }
            set
            {
                if (tempomat != value)
                {
                    tempomat = value;
                    NotifyPropertyChanged();
                }
            }

        }


        public int RPM
        {
            get { return rpm; }
            set
            {
                if (rpm != value)
                {
                    rpm = value;
                    NotifyPropertyChanged();
                }
            }

        }

        public int SPEED
        {
            get { return speed; }
            set
            {
                if (speed != value)
                {
                    speed = value;
                    NotifyPropertyChanged();
                }
            }

        }



        public int GEAR
        {
            get { return gear; }
            set
            {
                if (gear != value)
                {
                    gear = value;
                    NotifyPropertyChanged();
                }
            }

        }


        public int FUEL_MAX
        {
            get { return fuel_max; }
            set
            {
                if (fuel_max != value)
                {
                    fuel_max = value;
                    NotifyPropertyChanged();
                }
            }

        }

        public int FUEL
        {
            get { return fuel; }
            set
            {
                if (fuel != value)
                {
                    fuel = value;
                    NotifyPropertyChanged();
                }
            }

        }

        public bool MOTOR_AN
        {
            get { return motor_an; }
            set
            {
                if (motor_an != value)
                {
                    motor_an = value;
                    NotifyPropertyChanged();
                }
            }

        }


        public bool ELEKTRIK_AN
        {
            get { return elektrik_an; }
            set
            {
                if (elektrik_an != value)
                {
                    elektrik_an = value;
                    NotifyPropertyChanged();
                }
            }

        }

     

        public string SHIFTER_TYP
        {
            get { return shifter_typ; }
            set
            {
                if (shifter_typ != value)
                {
                    shifter_typ = value;
                    NotifyPropertyChanged();
                }
            }

        }

        public int RPM_MAX
        {
            get { return rpm_max; }
            set
            {
                if (rpm_max != value)
                {
                    rpm_max = value;
                    NotifyPropertyChanged();
                }
            }

        }

        public int GAENGE_ZURUECK_MAX
        {
            get { return gaenge_zurueck_max; }
            set
            {
                if (gaenge_zurueck_max != value)
                {
                    gaenge_zurueck_max = value;
                    NotifyPropertyChanged();
                }
            }

        }

        public int GAENGE_VOR_MAX
        {
            get { return gaenge_vor_max; }
            set
            {
                if (gaenge_vor_max != value)
                {
                    gaenge_vor_max = value;
                    NotifyPropertyChanged();
                }
            }

        }

        public string LICENSEPLATE_COUNTRY_ID
        {
            get { return licenseplate_country_id; }
            set
            {
                if (licenseplate_country_id != value)
                {
                    licenseplate_country_id = value;
                    NotifyPropertyChanged();
                }
            }

        }

        public string LICENSEPLATE_COUNTRY
        {
            get { return licenseplate_country; }
            set
            {
                if (licenseplate_country != value)
                {
                    licenseplate_country = value;
                    NotifyPropertyChanged();
                }
            }

        }

        public string LICENSEPLATE
        {
            get { return licenseplate; }
            set
            {
                if (licenseplate != value)
                {
                    licenseplate = value;
                    NotifyPropertyChanged();
                }
            }

        }

        public string LKW_MODELL
        {
            get { return lkw_modell; }
            set
            {
                if (lkw_modell != value)
                {
                    lkw_modell = value;
                    NotifyPropertyChanged();
                }
            }

        }


        public string LKW_HERSTELLER_ID
        {
            get { return lkw_hersteller_id; }
            set
            {
                if (lkw_hersteller_id != value)
                {
                    lkw_hersteller_id = value;
                    NotifyPropertyChanged();
                }
            }

        }

        public string LKW_HERSTELLER
        {
            get { return lkw_hersteller; }
            set
            {
                if (lkw_hersteller != value)
                {
                    lkw_hersteller = value;
                    NotifyPropertyChanged();
                }
            }

        }

        public string CLIENT_VERSION
        {
            get { return client_version; }
            set
            {
                if (client_version != value)
                {
                    client_version = value;
                    NotifyPropertyChanged();
                }
            }

        }

        public string DLL_VERSION
        {
            get { return dll_version; }
            set
            {
                if (dll_version != value)
                {
                    dll_version = value;
                    NotifyPropertyChanged();
                }
            }

        }


        public string TELEMETRY_VERSION
        {
            get { return telemetry_version; }
            set
            {
                if (telemetry_version != value)
                {
                    telemetry_version = value;
                    NotifyPropertyChanged();
                }
            }

        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
