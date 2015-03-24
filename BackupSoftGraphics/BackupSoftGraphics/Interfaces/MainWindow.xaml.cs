#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using clientbackup;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Interop;
#endregion

namespace BackupSoftGraphics
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        App application;
        public MainWindow()
        {
            InitializeComponent();
            application = (App)Application.Current;
            btnSave.Click += btnSave_Click;
            lbUsername.Content = Environment.UserName;
            Minimize();
           
            if (application.Sauvegarde.nextSave.Year == 2000)
            {
                lbDateProchaineSauvegarde1.Foreground = Brushes.Red;
                lbDateProchaineSauvegarde1.Content = "non planifiée";
            }
            else
            {
                lbDateProchaineSauvegarde1.Content = String.Format("Date de la prochaine sauvegarde: {0}",application.Sauvegarde.nextSave.ToShortDateString());
            }
        }

        void btnSave_Click(object sender, RoutedEventArgs e)
        {
            application.MainWindowBtnSaveClick();
        }

         void menuParamètres_Click(object sender, RoutedEventArgs e)
        {
            application.OpenConfigurationWindow();
        }

         void menuQuitter_Click(object sender, RoutedEventArgs e)
        {
            application.MainWindowBtnQuitterClick();
        }

        public void Minimize()
        {
            Visibility = System.Windows.Visibility.Collapsed;
            this.ShowInTaskbar = true;
        }

        public void maximize()
        {
            Visibility =  System.Windows.Visibility.Visible;
            ShowInTaskbar = true;
            Height = 522;
            Width = 339;
            WindowState = System.Windows.WindowState.Normal;
        }


        private void réduireToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Minimize();
        }



        //lorsque l'on clique sur la bulle d'information
        private void notifyIcon_Click(object sender, EventArgs e)
        {
            maximize();
        }

        [DllImport("user32.dll", EntryPoint = "ShutdownBlockReasonCreate")]
        public extern static void ShutdownBlockReasonCreate(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] string pwszReason);

        [DllImport("user32.dll", EntryPoint = "ShutdownBlockReasonDestroy")]
        public extern static bool shutdownBlockReasonDestroy(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] string pwszReason);

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_QUERYENDSESSION = 0x0011;
            const int WM_ENDSESSION = 0x0016;
            if ((msg == WM_QUERYENDSESSION || msg == WM_ENDSESSION) && ((DateTime.Now.Day * DateTime.Now.Month * DateTime.Now.Year) != (application.Sauvegarde.nextSave.Day * application.Sauvegarde.nextSave.Month * application.Sauvegarde.nextSave.Year)))
            {
                //Avoiding a close event
                handled = false;
            }
            else
                handled = true;
            return IntPtr.Zero;
        }
      
        //instancie une nouvelle fenetre permettant le lancement de la sauvegarde et l'affiche à l'ecran
        public void initSaveViewer()
        {
            var saveView = new SaveViewWindow(application);
            saveView.Show();
        }

        //lorsque l'on clique sur "effacer"
        private void effacerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var r = MessageBox.Show("Effacer le log", "êtes-vous sûr?", MessageBoxButton.YesNo);
            if (r == MessageBoxResult.Yes)
            {
                Log.effacer();
            }
        }

        //lorsque l'on clique sur "consulter"
        private void consulterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log.open();
        }

        //lorsque l'on clique sur "ouvrir le repertoire..."
        private void btnCible_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(application.Config.path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        //affiche comme date de pricaine sauvegarde la date entrée en paramètre
        public void setLbDateProchaineSauvegarde(DateTime d)
        {
            this.lbDateProchaineSauvegarde1.Content = application.Sauvegarde.nextSave.ToShortDateString() + " à " + application.Sauvegarde.nextSave.ToShortTimeString();
        }


        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            //source.AddHook(new HwndSourceHook(WndProc));
        }

        private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //string mdp = Serialization.deserializeMDPAdmin();
            //if (mdp == null)
            //{
            //    MessageBox.Show("mot de passe administrateur introuvable.Veuillez en enregistrer un");
            //    return;
            //}

            //var MDPAC = new MDPControlWindow(mdp);
            

            //if ((bool)MDPAC.ShowDialog())
            //{
            //    Log.Write("Fermeture de l'application par l'administrateur");
            //    return;
            //}

            //e.Cancel = true;

        }

        private void menuReduire_Click(object sender, RoutedEventArgs e)
        {
            Minimize();
        }

        private void menuMDPAdmin_Click(object sender, RoutedEventArgs e)
        {
            application.MainwindowBtnAdminPasswordClick();
        }

        private void menuMDPUser_Click(object sender, RoutedEventArgs e)
        {
            application.MainWindowBtnUserPasswordClick();
        }

        private void menuConfigWindow_Click(object sender, RoutedEventArgs e)
        {
            application.MenuParametresClick();
        }

        private void mainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        
    }


}
