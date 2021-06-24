using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

using Newtonsoft.Json;

using XelsDesktopWalletApp.Models;
using XelsDesktopWalletApp.Models.CommonModels;

namespace XelsDesktopWalletApp.Views
{
    /// <summary>
    /// Interaction logic for TransactionDetail.xaml
    /// </summary>
    public partial class TransactionDetail : Window
    {
        #region Base
        //static HttpClient client = new HttpClient();
        string baseURL = URLConfiguration.BaseURL;// "http://localhost:37221/api";
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
            PopulateView();
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

            HttpResponseMessage response = await URLConfiguration.Client.GetAsync(getUrl);

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

        private void PopulateView()
        {
            this.TransactionIDTxt_Copyed.Visibility = Visibility.Hidden; 
            this.AmountSentLabel.Visibility = Visibility.Hidden;
            this.AmountSentTxt.Visibility = Visibility.Hidden;
            this.FeeLabel.Visibility = Visibility.Hidden;
            this.FeeTxt.Visibility = Visibility.Hidden;

            this.TypeTxt.Text = this._transaction.transactionType;
            if(this._transaction.transactionType == "rewarded" || this._transaction.transactionType == "received")
            {
                this.TotalAmountRedTxt.Content = Visibility.Hidden;
                this.TotalAmountTxt.Content = Visibility.Visible;
                this.TotalAmountTxt.Content = this._transaction.transactionAmount;
                
            }
            else if (this._transaction.transactionType == "sent" )
            {
                this.TotalAmountRedTxt.Content = Visibility.Visible;
                this.TotalAmountTxt.Content = Visibility.Hidden;
                this.TotalAmountRedTxt.Content = this._transaction.transactionAmount + this._transaction.transactionFee;
                this.AmountSentLabel.Content = Visibility.Visible;
                this.AmountSentTxt.Visibility = Visibility.Visible;
                this.AmountSentTxt.Content = this._transaction.transactionAmount;
                this.FeeLabel.Content = Visibility.Visible;
                this.FeeTxt.Content = Visibility.Visible;
                this.FeeTxt.Content = this._transaction.transactionFee;
            }

            this.DateTxt.Text = this._transaction.transactionTimestamp.ToString(); 
            this.BlockTxt.Text = "#" + this._transaction.transactionConfirmedInBlock.ToString();

            if (this._transaction.transactionConfirmedInBlock < 1)
            {
                this.ConfirmationsTxt.Text = "Unconfirmed!" ;
            }
            else if (this._transaction.transactionConfirmedInBlock >= 1)
            {
                this.ConfirmationsTxt.Text = this.confirmations.ToString();
            }
            
            this.TransactionIDTxt.Text = this._transaction.transactionId.ToString();

        }

        private void TransactionIDCopyButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(this.TransactionIDTxt.Text);
            this.TransactionIDTxt_Copyed.Visibility = Visibility.Visible;
        }


    }
}
