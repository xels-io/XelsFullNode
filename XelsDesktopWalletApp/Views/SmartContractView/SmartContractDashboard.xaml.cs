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
using XelsDesktopWalletApp.Models.CommonModels;

namespace XelsDesktopWalletApp.Views.SmartContractView
{
    /// <summary>
    /// Interaction logic for SmartContractDashboard.xaml
    /// </summary>
    /// 
    public static class GLOBALS
    {
        public static string Address { get; set; }
        public static string AddressBalance { get; set; }
    }
    public partial class SmartContractDashboard : Page
    {

        #region Base
        static HttpClient client = new HttpClient();
        string baseURL = URLConfiguration.BaseURLMain;// Common Url
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

        public SmartContractDashboard(string walletname,string selectedAddress)
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
            string getUrl = path + $"/smartContractWallet/address-balance?address={address}";
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
        private void dashboardBtn_Click(object sender, RoutedEventArgs e)
        {
            Dashboard mw = new Dashboard(this.walletName);
            mw.ShowDialog();
            //Dashboard mainWindow = Application.Current.Da as Dashboard;
            //mainWindow.Visibility = Visibility.Visible;
            //Window win = (Window)this.Parent;
            //win.Close();
        }
    }
}
