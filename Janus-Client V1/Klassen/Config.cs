using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus_Client_V1.Klassen
{
    public class Config
    {
        // ALLGEMEIN
        public static string dll_Version = "10";

        // LOGGING
        public static string datum = DateTime.Now.ToString("dd_MMM_yyyy_HH_mm");
        public static string LogRoot = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Projekt-JANUS";
        public static string ClientLogFileName = @"\Client_Log_ " + datum.ToString() + ".txt";
        public static string SystemLogFileName = @"\System_Log_.txt";

    }
}
