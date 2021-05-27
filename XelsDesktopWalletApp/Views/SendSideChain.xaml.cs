using System;
using System.Collections.Generic;
using System.Net.Http;
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
    /// Interaction logic for SendSideChain.xaml
    /// </summary>
    public partial class SendSideChain : Window
    {

        static HttpClient client = new HttpClient();
        readonly string baseURL = "http://localhost:37221/api";


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


        public SendSideChain()
        {
            InitializeComponent();
        }

        public SendSideChain(string walletname)
        {
            InitializeComponent();
            this.DataContext = this;

            this.walletName = walletname;
        }
        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            Dashboard ds = new Dashboard(this.walletName);
            ds.Show();
            this.Close();
        }

        private void createButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void XELS_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SELS_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BELS_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Mainchain_Button_Click(object sender, RoutedEventArgs e)
        {

            Send send = new Send(this.walletName);
            send.Show();
            this.Close();
        }

        private void Sidechain_Button_Click(object sender, RoutedEventArgs e)
        {
            SendSideChain sendSC = new SendSideChain(this.walletName);
            sendSC.Show();
            this.Close();

        }

        private void sendButton_Click(object sender, RoutedEventArgs e)
        {

        }


    }
}
