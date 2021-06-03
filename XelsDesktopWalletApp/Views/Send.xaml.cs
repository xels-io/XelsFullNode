﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for Send.xaml
    /// </summary>
    public partial class Send : Window
    {

        private static HttpClient client = new HttpClient();
        private readonly string baseURL = "http://localhost:37221/api";

        private readonly WalletInfo walletInfo = new WalletInfo();
        private TransactionSending transactionSending = new TransactionSending();

        private double estimatedFee = 0;

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


        public Send()
        {
            InitializeComponent();
        }

        public Send(string walletname)
        {
            InitializeComponent();
            this.DataContext = this;

            this.walletName = walletname;
            this.walletInfo.walletName = this.walletName;
            EstimateFeeAsync();
            LoadCreateAsync();
        }


        public bool isValid()
        {

            if (this.textSidechainDestinationAddress.Text == string.Empty)
            {
                MessageBox.Show("An address is required.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                this.textSidechainDestinationAddress.Focus();
                return false;
            }

            if (this.textSidechainDestinationAddress.Text.Length > 25)
            {
                MessageBox.Show("An address is at least 26 characters long.");
                this.textSidechainDestinationAddress.Focus();
                return false;
            }

            if (this.textAmount.Text == string.Empty)
            {
                MessageBox.Show("An amount is required.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                this.textAmount.Focus();
                return false;
            }

            if (this.textAmount.Text.Length > 0.00001)
            {
                MessageBox.Show("The amount has to be more or equal to 1.");
                this.textAmount.Focus();
                return false;
            }

            if (this.textAmount.Text.Length > 25)
            {
                MessageBox.Show("The total transaction amount exceeds your spendable balance.");
                this.textAmount.Focus();
                return false;
            }

            if (!Regex.IsMatch(this.textAmount.Text, @"^([0-9]+)?(\.[0-9]{0,8})?$"))
            {
                MessageBox.Show("Enter a valid transaction amount. Only positive numbers and no more than 8 decimals are allowed.");
                this.textAmount.Focus();
                return false;
            }

            if (this.textTransactionFee.Text == "")
            {
                MessageBox.Show("Transaction Fee is required.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                this.textTransactionFee.Focus();
                return false;
            }

            if (this.password.Password == "")
            {
                MessageBox.Show("Your password is required.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                this.password.Focus();
                return false;
            }

            return true;
        }

        public async void LoadCreateAsync()
        {
            await GetWalletBalanceAsync(this.baseURL);

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
            _ = BuildTransactionAsync();
        }


        private async Task GetMaxBalanceAsync()
        {

            string postUrl = this.baseURL + $"/wallet/send-transaction";
            var content = "";

            MaximumBalance maximumBalance = new MaximumBalance();
            maximumBalance.WalletName = this.walletInfo.walletName;
            maximumBalance.AccountName = "account 0";
            maximumBalance.FeeType = "medium";
            maximumBalance.AllowUnconfirmed = true;

            HttpResponseMessage response = await client.PostAsync(postUrl, new StringContent(JsonConvert.SerializeObject(maximumBalance), Encoding.UTF8, "application/json"));


            if (response.IsSuccessStatusCode)
            {
                content = await response.Content.ReadAsStringAsync();
            }
            else
            {
                MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }
            this.estimatedFee = double.Parse(content);

        }


        //private List<Recipient> GetRecipient()
        //{
        //    List<Recipient> recipientList = new List<Recipient>();
        //    Recipient _recipient = new Recipient();
        //    _recipient.destinationAddress = this.textSidechainDestinationAddress.Text;
        //    _recipient.amount = this.textAmount.Text;
        //    recipientList.Add(_recipient);
        //    return recipientList;
        //}


        private async void EstimateFeeAsync()
        {

            string postUrl = this.baseURL + $"/wallet/estimate-txfee";
            var content = "";

            FeeEstimation feeEstimation = new FeeEstimation();
            feeEstimation.walletName = this.walletInfo.walletName;
            feeEstimation.accountName = "account 0";
            feeEstimation.destinationAddress = this.textSidechainDestinationAddress.Text;
            feeEstimation.amount = this.textAmount.Text;
            feeEstimation.feeType = "medium"; // it should be set from view page, not here
            feeEstimation.allowUnconfirmed = true;

            // need to convert to array - recipient 
            // make and send proper structure of object to call api - feeEstimation
            // api service - in angular project
            HttpResponseMessage response = await client.PostAsync(postUrl, new StringContent(JsonConvert.SerializeObject(feeEstimation), Encoding.UTF8, "application/json"));


            if (response.IsSuccessStatusCode)
            {
                content = await response.Content.ReadAsStringAsync();
            }
            else
            {
                MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }
            this.estimatedFee = double.Parse(content);
        }

        private async Task BuildTransactionAsync()
        {
            string postUrl = this.baseURL + $"/wallet/build-transaction";
            var content = "";

            TransactionBuilding transactionBuilding = new TransactionBuilding();
            transactionBuilding.walletName = this.walletInfo.walletName;
            transactionBuilding.accountName = "account 0";
            transactionBuilding.password = this.password.Password;
            transactionBuilding.destinationAddress = this.textSidechainDestinationAddress.Text.Trim();
            transactionBuilding.amount = this.textAmount.Text;
            transactionBuilding.feeAmount = this.estimatedFee / 100000000;
            transactionBuilding.allowUnconfirmed = true;
            transactionBuilding.shuffleOutputs = false;


            HttpResponseMessage response = await client.PostAsync(postUrl, new StringContent(JsonConvert.SerializeObject(transactionBuilding), Encoding.UTF8, "application/json"));


            if (response.IsSuccessStatusCode)
            {
                content = await response.Content.ReadAsStringAsync();
            }
            else
            {
                MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }

            // this.estimatedFee = response.fee;
            // this.transactionSending.hex = response.hex;

            _ = SendTransactionAsync(this.transactionSending);
        }


        private async Task SendTransactionAsync(TransactionSending tranSending)
        {
            if (isValid())
            {
                string postUrl = this.baseURL + $"/wallet/send-transaction";
                // var content = "";

                HttpResponseMessage response = await client.PostAsync(postUrl, new StringContent(JsonConvert.SerializeObject(tranSending), Encoding.UTF8, "application/json"));


                if (response.IsSuccessStatusCode)
                {
                    // content = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
                }
            }

        }

        private async Task GetWalletBalanceAsync(string path)
        {
            string getUrl = path + $"/wallet/balance?WalletName={this.walletInfo.walletName}&AccountName=account 0";
            var content = "";

            HttpResponseMessage response = await client.GetAsync(getUrl);


            if (response.IsSuccessStatusCode)
            {
                content = await response.Content.ReadAsStringAsync();

                //this.addresses = JsonConvert.DeserializeObject<ReceiveWalletArray>(content);
            }
            else
            {
                MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }

            //ListConvert(this.addresses);

        }


    }
}
