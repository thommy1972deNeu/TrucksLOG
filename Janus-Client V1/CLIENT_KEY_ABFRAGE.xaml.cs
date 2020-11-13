using System;
using System.Diagnostics;
using System.Windows;
using TrucksLOG.Klassen;

namespace Janus_Client_V1
{
    /// <summary>
    /// Interaktionslogik für CLIENT_KEY_ABFRAGE.xaml
    /// </summary>
    public partial class CLIENT_KEY_ABFRAGE : Window
    {
        public CLIENT_KEY_ABFRAGE()
        {
            InitializeComponent();
        }

        private void senden_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(client_key.Text))
                return;
            try
            {
                if (client_key.Text.Length <= 90)
                {
                    MessageBox.Show("Der Client-Key ist zu kurz !", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    Logging.WriteClientLog("Fehler beim Eintragen des Client-Key in die Registry: Client-Key war zu kurz!");
                } else if (client_key.Text.Length >= 105)
                {
                    MessageBox.Show("Der Client-Key ist zu lang !", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    Logging.WriteClientLog("Fehler beim Eintragen des Client-Key in die Registry: Client-Key war zu lang!");
                } else 
                {
                    REG.Schreiben("Config", "CLIENT_KEY", client_key.Text);
                    MessageBox.Show("Bitte starte den Client neu um die Einstellungen zu Speichern !", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    Application.Current.Shutdown();
                }

            } catch (Exception ex)
            {
                MessageBox.Show("Es gab einen Fehler beim Schreiben des Client-Keys" + ex.Message + ex.StackTrace, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                Logging.WriteClientLog("Fehler beim Eintragen des Client-Key in die Registry: " + ex.Message);
            }

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
