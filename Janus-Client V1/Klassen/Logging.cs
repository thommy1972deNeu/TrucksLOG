using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace TrucksLOG.Klassen
{
    public class Logging
    {
        public static void Make_Log_File()
        {
            if (!Directory.Exists(Config.LogRoot))
                 Directory.CreateDirectory(Config.LogRoot);

            if (!File.Exists(Config.LogRoot + Config.ClientLogFileName))
            {
                try
                {
                    File.Create(Config.LogRoot + Config.ClientLogFileName);
                    File.WriteAllText(Config.LogRoot + Config.ClientLogFileName, string.Empty);
                } catch { }
            }
            else
            {
                try
                {
                    File.WriteAllText(Config.LogRoot + Config.ClientLogFileName, "<-------------------------   NEW START   ----------------------->" + Environment.NewLine);
                } catch { }
            }

        }

        public static void WriteClientLog(string text, [CallerLineNumber] int linenumber = 0, [CallerFilePath] string file = null)
        {
            try
            {
                File.AppendAllText(Config.LogRoot + Config.ClientLogFileName, "<" + DateTime.Now + "> " + text + ", Line Number: " + linenumber + ", File: " + file + Environment.NewLine);
            } catch { }
        }
    }
}
