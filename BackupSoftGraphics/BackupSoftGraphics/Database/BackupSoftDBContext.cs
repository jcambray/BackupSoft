using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using BackupSoftGraphics.Database.Model;




namespace BackupSoftGraphics.Database
{
     public  class BackupSoftDBContext : DbContext
    {
         public BackupSoftDBContext()
             : base("BackupSoftDB")
         {
             System.Data.Entity.Database.SetInitializer<BackupSoftDBContext>(new CreateDatabaseIfNotExists<BackupSoftDBContext>());
         }

         public DbSet<BackupFolder> BackupFolders { get; set; }
    }
}
