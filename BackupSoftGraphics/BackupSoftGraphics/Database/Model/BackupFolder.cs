using System;
using  System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackupSoftGraphics.Database.Model
{
   public  class BackupFolder : INotifyPropertyChanged
    {
        public int Id { get; set; }


        public String Fullname { get; set; }

        public DateTime? LastSaveDate { get; set; }

        public int Size { get; set; }

        public String Name { get { return Path.GetFileName(Fullname); } }

        public List<BackupFolder> Children { get; set; }


        private bool? isChecked;
        public bool? IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                isChecked = value;
                RaiseEvent("IsChecked");
            }
        }

        private bool? allChildrenChecked;
        public bool? AllChildrenChecked
        {
            get
            {
                return allChildrenChecked;
            }

            set
            {
                allChildrenChecked = value;
                    RaiseEvent("AllChildrenChecked");
            }
        }

        
        public List<BackupFile> FilesList { get; set; }

        public BackupFolder()
        {
            Children = new List<BackupFolder>();
        }


        public event PropertyChangedEventHandler PropertyChanged;

       private void RaiseEvent(string propertyName)
        {
           if(PropertyChanged != null)
               PropertyChanged(this,new PropertyChangedEventArgs(propertyName));
        }
    }
}
