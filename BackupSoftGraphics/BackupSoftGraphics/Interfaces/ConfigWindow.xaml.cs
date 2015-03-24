﻿#region Usings
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
    public partial class ConfigWindow : Window , INotifyPropertyChanged
    {
        public  App Application {get;set;}
        private const int MAX_HOUR = 23;
        private const int MAX_MINUTE = 59;
        private const int MIN_VALUE = 0;
        private List<string> folders;
        private List<string> files;
        public event PropertyChangedEventHandler PropertyChanged;
        private BindingList<TreeViewItemModel> treeViewItemModelList;
        public BindingList<TreeViewItemModel> TreeViewItemModelList
        {
            get { return treeViewItemModelList; }
            set
            {
                treeViewItemModelList = value;
                RaisePropertyChangedEvent("FolderSaveItems");
            }
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

            treeViewItemModelList = new BindingList<TreeViewItemModel>();
            folders = Serialization.deserializeXML("folders.xml");
            files = Serialization.deserializeXML("files.xml");

            new DirectoryInfo(System.IO.Path.Combine(Application.Config.SearchRoot, Environment.UserName))
                .GetDirectories()
                .ToList()
                .ForEach(I => TreeViewItemModelList.Add(new TreeViewItemModel(I.FullName,null)));
            DataContext = this;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            Application.Config.SaveConfiguration();
            //Serialization.serializeToXML(GetCheckedNodes(), "folders.xml");
            SaveCheckedNodes(GetCheckedNodes());
            Close();
        }

        private void SaveCheckedNodes(List<string> list)
        {
            var backupFolders = list.Select(S => new BackupFolder {  Fullname = S}).ToList();
            Application.DBContext.BackupFolders.AddRange(backupFolders);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

      

        private void folderTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                var item = e.NewValue as TreeViewItemModel;
                new DirectoryInfo(item.Folder.FullName)
                    .GetDirectories()
                    .ToList()
                    .ForEach(I =>
                    {
                        item.AddItem(new TreeViewItemModel(I.FullName,item));
                    });
                RaisePropertyChangedEvent("TreeViewItemModelList");               
            }
            catch (UnauthorizedAccessException) { }
        }

        private void RaisePropertyChangedEvent( string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void TreeViewItem_Click(object sender, RoutedEventArgs e)
        {
            var model = ((TreeViewItem)sender).DataContext as TreeViewItemModel;
            model.Children.ToList().ForEach(I => I.IsChecked = model.IsChecked);
            if (model.Parent != null)
                model.UpdateParentIsChecked();
        }


        private List<String> GetCheckedNodes()
        {
            var checkedNodes = new List<String>();
          
            foreach (TreeViewItemModel model in folderTreeView.ItemsSource)
            {
                ProcessNode(model,checkedNodes);
            }
            return checkedNodes;
        }
     
        private void ProcessNode(TreeViewItemModel item,List<string> list)
        {
            foreach (var i in item.Children)
            {
                if (i.IsChecked == null)
                {
                    if (!folders.Contains(i.Folder.FullName))
                        list.Add(i.Folder.FullName);
                    ProcessNode(i,list);
                    continue;
                }
                
                if ((bool)i.IsChecked)
                {
                    if (!folders.Contains(i.Folder.FullName))
                        list.Add(i.Folder.FullName);
                    ProcessNode(i,list);
                }
            }
            
        }

    }
}