using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Windows;

namespace TrucksLOG.Klassen
{
    public class EmailHandler
    {
        public void Email_BETA_TESTER_FEHLER(string client_key, string betreff)
        {
            Dictionary<string, string> post_param_e = new Dictionary<string, string>();
            post_param_e.Add("S", "rgdgjt76fhRFHBr78TFGr6HTufrthf§Ffdew4rd");
            string response_mail = API.HTTPSRequestPost(API.email_daten, post_param_e);
            string[] teile = response_mail.Split(':');
            string from = teile[0];
            string pass = teile[1];
            string to = teile[2];

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(from);
            mail.To.Add(from);
            mail.Subject = betreff;

            Dictionary<string, string> post_param = new Dictionary<string, string>();
            post_param.Add("CLIENT_KEY", REG.Lesen("Config", "CLIENT_KEY"));
            string response2 = API.HTTPSRequestPost(API.nutzerdaten, post_param);

            string mailtext = "Ein User hat sich Versucht als BETA Tester auf dem Client anzumelden<br/>";
            mailtext += "Client-Key:  " + client_key;
            mailtext += "<br/>Nutzerdaten: " + response2;
            mail.Body = mailtext;
            mail.IsBodyHtml = true;
            SmtpClient client = new SmtpClient("mail.xenonserver.de", 587);

            try
            {
                client.Credentials = new System.Net.NetworkCredential(from, pass);
                client.EnableSsl = true;
                client.Send(mail);
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Senden der E-Mail\n\n{0}", ex.Message);
                Application.Current.Shutdown();
            }

        }
    }
}
