using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace TrucksLOG.Klassen
{
    public class API
    {
        public static string server = "https://truckslog.org/";
        public static string job_delivered = server + "job_delivered.php";
        public static string job_started = server + "start_tour.php";
        public static string job_cancel = "https://truckslog.org/job_cancel.php";
        public static string job_update = server + "job_update.php";
        public static string job_finish = server + "job_finish.php";
        public static string strafe = server + "strafe.php";
        public static string tollgate = server + "tollgate.php";
        public static string tanken = server + "tanken.php";
        public static string transport = server + "transport.php";
        public static string link_click = server + "link_click.php";
        public static string patreon_state = server + "patreon.php";
        public static string beta_tester = server + "beta.php";
        public static string updates = server + "updates.php";
        public static string nutzerdaten = server + "nutzerdaten.php";
        public static string email_daten = server + "email_daten.php";
        public static string key_check = server + "key_check.php";
        public static string c_online = server + "c_online.php";
        public static string tmp_versionen = server + "tmp_version.php";
        public static string user_zu_schnell = server + "zu_schnell.php";
        public static string SpeditionsCheck = server + "speditions_check.php";
        public static string delete_tour = server + "del_tour.php";
        public static string punktekonto = server + "punktekonto.php";
        public static string bann_check = server + "bann_check.php";
        public static string useronline_url = server + "client_useronline.php";
        public static string fahreronline_url = server + "fahrer_online.php";
        public static string onlinecheck = server + "onlinecheck.php";
        public static string client_version = server + "client_version.php";
        public static string dlc_update_ets = server + "DLC_Update_ETS.php";
        public static string dlc_update_ats = server + "DLC_Update_ATS.php";
        public static string updatetext_uri = "http://client.truckslog.org/changelog.html";
        

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
