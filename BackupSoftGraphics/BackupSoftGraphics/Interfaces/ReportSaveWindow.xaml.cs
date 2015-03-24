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
using System.Windows.Shapes;

namespace BackupSoftGraphics
{
    /// <summary>
    /// Logique d'interaction pour ReportSaveWindow.xaml
    /// </summary>
    public partial class ReportSaveWindow : Window
    {
        private App application;
        public ReportSaveWindow(App app)
        {
            InitializeComponent();
            application = app;
        }
    }
}
