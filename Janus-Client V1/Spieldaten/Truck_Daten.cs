using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Janus_Client_V1.Spieldaten
{
    public class Truck_Daten : INotifyPropertyChanged
    {
        private string telemetry_version;
        private string dll_version;
        private string client_version;





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
