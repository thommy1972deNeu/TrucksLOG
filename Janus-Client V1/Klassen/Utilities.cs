using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace Janus_Client_V1.Klassen
{
    public class Utilities
    {
        private static bool _cachedRunningFlag;

        public static bool IsGameRunning
        {
            get
            {
                if ((DateTime.Now - new DateTime(Interlocked.Read(ref _lastCheckTime))) > TimeSpan.FromSeconds(3.0))
                {
                    Interlocked.Exchange(ref _lastCheckTime, DateTime.Now.Ticks);
                    Process[] processes = Process.GetProcesses();
                    int index = 0;
                    while (true)
                    {
                        if (index < processes.Length)
                        {
                            Process process = processes[index];
                            try
                            {
                                if (process.MainWindowTitle.StartsWith("Euro Truck Simulator 2") || (process.ProcessName == "eurotrucks2") || (process.ProcessName == "amtrucks"))
                                {
                                    _cachedRunningFlag = true;
                                    if (process.ProcessName == "eurotrucks2")
                                    {
                                        LastRunningGameName = "ETS2";
                                    }
                                    else if (process.ProcessName == "amtrucks")
                                    {
                                        LastRunningGameName = "ATS";
                                    }
                                    return _cachedRunningFlag;
                                }


                            }
                            catch
                            {
                            }
                            index++;
                            continue;
                        }
                        else
                        {
                            _cachedRunningFlag = false;
                        }
                        break;
                    }
                }
                return _cachedRunningFlag;
            }
        }

        public static void MAUT_BEZAHLEN()
        {

            var request = (HttpWebRequest)WebRequest.Create("https://www.zwpc.de/api/Tollgate_Payment.php");

            var postData = "accesstoken=" + REG.Lesen("Config", "AccessToken");
            postData += "&payment=" + "421423";
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
        }

        public static ref long _lastCheckTime => throw new NotImplementedException();

        public static string LastRunningGameName { get; private set; }
    }
}
