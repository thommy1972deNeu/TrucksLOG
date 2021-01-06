using System;
using System.Windows.Media;

namespace TrucksLOG.Klassen
{
    public class SoundPlayer
    {
       

        public static void Sound_Willkommen()
        {
            var player = new MediaPlayer();
            player.Open(new Uri(@"Resources/Willkommen.wav", UriKind.RelativeOrAbsolute));
            player.Volume = Convert.ToDouble(REG.Lesen("Config", "Volume"));
            player.Play();
        }
        public static void Sound_Tour_Gestartet()
        {
            MediaPlayer player = new MediaPlayer();
            player.Open(new Uri("Resources/Tour_Gestartet.wav"));
            player.Volume = Convert.ToDouble(REG.Lesen("Config", "Volume"));
            player.Play();
        }
        public static void Sound_Tour_Abgebrochen()
        {
            MediaPlayer player = new MediaPlayer();
            player.Open(new Uri("Resources/Tour_Abgebrochen.wav"));
            player.Volume = Convert.ToDouble(REG.Lesen("Config", "Volume"));
            player.Play();
        }
        public static void Sound_Tour_Beendet()
        {
            MediaPlayer player = new MediaPlayer();
            player.Open(new Uri("Resources/Tour_Beendet.wav"));
            player.Volume = Convert.ToDouble(REG.Lesen("Config", "Volume"));
            player.Play();
        }

        public static void Sound_Mautstation_Passiert()
        {
            MediaPlayer player = new MediaPlayer();
            player.Open(new Uri("Resources/Mautstation_Passiert.wav"));
            player.Volume = Convert.ToDouble(REG.Lesen("Config", "Volume"));
            player.Play();
        }

        public static void Sound_Strafe_Erhalten()
        {
            MediaPlayer player = new MediaPlayer();
            player.Open(new Uri("Resources/Strafe_Erhalten.wav"));
            player.Volume = Convert.ToDouble(REG.Lesen("Config", "Volume"));
            player.Play();
        }

        public static void Sound_Starting()
        {
            MediaPlayer player = new MediaPlayer();
            player.Open(new Uri(@"Resources/Willkommen.wav.wav"));
            player.Volume = Convert.ToDouble(REG.Lesen("Config", "Volume"));
            player.Play();
        }

    }
}
