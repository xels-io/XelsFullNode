using System;
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
using NBitcoin;
using Newtonsoft.Json;
using XelsDesktopWalletApp.Models;
using XelsDesktopWalletApp.Models.CommonModels;

namespace XelsDesktopWalletApp.Views
{
    /// <summary>
    /// Interaction logic for Send.xaml
    /// </summary>
    public partial class Send : Window
    {

        //private static HttpClient client = new HttpClient();
        private readonly string baseURL = URLConfiguration.BaseURL;// "http://localhost:37221/api";

        private readonly WalletInfo walletInfo = new WalletInfo();
        private TransactionSending transactionSending = new TransactionSending();
        private TransactionBuilding transactionBuilding = new TransactionBuilding();

        private WalletBalanceArray balances = new WalletBalanceArray();
        private BuildTransaction buildTransaction = new BuildTransaction();
        
        private Money totalBalance;
        private Xels.Bitcoin.Features.Wallet.CoinType cointype;
        private Money spendableBalance;

        private Money estimatedFee = 0;
        private bool isSending = false;

        private Money opReturnAmount = 1;
        
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
            LoadCreateAsync();
        }


        public bool isAddrAmtValid()
        {


            if (this.textSidechainDestinationAddress.Text == string.Empty)
            {
                MessageBox.Show("An address is required.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                this.textSidechainDestinationAddress.Focus();
                return false;
            }

            if (this.textSidechainDestinationAddress.Text.Length < 26)
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

            if (this.textAmount.Text.Length < 0.00001)
            {
                MessageBox.Show("The amount has to be more or equal to 1.");
                this.textAmount.Focus();
                return false;
            }

            if (this.textAmount.Text.Length > ((this.spendableBalance - this.estimatedFee) / 100000000))
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

            return true;
        }

        public bool isValid()
        {
            if (isAddrAmtValid())
            {

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


        private void TxtAmount_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.textSidechainDestinationAddress.Text != "" && this.textAmount.Text != "")
            {
                EstimateFeeAsync();
                this.textAmount.Focus();
            }
        }
        private void TxtAddress_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.textSidechainDestinationAddress.Text != "" && this.textAmount.Text != "")
            {
                EstimateFeeAsync();
                this.textSidechainDestinationAddress.Focus();
            }
        }

        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            this.isSending = true;
            _ = BuildTransactionAsync();
        }


        private async Task GetWalletBalanceAsync(string path)
        {
            string getUrl = path + $"/wallet/balance?WalletName={this.walletInfo.walletName}&AccountName=account 0";
            var content = "";

            HttpResponseMessage response = await URLConfiguration.Client.GetAsync(getUrl);


            if (response.IsSuccessStatusCode)
            {
                content = await response.Content.ReadAsStringAsync();

                this.balances = JsonConvert.DeserializeObject<WalletBalanceArray>(content);

                this.totalBalance = this.balances.balances[0].amountConfirmed + this.balances.balances[0].amountUnconfirmed;
                this.cointype = this.balances.balances[0].coinType;
                this.spendableBalance = this.balances.balances[0].spendableAmount;

                this.textAvailableCoin.Content = this.totalBalance.ToString();
                this.textCoinType.Content = this.cointype;

            }
            else
            {
                MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }


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

            HttpResponseMessage response = await URLConfiguration.Client.PostAsync(postUrl, new StringContent(JsonConvert.SerializeObject(maximumBalance), Encoding.UTF8, "application/json"));


            if (response.IsSuccessStatusCode)
            {
                content = await response.Content.ReadAsStringAsync();
                this.estimatedFee = Money.Parse(content);
            }
            else
            {
                MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }

        }


        private Recipient[] GetRecipient()
        {

            Recipient[] recipients = new Recipient[1];

            recipients[0].destinationAddress = this.textSidechainDestinationAddress.Text.Trim();
            recipients[0].amount = this.textAmount.Text;

            return recipients;
        }


        private async void EstimateFeeAsync()
        {
            if (isAddrAmtValid())
            {
                this.textTransactionFee.Text = "medium";
                Recipient[] recipients = GetRecipient();

                string postUrl = this.baseURL + $"/wallet/estimate-txfee";
                var content = "";

                FeeEstimation feeEstimation = new FeeEstimation();
                feeEstimation.walletName = this.walletInfo.walletName;
                feeEstimation.accountName = "account 0";
                feeEstimation.recipients = recipients;
                feeEstimation.feeType = this.textTransactionFee.Text;
                feeEstimation.allowUnconfirmed = true;

                HttpResponseMessage response = await URLConfiguration.Client.PostAsync(postUrl, new StringContent(JsonConvert.SerializeObject(feeEstimation), Encoding.UTF8, "application/json"));


                if (response.IsSuccessStatusCode)
                {
                    content = await response.Content.ReadAsStringAsync();
                    this.estimatedFee = Money.Parse(content);
                }
                else
                {
                    MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
                }
            }

        }

        private async Task BuildTransactionAsync()
        {
            Recipient[] recipients = GetRecipient();


            string postUrl = this.baseURL + $"/wallet/build-transaction";
            var content = "";

            this.transactionBuilding.walletName = this.walletInfo.walletName;
            this.transactionBuilding.accountName = "account 0";
            this.transactionBuilding.password = this.password.Password;
            this.transactionBuilding.recipients = recipients;
            this.transactionBuilding.feeAmount = this.estimatedFee / 100000000;
            this.transactionBuilding.allowUnconfirmed = true;
            this.transactionBuilding.shuffleOutputs = false;


            HttpResponseMessage response = await URLConfiguration.Client.PostAsync(postUrl, new StringContent(JsonConvert.SerializeObject(this.transactionBuilding), Encoding.UTF8, "application/json"));


            if (response.IsSuccessStatusCode)
            {
                content = await response.Content.ReadAsStringAsync();
                this.buildTransaction = JsonConvert.DeserializeObject<BuildTransaction>(content);

                this.estimatedFee = this.buildTransaction.fee;
                this.transactionSending.hex = this.buildTransaction.hex;

                if (this.isSending)
                {
                    _ = SendTransactionAsync(this.transactionSending);
                }

            }
            else
            {
                this.isSending = false;
                MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }

        }


        private async Task SendTransactionAsync(TransactionSending tranSending)
        {
            if (isValid())
            {
                string postUrl = this.baseURL + $"/wallet/send-transaction";
                // var content = "";

                HttpResponseMessage response = await URLConfiguration.Client.PostAsync(postUrl, new StringContent(JsonConvert.SerializeObject(tranSending), Encoding.UTF8, "application/json"));


                if (response.IsSuccessStatusCode)
                {
                    // content = await response.Content.ReadAsStringAsync();

                    SendConfirmation sendConfirmation = new SendConfirmation();
                    sendConfirmation.transaction = this.transactionBuilding;
                    sendConfirmation.transactionFee = this.estimatedFee;
                    sendConfirmation.cointype = this.cointype;

                    SendConfirmationMainChain sendConf = new SendConfirmationMainChain(sendConfirmation, this.walletName);
                    sendConf.Show();
                    this.Close();

                }
                else
                {
                    this.isSending = false;
                    MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
                }
            }

        }


    }
}
