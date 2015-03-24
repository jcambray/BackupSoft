using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace clientbackup
{
    class FileSaveItem : ISaveItem
    {

        string name;
        long size;
        FileInfo file;
        DateTime dateofCopy;
        

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
                return size;
            }
        }

 

        public FileInfo File
        {
            get { return file; }
        }


        public FileSaveItem(string fullName)
        {
            file = new FileInfo(fullName);
            size = file.Length;
            name = file.Name;
            dateofCopy = DateTime.MinValue;
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

        public void Copy(string parentName)
        {
            file.CopyTo(parentName + @"\" + file.Name);
            dateofCopy = DateTime.Now;
        }

        public string GetParent()
        {
            return file.Directory.Name;
        }
    }
}
