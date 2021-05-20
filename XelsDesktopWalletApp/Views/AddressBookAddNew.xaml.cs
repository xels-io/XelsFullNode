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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XelsDesktopWalletApp.Views
{
    /// <summary>
    /// Interaction logic for AddressBookAddNew.xaml
    /// </summary>
    public partial class AddressBookAddNew : Window
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


        public AddressBookAddNew()
        {
            InitializeComponent();

            this.DataContext = this;
        }
        public AddressBookAddNew(string walletname)
        {
            InitializeComponent();

            this.DataContext = this;


            this.walletName = walletname;
        }

        private void Show_Click(object sender, RoutedEventArgs e)
        {
            Send send = new Send();
            send.Show();
            this.Close();
            //MyPopup.IsOpen = true;
        }

        private void receiveButton_Click(object sender, RoutedEventArgs e)
        {
            Receive receive = new Receive();
            receive.Show();
            this.Close();
        }
        private void createButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Hide_Click(object sender, RoutedEventArgs e)
        {
            //MyPopup.IsOpen = false;
        }


        private void Hyperlink_NavigateDashboard(object sender, RequestNavigateEventArgs e)
        {
            Dashboard ds = new Dashboard(this.walletName);
            ds.Show();
            this.Close();
        }
        private void Hyperlink_NavigateHistory(object sender, RequestNavigateEventArgs e)
        {
            History hs = new History(this.walletName);
            hs.Show();
            this.Close();
        }
        private void Hyperlink_NavigateExchange(object sender, RequestNavigateEventArgs e)
        {
            Exchange ex = new Exchange(this.walletName);
            ex.Show();
            this.Close();
        }
        private void Hyperlink_NavigateLogout(object sender, RequestNavigateEventArgs e)
        {
            LogoutConfirm lc = new LogoutConfirm(this.walletName);
            lc.Show();
            this.Close();
        }
    }
}
