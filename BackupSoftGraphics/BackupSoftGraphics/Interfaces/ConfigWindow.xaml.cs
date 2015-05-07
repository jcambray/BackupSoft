#region Usings
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
using System.Configuration;
using clientbackup;
using System.IO;
using System.ComponentModel;
using BackupSoftGraphics.Database.Model;

#endregion


namespace BackupSoftGraphics
{
    /// <summary>
    /// Logique d'interaction pour ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        public  App Application {get;set;}
        private const int MAX_HOUR = 23;
        private const int MAX_MINUTE = 59;
        private const int MIN_VALUE = 0;
        public TreeviewViewModel TreeviewViewModel
        {
            get
            {
                return new TreeviewViewModel();
            }
        }

        public ConfigWindow()
        {

        }

        public ConfigWindow(App app)
        {
            InitializeComponent();
            
            Application = app;
            for(int i = 0; i <= 30;i++)
                cbPeriod.Items.Add(i);

            cbPeriod.SelectedIndex = 0;

            for (int i = 0; i <= 3; i++)
                cbKeepSave.Items.Add(i);
            cbKeepSave.SelectedIndex = 0;
            DataContext = this;
        }


        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            Application.Config.SaveConfiguration();
            Serialization.serializeToXML(GetCheckedNodes(), "folders.xml");
            SaveCheckedNodes(GetCheckedNodes());
            Close();
        }

        private void SaveCheckedNodes(List<string> list)
        {
            var backupFolders = list.Select(S => new BackupFolder {  Fullname = S}).ToList();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


      

        private List<String> GetCheckedNodes()
        {
            var checkedNodes = new List<String>();

            foreach (BackupFolder model in folderTreeView.ItemsSource)
            {
                ProcessNode(model, checkedNodes);
            }
            return checkedNodes;
        }
     
        private void ProcessNode(BackupFolder item,List<string> list)
        {
            foreach (var i in item.Children)
            {
                if (i.IsChecked == null)
                {
                    //if (!folders.Contains(i.Folder.FullName))
                    list.Add(i.Fullname);
                    ProcessNode(i, list);
                    continue;
                }
                
                if ((bool)i.IsChecked == true)
                {
                    //if (!folders.Contains(i.Folder.FullName))
                     list.Add(i.Fullname);
                    ProcessNode(i, list);
                }
            }
            
        }


        private void cbTreeViewItem_Click(object sender, RoutedEventArgs e)
        {
            var control = sender as CheckBox;
            var model = control.DataContext as BackupFolder;
            if (model.IsChecked == true)
                UpdateChildren(model, true);
            if (model.IsChecked == false)
                UpdateChildren(model, false);
        }

        void UpdateChildren(BackupFolder folder, bool? newValue)
        {
            foreach (var c in folder.Children)
            {
                c.IsChecked = newValue;
                UpdateChildren(c, newValue);
            }
        }

    }
}
