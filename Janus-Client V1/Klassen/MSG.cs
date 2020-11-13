using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;

namespace TrucksLOG.Klassen
{
    public class MSG
    {
        public async void Schreiben(string titel, string text)
        {
            var metroWindow = (Application.Current.MainWindow as MetroWindow);
            await metroWindow.ShowMessageAsync(titel, text);
        }

    }
}
