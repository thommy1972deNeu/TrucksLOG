using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrucksLOG.Klassen
{
    public class Config
    {
        // ALLGEMEIN
        public static string dll_Version = "10";
        static string Datum = DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
        // LOGGING
        public static string LogRoot = System.IO.Directory.GetCurrentDirectory() + @"\LOGS";
        public static string ClientLogFileName = @"\Client_Log_" + Datum + ".txt";

    }
}
