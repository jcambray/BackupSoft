using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackupSoftGraphics.Database.Model;
using System.IO;

namespace BackupSoftGraphics
{
    public class TreeviewViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private BindingList<BackupFolder> backupFoldersList;
        public BindingList<BackupFolder> BackupFoldersList
        {
            get { return backupFoldersList; }
            set
            {
                backupFoldersList = value;
                RaisePropertyChangedEvent("BackupFoldersList");
            }
        }


        private void RaisePropertyChangedEvent(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        public TreeviewViewModel()
        {

        }


        private void RetrieveDirectories(BackupFolder root, List<String> backupFoldersloadedFromDatabase)
        {
            try
            {
                var tempList = new DirectoryInfo(root.Fullname).GetDirectories();
                foreach (DirectoryInfo d in tempList)
                {
                    BackupFolder b;
                    if (backupFoldersloadedFromDatabase.Contains(d.FullName))
                        b = new BackupFolder { Fullname = d.FullName, IsChecked = true };
                    else
                        b = new BackupFolder { Fullname = d.FullName, IsChecked = false };
                    root.Children.Add(b);
                    RetrieveDirectories(b, backupFoldersloadedFromDatabase);
                }

            }
            catch (Exception) { }
        }

        public BindingList<BackupFolder> FillFolderTreeviewContentList()
        {
            var root = new BackupFolder { Fullname = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile, Environment.SpecialFolderOption.DoNotVerify) };
            var backupFoldersloadedFromDatabase = clientbackup.Serialization.deserializeXML("folders.xml");
            RetrieveDirectories(root, backupFoldersloadedFromDatabase);
            return new BindingList<BackupFolder>(root.Children);
        }



    }
}
