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
    /// Interaction logic for TransactionDetail.xaml
    /// </summary>
    public partial class TransactionDetail : Window
    {
        #region Base
        static HttpClient client = new HttpClient();
        string baseURL = "http://localhost:37221/api";
        #endregion
        #region Wallet Info
        private readonly WalletInfo walletInfo = new WalletInfo();
        private WalletGeneralInfoModel walletGeneralInfo = new WalletGeneralInfoModel();

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

        private int? lastBlockSyncedHeight;
        private int? confirmations;
        private TransactionInfo _transaction = new TransactionInfo();
        public TransactionDetail()
        {
            InitializeComponent();
        }
        public TransactionDetail(string walletname, TransactionInfo transaction)
        {
            InitializeComponent();
            this.DataContext = this;
            this.walletName = walletname;
            this.walletInfo.walletName = this.walletName;
            GetTransaction(transaction);
            _ = GetGeneralWalletInfoAsync(this.baseURL);

        }

        private void GetTransaction(TransactionInfo transaction)
        {
            this._transaction.transactionId = transaction.transactionId;
            this._transaction.transactionAmount = transaction.transactionAmount;
            this._transaction.transactionFee = transaction.transactionFee;
            this._transaction.transactionConfirmedInBlock = transaction.transactionConfirmedInBlock;
            this._transaction.transactionTimestamp = transaction.transactionTimestamp;
            this._transaction.transactionType = transaction.transactionType;
            this._transaction.transactionTypeName = transaction.transactionTypeName;
    }


        private async Task GetGeneralWalletInfoAsync(string path)
        {
            string getUrl = path + $"/wallet/general-info?Name={this.walletInfo.walletName}";
            var content = "";

            HttpResponseMessage response = await client.GetAsync(getUrl);


            if (response.IsSuccessStatusCode)
            {
                content = await response.Content.ReadAsStringAsync();

                this.walletGeneralInfo = JsonConvert.DeserializeObject<WalletGeneralInfoModel>(content);

                this.lastBlockSyncedHeight = this.walletGeneralInfo.lastBlockSyncedHeight;
                GetConfirmations(this._transaction);
            }
            else
            {
                MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }

        }

        private void GetConfirmations(TransactionInfo transactionInfo)
        {
            if (transactionInfo.transactionConfirmedInBlock>0)
            {
                this.confirmations = this.lastBlockSyncedHeight - transactionInfo.transactionConfirmedInBlock + 1;
            }
            else
            {
                this.confirmations = 0;
            }
        }


    }
}
