using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
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
using XelsDesktopWalletApp.Models.CommonModels;
using XelsDesktopWalletApp.Models.SmartContractModels;

namespace XelsDesktopWalletApp.Views.SmartContractView
{
    public static class GLOBALS
    {
        public static string Address { get; set; }
        public static string AddressBalance { get; set; }
    }
    public partial class SmartContractDashboard : Page
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
        public SmartContractDashboard()
        {
            InitializeComponent();
        }

        public SmartContractDashboard(string walletname, string selectedAddress)
        {
            InitializeComponent();
            this.DataContext = this;
            this.walletName = walletname;
            this.walletInfo.walletName = this.walletName;
            this.lab_ActiveAddress.Content = selectedAddress;
            GLOBALS.Address = selectedAddress;
            LoadCreateAsync();
        }

        public async void LoadCreateAsync()
        {
            await GetAddressBalanceAsync(this.baseURL, GLOBALS.Address);
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


        private async Task<string> GetTrasactionHistoryAsync(string walletName, string address)
        {
            try
            {
                string content = "";
                client.BaseAddress = new Uri(this.baseURL);
                client.DefaultRequestHeaders.Accept.Add(
                   new MediaTypeWithQualityHeaderValue("application/json"));

                var reuestModel = new SmtGetHistoryRequest();
                reuestModel.WalletName = walletName;
                reuestModel.Address = address;



                HttpResponseMessage response = client.GetAsync("/SmartContractWallet/history").Result;

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    var finalData = JsonConvert.DeserializeObject<IEnumerable<SmartContractTransactionItem>>(data);

                    // grdEmployee.ItemsSource = data;
                }
                else
                {
                    MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);

                }
                return content;
            }
            catch (Exception e)
            {

                throw;
            }
        }

        private void dashboardBtn_Click(object sender, RoutedEventArgs e)
        {
            Dashboard mw = new Dashboard(this.walletName);
            mw.ShowDialog();

        }
    }

}