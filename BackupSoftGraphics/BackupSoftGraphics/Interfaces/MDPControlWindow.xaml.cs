﻿using System;
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

namespace BackupSoftGraphics
{
    /// <summary>
    /// Logique d'interaction pour MDPControlWindow.xaml
    /// </summary>
    public partial class MDPControlWindow : Window
    {

        private string adminPwd;

        public MDPControlWindow()
        {
            InitializeComponent();
            adminPwd = Security.Decrypt(Serialization.deserializeMDPAdmin()["admin"]);
            pwdBox.Focus();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (pwdBox.Password == adminPwd)
                DialogResult = true;
            else
                DialogResult = false;
            Close();
        }


    }
}