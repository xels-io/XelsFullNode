using System;
using System.Collections.Generic;
using System.Net.Http;
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
using Newtonsoft.Json;
using XelsDesktopWalletApp.Models;

namespace XelsDesktopWalletApp.Views
{
    /// <summary>
    /// Interaction logic for SendSelsBels.xaml
    /// </summary>
    public partial class SendSelsBels : Window
    {

        static HttpClient client = new HttpClient();
        private readonly string baseURL = "http://localhost:37221/api";

        private readonly WalletInfo walletInfo = new WalletInfo();
        private TransactionSending transactionSending = new TransactionSending();



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


        public SendSelsBels()
        {
            InitializeComponent();
        }

        public SendSelsBels(string walletname)
        {
            InitializeComponent();
            this.DataContext = this;

            this.walletName = walletname;
            this.walletInfo.walletName = this.walletName;
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            Dashboard ds = new Dashboard(this.walletName);
            ds.Show();
            this.Close();
        }

        private void XELS_Button_Click(object sender, RoutedEventArgs e)
        {
            Send send = new Send(this.walletName);
            send.Show();
            this.Close();
        }

        private void SELS_Button_Click(object sender, RoutedEventArgs e)
        {
            SendSelsBels sendsb = new SendSelsBels(this.walletName);
            sendsb.Show();
            this.Close();
        }

        private void BELS_Button_Click(object sender, RoutedEventArgs e)
        {
            SendSelsBels sendsb = new SendSelsBels(this.walletName);
            sendsb.Show();
            this.Close();
        }


        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            _ = SendTransactionAsync();
        }


        private async Task SendTransactionAsync()
        {
            string postUrl = this.baseURL + $"/wallet/send-transaction";

            HttpResponseMessage response = await client.PostAsync(postUrl, new StringContent(JsonConvert.SerializeObject(this.transactionSending.hex), Encoding.UTF8, "application/json"));


            if (response.IsSuccessStatusCode)
            {

            }
            else
            {
                MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }

        }




    }
}
