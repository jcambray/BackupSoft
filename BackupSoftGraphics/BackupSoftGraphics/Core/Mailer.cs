using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;
using System.Text;
using System.Configuration;

namespace clientbackup
{
    class Mailer : IDisposable
    {
        private string expediteur;
        private string destinataires;
        private MailMessage mailMsg;
        private Save sauvegarde;
        private SmtpClient client;

        public Mailer(Save s)
        {
            this.client = new SmtpClient(ConfigurationManager.AppSettings["SMTP"], int.Parse(ConfigurationManager.AppSettings["port"]));
            this.client.DeliveryMethod = SmtpDeliveryMethod.Network;
            this.expediteur = ConfigurationManager.AppSettings["from"];
            this.destinataires = ConfigurationManager.AppSettings["to"];
            if(ConfigurationManager.AppSettings["SSL"] == "1")
            { this.client.EnableSsl = true; }
            this.client.UseDefaultCredentials = false;
            NetworkCredential login = new NetworkCredential(this.expediteur,ConfigurationManager.AppSettings["MDPfrom"]);
            this.client.Credentials = login;
            this.mailMsg = new MailMessage();
            this.mailMsg.From = new MailAddress(this.expediteur);
            this.mailMsg.To.Add(this.destinataires);
            this.sauvegarde = s;
        }

        //public void sendNotificationSauvegarde()
        //{
        //    try
        //    {
        //        string etatSauvegarde = "Terminée";
        //        if (sauvegarde.verifieSiTerminee() == '2')
        //        { etatSauvegarde = "Terminée"; }
        //        else
        //        { etatSauvegarde = "incomplète"; }
        //        this.mailMsg.Subject = DateTime.Now.ToShortDateString() + " Fin sauvegarde " + Environment.UserName;
        //        this.mailMsg.SubjectEncoding = System.Text.Encoding.UTF8;
        //        this.mailMsg.Body = DateTime.Now.ToShortDateString() + " à " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ": Fin de la sauvegarde" + Environment.NewLine
        //        + "Etat: " + etatSauvegarde + "." + Environment.NewLine
        //        + "Nombre de fichiers copiés: " + this.sauvegarde.getNbFichiersCopie() + Environment.NewLine
        //        + "Volume des données sauvegardées: " + this.sauvegarde.getVolumeFichiers().ToString() + " Mo." + Environment.NewLine
        //        + "Espace disponible sur l'emplacement de la sauvegarde: " + (int)sauvegarde.EspaceDispo() + @"/" + (int)sauvegarde.EspaceTotal() + " Go"
        //        + Environment.NewLine
        //        + Environment.NewLine
        //        + "Envoyé depuis AUTOMOTOR Backup";
        //        this.mailMsg.BodyEncoding = System.Text.Encoding.UTF8;
        //        this.client.Send(this.mailMsg);
        //    }
        //    catch(Exception e)
        //    { MessageBox.Show(e.Message); }
        //}

        public void sendNotificationDebut()
        {
            try
            {
                mailMsg.Subject = DateTime.Now.ToShortDateString() + " Debut sauvegarde " + Environment.UserName;
                mailMsg.SubjectEncoding = System.Text.Encoding.UTF8;
                mailMsg.Body = DateTime.Now.ToShortDateString() + " à " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ": lancement de la sauvegarde." + Environment.NewLine
                + Environment.NewLine
                + "Espace requis :" + (int)(sauvegarde.GetRequiredDiskSpace() / 1048576) + "Go."
                + Environment.NewLine
                + "Espace restant : " + (int)sauvegarde.EspaceDispo() + "Go."
                + Environment.NewLine
                + "Envoyé depuis AUTOMOTOR Backup";
                this.mailMsg.BodyEncoding = System.Text.Encoding.UTF8;
                this.client.Send(this.mailMsg);
            }
            catch(Exception ex)
            { MessageBox.Show(ex.Message); }
        }

        public void Dispose()
        {
            mailMsg.Dispose();
            client.Dispose();
        }
    }
}
