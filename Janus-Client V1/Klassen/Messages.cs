using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Janus_Client_V1.Klassen
{
    public class Messages
    {
        public async void Schreiben(string titel, string text)
        {
            var metroWindow = (Application.Current.MainWindow as MetroWindow);
            await metroWindow.ShowMessageAsync(titel, text);
        }
    }
}
