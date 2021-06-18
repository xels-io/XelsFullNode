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
using System.Windows.Navigation;
using System.Windows.Shapes;
using NBitcoin;
using Newtonsoft.Json;
using XelsDesktopWalletApp.Models;

namespace XelsDesktopWalletApp.Views
{
    /// <summary>
    /// Interaction logic for History.xaml
    /// </summary>
    public partial class History : Window
    {

        #region Base
        static HttpClient client = new HttpClient();
        string baseURL = "http://localhost:37221/api";
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

        private HistoryModelArray historyModelArray = new HistoryModelArray();
        private List<TransactionInfo> transactions = new List<TransactionInfo>();
        public History()
        {
            InitializeComponent();
        }
        public History(string walletname)
        {
            InitializeComponent();
            this.DataContext = this;

            this.walletName = walletname;
            this.walletInfo.walletName = this.walletName;
            _ = GetWalletHistoryAsync(this.baseURL);
        }


        private async Task GetWalletHistoryAsync(string path)
        {
            string getUrl = path + $"/wallet/history?WalletName={this.walletInfo.walletName}&AccountName=account 0";
            var content = "";

            HttpResponseMessage response = await client.GetAsync(getUrl);


            if (response.IsSuccessStatusCode)
            {
                content = await response.Content.ReadAsStringAsync();
                
                this.historyModelArray = JsonConvert.DeserializeObject<HistoryModelArray>(content);

                if (this.historyModelArray.history != null && this.historyModelArray.history[0].transactionsHistory.Length > 0)
                {
                    int transactionsLen = this.historyModelArray.history[0].transactionsHistory.Length;
                    this.NoData.Visibility = Visibility.Hidden;
                    this.HistoryListBinding.Visibility = Visibility.Visible;

                    TransactionItemModel[] historyResponse = new TransactionItemModel[transactionsLen];
                    historyResponse = this.historyModelArray.history[0].transactionsHistory;

                    GetTransactionInfo(historyResponse);
                }
                else
                {
                    this.HistoryListBinding.Visibility = Visibility.Hidden;
                    this.NoData.Visibility = Visibility.Visible;
                }
            }
            else
            {
                MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }

        }


        private void GetTransactionInfo(TransactionItemModel[] transactions)
        {

            foreach (TransactionItemModel transaction in transactions)
            {
                TransactionInfo transactionInfo = new TransactionInfo();

                //Type
                if (transaction.type == TransactionItemType.Send)
                {
                    transactionInfo.transactionType = "sent";
                }
                else if (transaction.type == TransactionItemType.Received)
                {
                    transactionInfo.transactionType = "received";
                }
                else if (transaction.type == TransactionItemType.Staked)
                {
                    transactionInfo.transactionType = "hybrid reward";
                }
                else if (transaction.type == TransactionItemType.Mined)
                {
                    transactionInfo.transactionType = "pow reward";
                }

                //Id
                transactionInfo.transactionId = transaction.id;

                //Amount
                transactionInfo.transactionAmount = transaction.amount ?? 0;

                //Fee
                if (transaction.fee != null)
                {
                    transactionInfo.transactionFee = transaction.fee;
                }
                else
                {
                    transactionInfo.transactionFee = 0;
                }

                //FinalAmount
                if (transactionInfo.transactionType != null)
                {
                    if (transactionInfo.transactionType == "sent")
                    {
                        Money finalAmt = transactionInfo.transactionAmount + transactionInfo.transactionFee;
                        transactionInfo.transactionFinalAmount = $" - {finalAmt}";
                    }
                    else if (transactionInfo.transactionType == "received")
                    {
                        Money finalAmt = transactionInfo.transactionAmount + transactionInfo.transactionFee;
                        transactionInfo.transactionFinalAmount = $" + {finalAmt}";
                    }
                    else if (transactionInfo.transactionType == "hybrid reward")
                    {
                        Money finalAmt = transactionInfo.transactionAmount + transactionInfo.transactionFee;
                        transactionInfo.transactionFinalAmount = $" + {finalAmt}";
                    }
                    else if (transactionInfo.transactionType == "pow reward")
                    {
                        Money finalAmt = transactionInfo.transactionAmount + transactionInfo.transactionFee;
                        transactionInfo.transactionFinalAmount = $" + {finalAmt}";
                    }
                }
                //ConfirmedInBlock
                transactionInfo.transactionConfirmedInBlock = transaction.confirmedInBlock;
                if (transactionInfo.transactionConfirmedInBlock != 0 || transactionInfo.transactionConfirmedInBlock != null)
                {
                    transactionInfo.transactionTypeName = TransactionItemTypeName.Confirmed;
                }
                else
                {
                    transactionInfo.transactionTypeName = TransactionItemTypeName.Unconfirmed;
                }

                //Timestamp
                transactionInfo.transactionTimestamp = transaction.timestamp;

                transactionInfo.transactionType = transactionInfo.transactionType.ToUpper();
                this.transactions.Add(transactionInfo);
            }

            this.HistoryListBinding.ItemsSource = this.transactions;
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

        private void Hyperlink_NavigateAddressBook(object sender, RequestNavigateEventArgs e)
        {
            AddressBook ex = new AddressBook(this.walletName);
            ex.Show();
            this.Close();
        }
        private void Hyperlink_NavigateLogout(object sender, RequestNavigateEventArgs e)
        {
            LogoutConfirm lc = new LogoutConfirm(this.walletName);
            lc.Show();
            this.Close();
        }


        private void Hyperlink_NavigateAdvanced(object sender, RequestNavigateEventArgs e)
        {
            Advanced adv = new Advanced(this.walletName);
            adv.Show();
            this.Close();
        }


    }
}
