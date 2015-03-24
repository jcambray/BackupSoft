using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupSoftGraphics.Database.Model
{
    public class BackupFile
    {
        public int Id { get; set; }
        public String Fullname { get; set; }
        public DateTime? LastSaveDate { get; set; }
        public long Size { get; set; }
    }
}
