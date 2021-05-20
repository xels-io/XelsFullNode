using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace XelsDesktopWalletApp.Views
{
    /// <summary>
    /// Interaction logic for LogoutConfirm.xaml
    /// </summary>
    public partial class LogoutConfirm : Window
    {
        private string walletName;
        public string WalletName
        {
            get
            {
                return this.walletName;
            }
            set
            {
                this.walletName = value;
            }
        }
        public LogoutConfirm()
        {
            InitializeComponent();
        }
        public LogoutConfirm(string walletname)
        {
            InitializeComponent();
            this.walletName = walletname;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Dashboard ds = new Dashboard(this.walletName);
            ds.Show();
            this.Close();
        }

        private void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            Create cr = new Create();
            cr.Show();
            this.Close();
        }
    }
}
