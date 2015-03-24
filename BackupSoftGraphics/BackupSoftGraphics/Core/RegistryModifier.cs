using System;
using System.Net;
using Microsoft.Win32;
using System.Configuration;
using System.Windows.Forms;

namespace clientbackup
{

    public class RegistryModifier
    {

        public static void EnableAutoLogon(string password)
        {
            RegistryKey regKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,RegistryView.Registry64);
            regKey = regKey.CreateSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon");
            regKey.SetValue("AutoAdminLogon", 1);
            regKey.SetValue("DefaultPassword", ConfigurationManager.AppSettings["password"]);
            regKey.SetValue("DefaultDomainName", Environment.UserDomainName);
            regKey.SetValue("DefaultUserName", Environment.UserName);
            regKey.Close();
        }

        public static void disableAutoLogon()
        {
            RegistryKey regKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            regKey = regKey.CreateSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon");
            regKey.SetValue("AutoAdminLogon", 0);
            regKey.Close();
        }

        
        public static void StartWithWindows()
        {
            if (ConfigurationManager.AppSettings["autoStart"] == "0")
            {
                RegistryKey regKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                regKey = regKey.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                regKey.SetValue(@"AUTOMOTOR Backup", Application.ExecutablePath);
                regKey.Close();
                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings.Remove("autoStart");
                config.AppSettings.Settings.Add("autoStart", "1");
                ConfigurationManager.RefreshSection("appSettings");
                config.Save(ConfigurationSaveMode.Modified);
            }
        }
    }
}
