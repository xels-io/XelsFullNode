using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        private static HttpClient client = new HttpClient();
        private string baseURL = "http://localhost:37221/api";

        private WalletBalanceArray walletBalanceArray = new WalletBalanceArray();
        private HistoryModelArray historyModelArray = new HistoryModelArray();

        TransactionItemModelArray transactionItem = new TransactionItemModelArray();
        private List<TransactionInfo> transactions = new List<TransactionInfo>();

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


        #region Own Property

        public bool sidechainEnabled = false;
        public bool stakingEnabled = false;

        private bool hasBalance = false;
        private Money confirmedBalance;
        private Money unconfirmedBalance;
        private Money spendableBalance;


        private string percentSynced;

        // general info
        private WalletGeneralInfoModel walletGeneralInfoModel = new WalletGeneralInfoModel();
        private string processedText;
        private string blockChainStatus;
        private string connectedNodesStatus;
        private double percentSyncedNumber = 0;


        // Staking  Info
        public bool isStarting = false;
        public bool isStopping = false;

        private StakingInfoModel stakingInfo = new StakingInfoModel();
        public Money awaitingMaturity = 0;

        #endregion


        public Dashboard()
        {
            InitializeComponent();

            this.DataContext = this;
        }
        public Dashboard(string walletname)
        {
            InitializeComponent();

            //this.AccountComboBox.SelectedItem = this.walletName;
            this.DataContext = this;


            this.walletName = walletname;
            this.walletInfo.walletName = this.walletName;
            GetGeneralInfoAsync();
            LoadLoginAsync();
            GetHistoryAsync();

            if (!this.sidechainEnabled)
            {
                _ = GetStakingInfoAsync(this.baseURL);
            }
            PopulateTxt();
        }


        public async void LoadLoginAsync()
        {
            await GetWalletBalanceAsync(this.baseURL);
        }

        private async Task GetWalletBalanceAsync(string path)
        {
            try
            {
                string getUrl = path + $"/wallet/balance?WalletName={this.walletInfo.walletName}&AccountName=account 0";
                var content = "";

                HttpResponseMessage response = await client.GetAsync(getUrl);


                if (response.IsSuccessStatusCode)
                {
                    content = await response.Content.ReadAsStringAsync();

                    this.walletBalanceArray = JsonConvert.DeserializeObject<WalletBalanceArray>(content);

                    this.confirmedBalance = this.walletBalanceArray.balances[0].amountConfirmed;
                    this.unconfirmedBalance = this.walletBalanceArray.balances[0].amountUnconfirmed;
                    this.spendableBalance = this.walletBalanceArray.balances[0].spendableAmount;

                    if ((this.confirmedBalance + this.unconfirmedBalance) > 0)
                    {
                        this.hasBalance = true;
                    }
                    else
                    {
                        this.hasBalance = false;
                    }
                    // Balance info
                    this.ConfirmedBalanceTxt.Text = $"{this.confirmedBalance} XELS";
                    this.UnconfirmedBalanceTxt.Text = $"{this.unconfirmedBalance} (unconfirmed)";
                }
                else
                {
                    MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
                }
            }
            catch (Exception r)
            {

                throw;
            }

        }


        public async void GetHistoryAsync()
        {
            await GetWalletHistoryAsync(this.baseURL);
        }

        private async Task GetWalletHistoryAsync(string path)
        {
            string getUrl = path + $"/wallet/history?WalletName={this.walletInfo.walletName}&AccountName=account 0";
            var content = "";

            HttpResponseMessage response = await client.GetAsync(getUrl);

            if (response.IsSuccessStatusCode)
            {
                content = await response.Content.ReadAsStringAsync();

                this.transactionItem = JsonConvert.DeserializeObject<TransactionItemModelArray>(content);


                //if (this.historyModelArray.history != null && this.historyModelArray.history[0].transactionsHistory.Length > 0)
                //{
                //    int transactionsLen = this.historyModelArray.history[0].transactionsHistory.Length;
                //    this.NoData.Visibility = Visibility.Hidden;
                //    this.HistoryList.Visibility = Visibility.Visible;

                //    TransactionItemModel[] historyResponse = new TransactionItemModel[transactionsLen];
                //    historyResponse = this.historyModelArray.history[0].transactionsHistory;

                //    GetTransactionInfo(historyResponse);


                if (this.transactionItem.Transactions != null && this.transactionItem.Transactions.Length > 0)
                {
                    int transactionsLen = this.transactionItem.Transactions.Length;
                    this.NoData.Visibility = Visibility.Hidden;
                    this.HistoryList.Visibility = Visibility.Visible;

                    TransactionItemModel[] historyResponse = new TransactionItemModel[transactionsLen];
                    historyResponse = this.historyModelArray.history[0].transactionsHistory;

                    GetTransactionInfo(historyResponse);
                }
                else
                {
                    this.HistoryList.Visibility = Visibility.Hidden;
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
                //this.transactions.Take(5);
            }

            this.HistoryList.ItemsSource = this.transactions.Take(5);

        }

        public async void GetGeneralInfoAsync()
        {
            await GetGeneralWalletInfoAsync(this.baseURL);
        }

        private async Task GetGeneralWalletInfoAsync(string path)
        {
            string getUrl = path + $"/wallet/general-info?Name={this.walletInfo.walletName}";
            var content = "";

            HttpResponseMessage response = await client.GetAsync(getUrl);

            if (response.IsSuccessStatusCode)
            {
                content = await response.Content.ReadAsStringAsync();
                this.walletGeneralInfoModel = JsonConvert.DeserializeObject<WalletGeneralInfoModel>(content);

                this.processedText = $"Processed { this.walletGeneralInfoModel.lastBlockSyncedHeight ?? 0} out of { this.walletGeneralInfoModel.chainTip} blocks.";
                this.blockChainStatus = $"Synchronizing.  { this.processedText}";

                if (this.walletGeneralInfoModel.connectedNodes == 1)
                {
                    this.connectedNodesStatus = "1 connection";
                }
                else if (this.walletGeneralInfoModel.connectedNodes >= 0)
                {
                    this.connectedNodesStatus = $"{ this.walletGeneralInfoModel.connectedNodes} connections";
                }

                if (!this.walletGeneralInfoModel.isChainSynced)
                {
                    this.percentSynced = "syncing...";
                }
                else
                {
                    this.percentSyncedNumber = ((this.walletGeneralInfoModel.lastBlockSyncedHeight / this.walletGeneralInfoModel.chainTip) * 100) ?? 0;
                    if (Math.Round(this.percentSyncedNumber) == 100 && this.walletGeneralInfoModel.lastBlockSyncedHeight != this.walletGeneralInfoModel.chainTip)
                    {
                        this.percentSyncedNumber = 99;
                    }

                    this.percentSynced = $"{ Math.Round(this.percentSyncedNumber)} %";

                    if (this.percentSynced == "100%")
                    {
                        this.blockChainStatus = $"Up to date.  { this.processedText}";
                    }
                }
            }
            else
            {
                MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }
        }


        private async Task GetStakingInfoAsync(string path)
        {
            string getUrl = path + $"/staking/getstakinginfo";
            var content = "";

            HttpResponseMessage response = await client.GetAsync(getUrl);

            if (response.IsSuccessStatusCode)
            {
                StakingInfoModel stakingInfoModel = new StakingInfoModel();
                content = await response.Content.ReadAsStringAsync();

                stakingInfoModel = JsonConvert.DeserializeObject<StakingInfoModel>(content);

                this.stakingInfo.enabled = stakingInfoModel.enabled; //stakingEnabled
                this.stakingInfo.staking = stakingInfoModel.staking; //stakingActive
                this.stakingInfo.weight = stakingInfoModel.weight; //stakingWeight
                this.stakingInfo.netStakeWeight = stakingInfoModel.netStakeWeight; //netStakingWeight
                this.awaitingMaturity = (this.unconfirmedBalance + this.confirmedBalance) - this.spendableBalance; //
                this.stakingInfo.expectedTime = stakingInfoModel.expectedTime; //expectedTime
                if (this.stakingInfo.staking)
                {
                    this.isStarting = false;
                }
                else
                {
                    this.isStopping = false;
                }
            }
            else
            {
                MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }
        }


        private void PopulateTxt()
        {

            // Connection info
            this.ConnectionStatusTxt.Text = this.connectedNodesStatus;
            this.walletGeneralInfoModel.lastBlockSyncedHeight = this.walletGeneralInfoModel.lastBlockSyncedHeight ?? 0;
            this.LastBlockSyncedHeightTxt.Text = this.walletGeneralInfoModel.lastBlockSyncedHeight.ToString();
            this.ChainTipTxt.Text = this.walletGeneralInfoModel.chainTip.ToString();
            this.ParentSyncedTxt.Text = this.percentSynced;

            this.ConnectionPercentTxt.Text = this.percentSynced;
            this.ConnectedCountTxt.Text = this.connectedNodesStatus;

            if (!this.stakingEnabled && !this.sidechainEnabled)
            {
                this.ConnectionNotifyTxt.Text = "Not Ok";
            }
            else if (this.stakingEnabled && !this.sidechainEnabled)
            {
                this.ConnectionNotifyTxt.Text = "Not Ok";
            }

            // Hybrid pow mining info
            this.HybridWeightTxt.Text = $"{this.stakingInfo.weight} XELS";
            this.CoinsAwaitingMaturityTxt.Text = $"{this.awaitingMaturity} XELS";
            this.NetworkWeightTxt.Text = $"{this.stakingInfo.netStakeWeight} XELS";
            this.ExpectedRewardTimmeTxt.Text = this.stakingInfo.expectedTime.ToString();


        }

        private void receiveButton_Click(object sender, RoutedEventArgs e)
        {
            Receive receive = new Receive(this.walletName);
            receive.Show();
            this.Close();
        }
        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            Send send = new Send(this.walletName);
            send.Show();
            this.Close();
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
        private void Hyperlink_NavigateSmartContract(object sender, RequestNavigateEventArgs e)
        {

            SmartContract sc = new SmartContract(this.walletName);
            sc.Show();
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

        private void ImportAddrButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void StopPOWMiningButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DetailButton_Click(object sender, RoutedEventArgs e)
        {
            TransactionInfo item = (TransactionInfo)((sender as Button)?.Tag as ListViewItem)?.DataContext;

            TransactionDetail td = new TransactionDetail(this.walletName, item);
            td.Show();
            this.Close();
        }


        private void GotoHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            History history = new History(this.walletName);
            history.Show();
            this.Close();
        }
    }
}
