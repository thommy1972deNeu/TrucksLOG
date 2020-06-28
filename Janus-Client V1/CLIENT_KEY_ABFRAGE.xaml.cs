using System.Diagnostics;
using System.Windows;
using Janus_Client_V1.Klassen;

namespace Janus_Client_V1
{
    /// <summary>
    /// Interaktionslogik für CLIENT_KEY_ABFRAGE.xaml
    /// </summary>
    public partial class CLIENT_KEY_ABFRAGE : Window
    {
        MSG msg = new MSG();
        public CLIENT_KEY_ABFRAGE()
        {
            InitializeComponent();
        }

        private void senden_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(client_key.Text))
                return;
            REG.Schreiben("Config", "CLIENT_KEY", client_key.Text);
            msg.Schreiben("Eintrag", "Der Schlüssel wurde eingetragen. Der Client muss jetzt einmal neu gestartet werden um die Änderungen zu Speichern");
            Application.Current.Shutdown();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(client_key.Text))
                return;
            e.Cancel = false;
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            if (string.IsNullOrEmpty(client_key.Text))
                return;
        }

        private void abbruch_Click(object sender, RoutedEventArgs e)
        {
            
            Application.Current.Shutdown();
        }

        private void Label_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Process.Start("https://projekt-janus.de");
        }
    }
}
