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

namespace WiFiControlWPF.DialogBox
{
    /// <summary>
    /// Логика взаимодействия для SetLoginPassword.xaml
    /// </summary>
    public partial class SetLoginPassword : Window
    {
        public SetLoginPassword()
        {
            InitializeComponent();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public string Login
        {
            get { return LogBox.Text; }
            set { LogBox.Text = value; }
        }

        public string Password
        {
            get { return PassBox.Text; }
            set { PassBox.Text = value; }
        }
    }
}
