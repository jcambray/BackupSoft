﻿using System;
using  System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupSoftGraphics.Database.Model
{
   public  class BackupFolder
    {
        public int Id { get; set; }
        public String Fullname { get; set; }
        public DateTime? LastSaveDate { get; set; }
        public int Size { get; set; }
        public String Name { get { return Path.GetFileName(Fullname); } }

        public List<BackupFolder> Children { get; set; }

        public bool? IsChecked { get; set; }

        public List<BackupFile> FilesList { get; set; }

        public BackupFolder()
        {
            Children = new List<BackupFolder>();
        }
        
    }
}
