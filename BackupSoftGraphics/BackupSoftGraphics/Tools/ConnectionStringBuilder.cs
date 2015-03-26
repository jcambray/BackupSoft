using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupSoftGraphics.Tools
{
   public class ConnectionStringBuilder
    {
       public static String GetRelativeConnectionString(string dataSource,string databaseLocation,string integratedSecurity,string connectionTimeout)
       {
           StringBuilder sb = new StringBuilder();
           sb.Append("Data Source=");
           sb.Append(dataSource + ";");
           sb.Append("AttachDbFilename=");
           sb.Append(databaseLocation + ";");
           sb.Append("Integrated Security=");
           sb.Append(integratedSecurity + ";");
           sb.Append("Connect Timeout=");
           sb.Append(connectionTimeout);
           return sb.ToString();
       }
    }
}
