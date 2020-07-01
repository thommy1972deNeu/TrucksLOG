using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus_Client_V1.Klassen
{
    public class SoundPlayer
    {
        public static void Sound_Willkommen()
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.Willkommen_wav);
            player.Play();
        }
        public static void Sound_Tour_Gestartet()
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.Tour_Gestartet);
            player.Play();
        }
        public static void Sound_Tour_Abgebrochen()
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.Tour_Abgebrochen);
            player.Play();
        }
        public static void Sound_Tour_Beendet()
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.Tour_Beendet);
            player.Play();
        }

        public static void Sound_Mautstation_Passiert()
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.Mautstation_Passiert);
            player.Play();
        }
        public static void Sound_Strafe_Erhalten()
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.Strafe_Erhalten);
            player.Play();
        }
    }
}
