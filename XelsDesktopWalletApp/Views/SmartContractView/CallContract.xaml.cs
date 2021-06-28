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
using System.Windows.Navigation;
using System.Windows.Shapes;
using XelsDesktopWalletApp.Models;
using XelsDesktopWalletApp.Models.SmartContractModels;
using XelsDesktopWalletApp.Models.CommonModels;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Globalization;

namespace XelsDesktopWalletApp.Views.SmartContractView
{
    /// <summary>
    /// Interaction logic for CallContract.xaml
    /// </summary>
    public partial class CallContract : UserControl
    {
        #region Base
        static HttpClient client = new HttpClient();
        string baseURL = URLConfiguration.BaseURL;// Common Url

        long gasCallLimitMinimum = 10000;
        long gasCallRecommendedLimit = 50000;
        //long gasCreateLimitMinimum = 12000;
        //long gasCreateTokenLimitMinimum = 15000;
        long gasLimitMaximum = 250000;
        long gasPriceMinimum = 1;
        long gasPriceMaximum = 10000;

        long amountA = 0;
        double feeAmt = 0.001;
        double gasPrice = 100;
        long gasLimit = 50000;

        #endregion
        #region Wallet Info
        private readonly WalletInfo walletInfo = new WalletInfo();

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


        #endregion
        public CallContract()
        {
            InitializeComponent();
        }
        public CallContract(string walletname, string selectedAddress,string addressBalance)
        {
            InitializeComponent();
            this.DataContext = this;
            this.walletName = walletname;
            this.walletInfo.walletName = this.walletName;
            this.SenderAddress.Text = selectedAddress;
            this.txtAmount.Text = this.amountA.ToString();
            this.txtFee.Text = this.feeAmt.ToString();
            this.txtGasPrice.Text = this.gasPrice.ToString();
            this.txtGasLimit.Text = this.gasLimit.ToString();
            this.txtBalance.Text = addressBalance;
           
        }
        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        private void btn_CallSubmit_Click(object sender, RoutedEventArgs e)
        {
            CallContractModel objCallContractModel = new CallContractModel();
            objCallContractModel.amount = this.txtAmount.Text;
            objCallContractModel.feeAmount = this.txtFee.Text;
            objCallContractModel.gasPrice =Convert.ToUInt64(this.txtGasPrice.Text);
            objCallContractModel.gasLimit = Convert.ToUInt64(this.txtGasLimit.Text);
            objCallContractModel.contractAddress = this.txtContractAddress.Text;
            objCallContractModel.methodName = this.txtMethodName.Text;
            objCallContractModel.password = this.txtWalletPassword.Password;
            objCallContractModel.sender = this.SenderAddress.Text;
            objCallContractModel.walletName = this.walletName;

            if (ValidationCheck())
            {
                CallContractSubmitAsync(objCallContractModel);
            }
 
         }

        public bool ValidationCheck()
        {
            var balanceVal = this.txtBalance.Text;
            var txtamtVal = this.txtAmount.Text;
            double d2 = double.Parse(balanceVal, CultureInfo.InvariantCulture);
            double d3 = double.Parse(txtamtVal, CultureInfo.InvariantCulture);
            if (d3 > d2)
            {
            //    if ( Convert.ToInt64(this.txtAmount.Text) > Convert.ToInt64(this.txtBalance.Text))
            //{
                MessageBox.Show("The amount you have entered exceeds balance available at the sender address", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                this.txtAmount.Focus();
                return false;
            }

            if (Convert.ToInt64(this.txtAmount.Text) < 0 || Convert.ToInt64(this.txtAmount.Text) == 0  || this.txtAmount.Text =="")
            {
                MessageBox.Show("The amount cannot be negative or Zero", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                this.txtAmount.Focus();
                return false;
            }

            if (Convert.ToInt64(this.txtFee.Text) > Convert.ToInt64(this.txtBalance.Text) || Convert.ToInt64(this.txtFee.Text) == Convert.ToInt64(this.txtBalance.Text))
            {
                MessageBox.Show("Fee must be less than your balance", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                this.txtFee.Focus();
                return false;
            }

            if (Convert.ToInt64(this.txtFee.Text) < 0.001 || this.txtFee.Text == "")
            {
                MessageBox.Show("The amount cannot be negative", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                this.txtAmount.Focus();
                return false;
            }


            if (Convert.ToInt64(this.txtGasPrice.Text) < Convert.ToInt64(this.gasPriceMinimum))
            {
                MessageBox.Show("Gas price must be greater than  "+ this.gasPriceMinimum, "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
              
                return false;
            }
            if (Convert.ToInt64(this.txtGasPrice.Text) > Convert.ToInt64(this.gasPriceMaximum))
            {
                MessageBox.Show("Gas price must be less than  " + this.gasPriceMaximum, "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
              
                return false;
            }

            if (Convert.ToInt64(this.txtGasPrice.Text) < 0 || this.txtGasPrice.Text == "" && !Regex.IsMatch(this.txtGasPrice.Text, @"^[0-9][0-9]*$") && !Regex.IsMatch(this.txtGasPrice.Text, @"^[+]?([0-9]{0,})*[.]?([0-9]{0,2})?$"))
            {
                MessageBox.Show("The gas price cannot be negative", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                this.txtGasPrice.Focus();
                return false;
            }
            if (Convert.ToInt64(this.txtGasLimit.Text) > Convert.ToInt64(this.gasLimitMaximum) && !Regex.IsMatch(this.txtGasPrice.Text, @"^[0-9][0-9]*$") && !Regex.IsMatch(this.txtGasPrice.Text, @"^[+]?([0-9]{0,})*[.]?([0-9]{0,2})?$"))
            {
                MessageBox.Show("Gas limit must be less than  " + this.gasLimitMaximum, "Failed", MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }
            if (Convert.ToInt64(this.txtGasLimit.Text) < Convert.ToInt64(this.gasCallLimitMinimum))
            {
                MessageBox.Show("Gas limit must be greater than  " + this.gasCallLimitMinimum, "Failed", MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }

            if (Convert.ToInt64(this.txtGasLimit.Text) < 0 || this.txtGasLimit.Text =="")
            {
                MessageBox.Show("The amount cannot be negative", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                this.txtGasLimit.Focus();
                return false;
            }

            if ( this.txtContractAddress.Text == "")
            {
                MessageBox.Show("Contract Address Is reuired.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                this.txtContractAddress.Focus();
                return false;
            }

            if (this.txtMethodName.Text == "")
            {
                MessageBox.Show("Method Name Is reuired.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                this.txtMethodName.Focus();
                return false;
            }

            if (this.txtWalletPassword.Password == "")
            {
                MessageBox.Show("Password is reuired. Please enter the password for wallet: "+this.walletName, "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                this.txtMethodName.Focus();
                return false;
            }

           
            return true;
        }
        private async void CallContractSubmitAsync(CallContractModel callContractModel)
        {
            string postUrl = this.baseURL + "/SmartContractWallet/call";

            HttpResponseMessage response = await client.PostAsync(postUrl, new StringContent(JsonConvert.SerializeObject(callContractModel), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Successfully Call Contract with  " + callContractModel.contractAddress);

                this.Visibility = Visibility.Collapsed;
            }
            else
            {
                MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }
        }


        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}
