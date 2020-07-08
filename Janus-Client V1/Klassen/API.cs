using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Windows;

namespace Janus_Client_V1.Klassen
{
    public class API
    {
        public static string server = "https://projekt-janus.de/api/";
        public static string job_delivered = server + "job_delivered.php";
        public static string job_started = server + "start_tour.php";
        public static string job_cancel = "https://projekt-janus.de/api/job_cancel.php";
        public static string job_update = server + "job_update.php";
        public static string job_finish = server + "job_finish.php";
        public static string strafe = server + "strafe.php";
        public static string tollgate = server + "tollgate.php";
        public static string tanken = server + "tanken.php";
        public static string transport = server + "transport.php";
        public static string link_click = server + "link_click.php";
        public static string versionFile_Link = "https://projekt-janus.de/client_updates/version.txt";

        public static string HTTPSRequestPost(string url, Dictionary<string, string> postParameters)
        {
            string s = "";
            foreach (string str2 in postParameters.Keys)
            {
                string[] textArray1 = new string[] { s, HttpUtility.UrlEncode(str2), "=", HttpUtility.UrlEncode(postParameters[str2]), "&" };
                s = string.Concat(textArray1);
            }

            HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            myHttpWebRequest.Method = "POST";

            byte[] data = Encoding.ASCII.GetBytes(s);

            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            myHttpWebRequest.ContentLength = data.Length;

            Stream requestStream = myHttpWebRequest.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();

            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            Stream responseStream = myHttpWebResponse.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(responseStream, Encoding.Default);
            string pageContent = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            responseStream.Close();
            myHttpWebResponse.Close();
            return pageContent;
        }
           


    }
}
