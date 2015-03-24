using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace clientbackup
{
   public class Configuration
    {
        public int nbjours { get; set; }
        public int heure { get; set; }
        public int minute { get; set; }
        public int period{ get; set; }
        public int nbSaves{ get; set; }
        public string path { get; set; }
        public char autoShutDown { get; set; }
        public DateTime nextSaveDate { get; set; }
        public String ExpAdress { get; set; }
        public String DestAdress { get; set; }
        public bool SSL { get; set; }
        public string PWDExp { get; set; }
        public string SMTP { get; set; }
        public int Port { get; set; }
        public string SearchRoot { get; set; }
        


        public Configuration()
        {
            this.period = Convert.ToInt32(ConfigurationManager.AppSettings["period"]);
            this.heure = Convert.ToInt32(ConfigurationManager.AppSettings["heure"]);
            this.minute = Convert.ToInt32(ConfigurationManager.AppSettings["minute"]);
            this.path = ConfigurationManager.AppSettings["path"];
            this.nbSaves = Convert.ToInt32(ConfigurationManager.AppSettings["nbSaves"]);
            this.nextSaveDate = Convert.ToDateTime(ConfigurationManager.AppSettings["nextSave"]);
            this.autoShutDown = Convert.ToChar(ConfigurationManager.AppSettings["autoShutDown"]);
            ExpAdress = ConfigurationManager.AppSettings["from"];
            DestAdress = ConfigurationManager.AppSettings["to"];
            SSL = bool.Parse(ConfigurationManager.AppSettings["SSL"]);
            PWDExp = ConfigurationManager.AppSettings["MDPfrom"];
            SMTP = ConfigurationManager.AppSettings["SMTP"];
            Port = int.Parse(ConfigurationManager.AppSettings["port"]);
            SearchRoot = ConfigurationManager.AppSettings["racineRecherche"];
        }

       public void SaveConfiguration()
        {
            var manager = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            manager.AppSettings.Settings["period"].Value = period.ToString();
            manager.AppSettings.Settings["heure"].Value = heure.ToString();
            manager.AppSettings.Settings["minute"].Value = minute.ToString();
            manager.AppSettings.Settings["path"].Value = path;
            manager.AppSettings.Settings["nbSaves"].Value = nbSaves.ToString();
            manager.AppSettings.Settings["autoShutDown"].Value = autoShutDown.ToString();
            manager.AppSettings.Settings["from"].Value = ExpAdress;
            manager.AppSettings.Settings["to"].Value = DestAdress;
            manager.AppSettings.Settings["SSL"].Value = SSL.ToString();
            manager.AppSettings.Settings["MDPfrom"].Value = PWDExp;
            manager.AppSettings.Settings["SMTP"].Value = SMTP;
            manager.AppSettings.Settings["port"].Value = Port.ToString();
            manager.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        public Configuration(int nbj,int h, int min, int per, int nbSav, string p)
        {
            nbjours = nbj;
            heure = h;
            minute = min;
            period = per;
            nbSaves = nbSav;
            path = p;
        }                    
    }
}
