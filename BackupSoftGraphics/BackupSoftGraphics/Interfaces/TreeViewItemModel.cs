using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;
using System.Collections.ObjectModel;
using clientbackup;

namespace BackupSoftGraphics
{
    public class TreeViewItemModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public DirectoryInfo Folder { get; set; }
        private ObservableCollection<TreeViewItemModel> children;
        public ObservableCollection<TreeViewItemModel> Children
        {
            get { return children; }
            set { children = value; }
        }

        public TreeViewItemModel Parent { get; set; }
        private bool? isChecked;
        public bool? IsChecked
        {
            get { return isChecked; }
            set
            { 
                isChecked = value;
                RaisePropertyChanged("IsChecked");
            }
        }
        public String Name
        {
            get { return Folder.Name; }
        }

        public String  Path
        {
            get { return Folder.FullName; }
        }

        public TreeViewItemModel()
        {

        }

        public TreeViewItemModel(string path, TreeViewItemModel parent)
        {
            Folder = new DirectoryInfo(path);
            Parent = parent;
            children = new ObservableCollection<TreeViewItemModel>();
            if (Serialization.deserializeXML("folders.xml").Contains(Folder.FullName))
                isChecked = true;
            else
            if (parent == null)
                isChecked = false;
            else
            if(parent != null)
                isChecked = parent.IsChecked;
        }

        public void AddItem(TreeViewItemModel item)
        {
            children.Add(item);
            RaisePropertyChanged("Children");
        }

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool HaveAtLeastOneUnCheckedChild()
        {
            int total = 0;
            foreach (var c in children.ToList())
            {
                if (c.IsChecked == null)
                    continue;
                else
                    if (!(bool)c.IsChecked)
                        total++;
            }
            return total > 0;
        }

        private bool HaveAtLeastOneCheckedChild()
        {
            int total = 0;
            foreach(var c in children.ToList())
            {
                if (c.IsChecked == null)
                    continue;
                else
                    if ((bool)c.IsChecked)
                        total++;
            }
             return total > 0;
        }

        private bool HaveAtLeastOneNullCheckedChild()
        {
          foreach(var c in children.ToList())
          {
              if (c.isChecked == null)
                  return true;
          }
            return false;
        }

        public void UpdateParentIsChecked()
        {
            if (Parent == null)
                return;
            if (Parent.HaveAtLeastOneCheckedChild() && Parent.HaveAtLeastOneUnCheckedChild())
                Parent.IsChecked = null;
            else
                if (!Parent.HaveAtLeastOneUnCheckedChild())
                    Parent.IsChecked = true;
                else
                    if (!Parent.HaveAtLeastOneCheckedChild() && !Parent.HaveAtLeastOneNullCheckedChild())
                        Parent.IsChecked = false;
                    else
                        Parent.IsChecked = null;
            Parent.UpdateParentIsChecked();
        }

    }
}
