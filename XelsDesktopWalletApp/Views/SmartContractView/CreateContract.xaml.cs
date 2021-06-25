using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using XelsDesktopWalletApp.Models;
using XelsDesktopWalletApp.Models.CommonModels;
using XelsDesktopWalletApp.Models.SmartContractModels;

namespace XelsDesktopWalletApp.Views.SmartContractView
{
    /// <summary>
    /// Interaction logic for CreateContract.xaml
    /// </summary>
    public partial class CreateContract : UserControl
    {
        #region Base
        static HttpClient client = new HttpClient();
        string baseURL = URLConfiguration.BaseURL;// Common Url

        long gasCallLimitMinimum = 10000;
        long gasCallRecommendedLimit = 50000;

        long gasCreateLimitMinimum = 12000;
        long gasCreateTokenLimitMinimum = 15000;
        long gasLimitMaximum = 250000;
        long gasPriceMinimum = 1;
        long gasPriceMaximum = 10000;

        long amountA = 0;
        double feeAmt = 0.001;
        double gasPrice = 100;
        long gasLimit = 12000;

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


        public CreateContract()
        {
            InitializeComponent();
        }

        public CreateContract(string walletname, string selectedAddress, string addressBalance)
        {
            InitializeComponent();
            this.DataContext = this;
            this.walletName = walletname;
            this.walletInfo.walletName = this.walletName;
            this.txtSender.Text = selectedAddress;
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

        private void btn_CreateSubmit_Click(object sender, RoutedEventArgs e)
        {
            CreateContractModel objcreateContractModel = new CreateContractModel();
            objcreateContractModel.amount = this.txtAmount.Text;
            objcreateContractModel.feeAmount = this.txtFee.Text;
            objcreateContractModel.gasPrice = Convert.ToUInt64(this.txtGasPrice.Text);
            objcreateContractModel.gasLimit = Convert.ToUInt64(this.txtGasLimit.Text);
            objcreateContractModel.contractCode = this.txtByteCode.Text;
            objcreateContractModel.password = this.txtPassword.Password;
            objcreateContractModel.sender = this.txtSender.Text;
            objcreateContractModel.walletName = this.walletName;

            if (ValidationCheck())
            {
                CreateContractSubmitAsync(objcreateContractModel);
            }
        }

        public bool ValidationCheck()
        {
            if (Convert.ToInt64(this.txtAmount.Text) > Convert.ToInt64(this.txtBalance.Text))
            {
                MessageBox.Show("The amount you have entered exceeds balance available at the sender address", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                this.txtAmount.Focus();
                return false;
            }

            if (Convert.ToInt64(this.txtAmount.Text) < 0 || Convert.ToInt64(this.txtAmount.Text) == 0 || this.txtAmount.Text == "")
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
                MessageBox.Show("Gas price must be greater than  " + this.gasPriceMinimum, "Failed", MessageBoxButton.OK, MessageBoxImage.Error);

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

            if (Convert.ToInt64(this.txtGasLimit.Text) < 0 || this.txtGasLimit.Text == "")
            {
                MessageBox.Show("The amount cannot be negative", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                this.txtGasLimit.Focus();
                return false;
            }

            //if (this.txtContractAddress.Text == "")
            //{
            //    MessageBox.Show("Contract Address Is reuired.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            //    this.txtContractAddress.Focus();
            //    return false;
            //}

            //if (this.txtMethodName.Text == "")
            //{
            //    MessageBox.Show("Method Name Is reuired.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            //    this.txtMethodName.Focus();
            //    return false;
            //}

            //if (this.txtPassword.Password == "")
            //{
            //    MessageBox.Show("Password is reuired. Please enter the password for wallet: " + this.walletName, "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            //    this.txtMethodName.Focus();
            //    return false;
            //}


            return true;
        }


        private async void CreateContractSubmitAsync(CreateContractModel createContractModel)
        {
            string postUrl = this.baseURL + "/SmartContractWallet/create";

            HttpResponseMessage response = await client.PostAsync(postUrl, new StringContent(JsonConvert.SerializeObject(createContractModel), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Successfully Create Contract ");

                this.Visibility = Visibility.Collapsed;
            }
            else
            {
                MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }
        }
        private void btn_Create_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}
