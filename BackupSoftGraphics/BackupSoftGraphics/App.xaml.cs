using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using clientbackup;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Media;
using System.Threading;
using System.Drawing;
using Configuration = clientbackup.Configuration;
using BackupSoftGraphics.Database;
using BackupSoftGraphics.Database.Model;

namespace BackupSoftGraphics
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public bool launch = true;
        private bool isAutoLogonEnabled;
        private Save sauvegarde;
        public Configuration Config {get;set;}
        public Save Sauvegarde
        {
            get
            {
                if (sauvegarde == null)
                {
                    sauvegarde = new Save();
                    return sauvegarde;
                }
                else
                    return sauvegarde;
            }
        }
        DispatcherTimer timer;
        System.Windows.Forms.NotifyIcon notifyIcon;
        public BackupSoftDBContext DBContext { get; set; }

        public App()
        {
            Startup += App_Startup;
            ShutdownMode = System.Windows.ShutdownMode.OnMainWindowClose;
        }

        void App_Startup(object sender, StartupEventArgs e)
        {
            SetConnectionString();
            DBContext = new BackupSoftDBContext();
            //System.Diagnostics.Debug.WriteLine(DBContext.Database.Connection.ConnectionString);
            //var test = new BackupFolder { Fullname = @"C:\test", IsChecked = true };
            //DBContext.BackupFolders.Add(test);
            DBContext.SaveChanges();
            Config = Sauvegarde.c;
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            ConfigureNotifyIcon();
            Configuretimer();
            Directory.CreateDirectory("Data");
            Log.Write("Lancement de l'application");

            //ConnectedToAMFNetwork();


            this.isAutoLogonEnabled = (bool)Serialization.deserialize();
            if (this.isAutoLogonEnabled == true)
            {
                Thread.Sleep(60000);
                RegistryModifier.disableAutoLogon();
                this.isAutoLogonEnabled = false;
                Serialization.serialize(this.isAutoLogonEnabled);
                //initSaveViewer();
            }
            
        }

        private void SetConnectionString()
        {
            var DatabaseFileName = "BackupSoftDB.mdf";
            var connectionString = Tools.ConnectionStringBuilder.GetRelativeConnectionString("(LocalDB)\\v11.0"
                , "\"" + System.AppDomain.CurrentDomain.BaseDirectory + DatabaseFileName + "\""
                , "True", "30");

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.ConnectionStrings.ConnectionStrings["BackupSoftDB"].ConnectionString = connectionString;
            config.Save(ConfigurationSaveMode.Modified, true);
            ConfigurationManager.RefreshSection("connectionStrings");
        }

        private void Configuretimer()
        {
            timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(0,0,1);
            timer.IsEnabled = true;
            timer.Start();
        }

        private void ConfigureNotifyIcon()
        {
            
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Text = "AUTOMOTOR Backup";
            notifyIcon.Visible = true;
            
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (Sauvegarde.checkSaveConditions())
            {
                timer.Stop();
                var rsf = new ReportSaveWindow(this);
                //DialogResult = rsf.ShowDialog();
                //if (DialogResult == System.Windows.Forms.DialogResult.OK)
                //{
                //    Sauvegarde.SetNextSave(rsf.getNextSave());
                //    window.lbDateProchaineSauvegarde1.Content = Sauvegarde.nextSave.ToShortDateString() + " à " + conf.heure + "h" + conf.minute;
                //    timer.Start();
                //}
                //else
                //{
                //    RegistryModifier.EnableAutoLogon(ConfigurationManager.AppSettings["password"]);
                //    this.isAutoLogonEnabled = true;
                //    Serialization.serialize(this.isAutoLogonEnabled);
                //    Save.restartComputer();
                //}
            }

            if (Sauvegarde.RemainingTime.Days == 0 && Sauvegarde.RemainingTime.Hours < 15)
            {
               // BackupSoftGraphics.MainWindow.ShutdownBlockReasonCreate(new WindowInteropHelper(MainWindow).Handle, "Une sauvegarde automatique va être éffectuée à " + Sauvegarde.nextSave.ToShortTimeString() + "." + Environment.NewLine + " Veuillez ne pas éteindre l'ordinateur");
            }
            else
            {
               // BackupSoftGraphics.MainWindow.shutdownBlockReasonDestroy(new WindowInteropHelper(MainWindow).Handle, "Arrêt autorisé");
            }
        }

      

         public void afficheNotification(string message, int duree)
        {
            notifyIcon.Visible = true;
            notifyIcon.Text = message;
            notifyIcon.BalloonTipTitle = message;
            notifyIcon.ShowBalloonTip(duree);
        }

         public void afficheAlerte()
         {

             if (Sauvegarde.RemainingTime.TotalHours < 6 && Sauvegarde.RemainingTime.TotalHours > 0 && (DateTime.Now.Minute == 0 && DateTime.Now.Second == 0))
             {
                 this.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Warning;
                 this.afficheNotification("jour de sauvegarde ,veuillez ne pas éteindre l'ordinateur", 10000);
             }
             else
             {
                 this.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
             }
         }

         public void notifyIcconAlerte(int heure, int minute, string message, int duree)
         {
             if (DateTime.Now.Day == Sauvegarde.nextSave.Day && DateTime.Now.Hour == heure && DateTime.Now.Minute == minute)
             {
                 this.afficheNotification(message, duree);
             }
         }

         public void ShowReportSaveWindow()
         {

         }

        public void OpenConfigurationWindow()
         {
             var mdp = Serialization.deserializeMDPAdmin();
             if (mdp == null)
             {
                 MessageBox.Show("mot de passe administrateur introuvable.Veuillez en enregistrer un");
             }
             else
             {
                 //var mdpControlWindow = new MDPControlWindow(mdp);
                 //if ((bool)mdpControlWindow.ShowDialog())
                 //{
                 //    var configurationWindow = new ConfigWindow();
                 //    configurationWindow.Show();
                 //}
             }

         }

        public void MainWindowBtnQuitterClick()
        {
          if(AdminAuthentification())
                Shutdown();
        }

        public void MainwindowBtnAdminPasswordClick()
        {
            if (AdminAuthentification())
            {
                var changePasswordWindow = new SavePasswordWindow(EPasswordType.Administrateur);
                changePasswordWindow.Show();
            }
        }

        public void MainWindowBtnUserPasswordClick()
        {
            if (AdminAuthentification())
            {
                var changePasswordWindow = new SavePasswordWindow(EPasswordType.Utilisateur);
                changePasswordWindow.Show();
            }
        }


        internal void MainWindowBtnSaveClick()
        {
            if (Sauvegarde.AsyncWorker != null)
                return;
            timer.Stop();
            //Sauvegarde.AsyncWorker.RunWorkerAsync();
            Sauvegarde.StartAsyncWork();
        }

        private void Application_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            if ((DateTime.Now.Day * DateTime.Now.Month * DateTime.Now.Year) != (Sauvegarde.nextSave.Day * Sauvegarde.nextSave.Month * Sauvegarde.nextSave.Year))
                e.Cancel = true;
            else
                e.Cancel = false;
        }

        private bool AdminAuthentification()
        {
            var MDPController = new MDPControlWindow();
            var result = (bool)MDPController.ShowDialog();
            if (result)
                return true;
            else
            {
                MessageBox.Show("Mot de passe incorrect", "BackupSoft", MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;
            }
        }

        public void MenuParametresClick()
        {
            if(AdminAuthentification())
            {
                var configWindow = new ConfigWindow(this);
                configWindow.ShowDialog();
            }
        }
    }
}
