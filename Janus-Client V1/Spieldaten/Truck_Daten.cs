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
        private int gear;
        private int speed;
        private int tempomat;
        private int rpm;
        private bool scheibenwischer;

        // LICHTER
        private bool beam_low;
        private bool beam_high;
        private bool brake_light;
        private bool reverse_light;
        private bool blinker_links;
        private bool blinker_rechts;
        private bool parking_light;

        // FUEL / ADBLUE
        private int fuel;
        private int fuel_max;

        // Warnings
        private bool fuel_warning;
        private bool air_warning;
        private bool oil_warning;
        private bool water_warning;
        private bool battery_warning;

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
        private float gewicht;
        private string ladung_name;
        private string ladung_id;
        private int frachtschaden;



        // -----------------------------------------------------------------------------------

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
        public int FRACHTSCHADEN
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


        public float GEWICHT
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

        public bool BRAKE_LIGHT
        {
            get { return brake_light; }
            set
            {
                if (brake_light != value)
                {
                    brake_light = value;
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

        public bool BATTERY_WARNING
        {
            get { return battery_warning; }
            set
            {
                if (battery_warning != value)
                {
                    battery_warning = value;
                    NotifyPropertyChanged();
                }
            }

        }


        public bool WATER_WARNING
        {
            get { return water_warning; }
            set
            {
                if (water_warning != value)
                {
                    water_warning = value;
                    NotifyPropertyChanged();
                }
            }

        }


        public bool OIL_WARNING
        {
            get { return oil_warning; }
            set
            {
                if (oil_warning != value)
                {
                    oil_warning = value;
                    NotifyPropertyChanged();
                }
            }

        }


        public bool AIR_WARNING
        {
            get { return air_warning; }
            set
            {
                if (air_warning != value)
                {
                    air_warning = value;
                    NotifyPropertyChanged();
                }
            }

        }

        public bool FUEL_WARNING
        {
            get { return fuel_warning; }
            set
            {
                if (fuel_warning != value)
                {
                    fuel_warning = value;
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
