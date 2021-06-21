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
using Newtonsoft.Json;
using XelsDesktopWalletApp.Models;

namespace XelsDesktopWalletApp.Views
{
    /// <summary>
    /// Interaction logic for SmartContract.xaml
    /// </summary>
    /// 
    public static class GLOBALS
    {
        public static string Address { get; set; }
        public static string AddressBalance { get; set; }
    }
    public partial class SmartContract : Window
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


        public SmartContract()
        {
            
            InitializeComponent();
          
        }

        public SmartContract(string walletname)
        {
            InitializeComponent();
          
            this.DataContext = this;
            this.walletName = walletname;
            this.walletInfo.walletName = this.walletName;
            LoadCreateAsync();
            this.lab_addBalance.Content = GLOBALS.AddressBalance;
        }

        public async void LoadCreateAsync()
        {
            string addr = await GetUnusedReceiveAddressesAsync(this.baseURL);
            addr = FreshAddress(addr);
            GLOBALS.Address = addr;
           await GetAddressBalanceAsync(this.baseURL, addr);
        }


        private string FreshAddress(string adr)
        {
            adr = adr.Trim(new char[] { '"' });
            return adr;
        }

        private async Task<string> GetUnusedReceiveAddressesAsync(string path)
        {
            string getUrl = path + $"/wallet/unusedaddress?WalletName={this.walletInfo.walletName}&AccountName=account 0";
            var content = "";

            HttpResponseMessage response = await client.GetAsync(getUrl);


            if (response.IsSuccessStatusCode)
            {
                content = await response.Content.ReadAsStringAsync();
            }
            else
            {
                MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }

            return content;
        }

        private async Task<string> GetAddressBalanceAsync(string path, string address)
        {
            string getUrl = path + $"/SmartContractWallet/address-balance?address={address}";
            var content = "";
           
            try
            {
                HttpResponseMessage response = await client.GetAsync(getUrl);
                if (response.IsSuccessStatusCode)
                {
                    content = await response.Content.ReadAsStringAsync();
                    GLOBALS.AddressBalance = content;
                }
                else
                {
                    MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
                    GLOBALS.AddressBalance = "000000000000";


                }
            }
            catch (Exception e)
            {

                throw;
            }
           

            return content;
        }
        private async Task<string> GetHistoryAsync(string path, string address)
        {

            string getUrl = path + $"/smartContractWallet/history";
            var content = "";

            HttpResponseMessage response = await client.GetAsync(getUrl);
            if (response.IsSuccessStatusCode)
            {
                content = await response.Content.ReadAsStringAsync();
                GLOBALS.AddressBalance = content;
            }
            else
            {
                MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
                GLOBALS.AddressBalance = "000000000000";


            }

            return content;
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
            SmartContract ex = new SmartContract(this.walletName);
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
