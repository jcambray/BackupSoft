using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using clientbackup;
using System.Collections;

namespace BackupSoftGraphics
{
    /// <summary>
    /// Logique d'interaction pour SavePasswordWindow.xaml
    /// </summary>
    public partial class SavePasswordWindow : Window
    {
        private const String MSG_ERROR = "Les mots de passes sont différents.";
        private EPasswordType type;
        private Dictionary<String,String> passwords;

        public SavePasswordWindow(EPasswordType passwordType)
        {
            InitializeComponent();
            type = passwordType;
            passwords = Serialization.deserializeMDPAdmin();
            if (type == EPasswordType.Administrateur)
            {
                lbType.Content = "Mot de passe Administrateur";
                tbPassword.Password = Security.Decrypt(passwords["admin"]);
                tbConfirm.Password =  Security.Decrypt(passwords["admin"]);
                
            }
            else
            {
                lbType.Content = "Mot de passe utilisateur";
                tbPassword.Password = Security.Decrypt(passwords["user"]);
                tbConfirm.Password = Security.Decrypt(passwords["user"]);
            }

            tbPassword.Focus();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnValidate_Click(object sender, RoutedEventArgs e)
        {
            if (tbConfirm.Password != tbPassword.Password)
                lbError.Content = MSG_ERROR;
            else
            {
                var encryptedPassword = Security.Encrypt(tbPassword.Password);

                if (type == EPasswordType.Administrateur)
                    passwords["admin"] = encryptedPassword;
                else
                    passwords["user"] = encryptedPassword;
                Serialization.serializeMDPAdmin(passwords);
                Close();
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }

    public enum EPasswordType
    {
        Administrateur,
        Utilisateur
    }
}
