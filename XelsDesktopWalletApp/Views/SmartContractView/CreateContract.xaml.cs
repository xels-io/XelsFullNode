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
using XelsDesktopWalletApp.Models.CommonModels;

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
            //this.txtBalance.Text = addressBalance;

        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;

        }

        private void btn_CreateSubmit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Create_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}
