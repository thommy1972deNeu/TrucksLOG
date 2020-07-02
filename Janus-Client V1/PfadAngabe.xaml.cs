using Janus_Client_V1.Klassen;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace Janus_Client_V1
{
    /// <summary>
    /// Interaktionslogik für PfadAngabe.xaml
    /// </summary>
    public partial class PfadAngabe : Window
    {
        MSG msg = new MSG();

        public PfadAngabe()
        {
            InitializeComponent();

         
        }

        private void tmp_suchen_btn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open_tmp = new OpenFileDialog();
            open_tmp.ShowDialog();
        }

        private void ets_suchen_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ats_suchen_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (pfad_ets.Text == "")
            {
                MessageBox.Show("Der Pfad zu Eurotruck Simulator 2 wurde nicht angegeben!", "Fehler", MessageBoxButton.OK, (MessageBoxImage)MessageBoxIcon.Error);
                e.Cancel = true;
            }
        }

        private void abbrechen_click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
