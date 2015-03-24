using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using System.ComponentModel;

namespace clientbackup
{
    public class FolderSaveItem : ISaveItem, INotifyPropertyChanged
    {
        string name;
        DirectoryInfo folder;
        DateTime dateofCopy;
        List<ISaveItem> items;
        public List<ISaveItem> Items {
            get {
                return new List<ISaveItem>(items);
            }
        }

        public DirectoryInfo Folder
        {
            get
            {
                return folder;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public long Size
        {
            get
            {
                long cumul = 0;
                items.ForEach(I => cumul += I.Size);
                return cumul;
            }
        }
        public bool IsNotEmpty
        {
            get
            {
                return !(items.Count == 0);
            }
        }

        public DateTime DateOfCopy
        {
            get
            {
                return dateofCopy;
            }
            set
            {
                dateofCopy = value;
            }
        }

        public FolderSaveItem(string fullName)
        {
            folder = new DirectoryInfo(fullName);
            name = folder.Name;
            dateofCopy = DateTime.MinValue;
            items = new List<ISaveItem>();
        }

        public void Copy(string root)
        {
            var path = root + ReformatedPath();
            Directory.CreateDirectory(path);
            items.ForEach(I => I.Copy(path));
            dateofCopy = DateTime.Now;
        }

        public void AddItem(ISaveItem item)
        {
            items.Add(item);
            RaisePropertyChange("Items");
        }

        public string GetParent()
        {
            return folder.Parent.FullName;
        }

        public String GetParentName()
        {
            return folder.Parent.Name;
        }

        public bool IsParentOf(FolderSaveItem item)
        {
            return item.Folder.Parent.FullName == Folder.FullName;
        }

        private String ReformatedPath()
        {
            return Folder.FullName.Replace(@"C:", "");
        }

        private void RaisePropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
