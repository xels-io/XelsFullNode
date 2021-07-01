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
using XelsDesktopWalletApp.Models;
using XelsDesktopWalletApp.Models.CommonModels;

namespace XelsDesktopWalletApp.Views.SmartContractView
{
    /// <summary>
    /// Interaction logic for TokenManagement.xaml
    /// </summary>
    public partial class TokenManagement : UserControl
    {
        #region Base
        static HttpClient client = new HttpClient();
        string baseURL = URLConfiguration.BaseURL;// Common Url
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

        public TokenManagement()
        {
            InitializeComponent();
        }

        public TokenManagement(string walletname, string selectedAddress)
        {
            InitializeComponent();
            this.DataContext = this;
            this.walletName = walletname;
            this.walletInfo.walletName = this.walletName;
            this.lab_ActiveAddress.Content = selectedAddress;
            LoadCreateAsync();
        }


        public async void LoadCreateAsync()
        {
            string activeAddress = this.lab_ActiveAddress.Content.ToString();
            await GetAddressBalanceAsync(activeAddress);
            //await GetHistoryAsync(GLOBALS.Address, this.walletName);
        }

        private async Task<string> GetAddressBalanceAsync(string address)
        {
            string getUrl = URLConfiguration.BaseURL + $"/SmartContractWallet/address-balance?address={address}";
            var content = "";
            try
            {
                HttpResponseMessage response = await client.GetAsync(getUrl);
                if (response.IsSuccessStatusCode)
                {
                    content = await response.Content.ReadAsStringAsync();
                    this.lab_addBalance.Content = content;
                }
                else
                {
                    MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
                    this.lab_addBalance.Content = "000000000000";
                    GLOBALS.AddressBalance = "000000000000";
                }
            }
            catch (Exception e)
            {

                throw;
            }


            return content;
        }

        private void Btn_AddToken_Click(object sender, RoutedEventArgs e)
        {
            this.tokenManagementPageContant.Children.Add(new AddToken(this.walletName));
        }

        
        private void Button_CrateTokenClick(object sender, RoutedEventArgs e)
        {
            string activeAddress = this.lab_ActiveAddress.Content.ToString();
            string balance = this.lab_addBalance.Content.ToString();
            this.tokenManagementPageContant.Children.Add(new IssueToken(this.walletName,activeAddress,balance));
        }

        

        private void Button_CopyClick(object sender, RoutedEventArgs e)
        {
            string activeAddress = this.lab_ActiveAddress.Content.ToString();
            Clipboard.SetText(activeAddress);
            MessageBox.Show(activeAddress + "  COPIED");
        }
    }
}
