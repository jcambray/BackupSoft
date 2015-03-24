using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace clientbackup
{
    public interface ISaveItem
    {
        string Name { get; set; }
        long Size { get; }
        DateTime DateOfCopy { get; set; }
        void Copy(string path);
        string GetParent();
    }
}
