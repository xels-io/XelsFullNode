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
    /// Interaction logic for SmartContractMain.xaml
    /// </summary>
    public partial class SmartContractMain : Window
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
        public SmartContractMain()
        {
            InitializeComponent();
        }

        public SmartContractMain(string walletname)
        {
            InitializeComponent();

            this.DataContext = this;
            this.walletName = walletname;
            this.walletInfo.walletName = this.walletName;
           
            // LoadCreateAsync();
        }

        private void useAddressBtn_Click(object sender, RoutedEventArgs e)
        {

            string selectedAddress = this.selectaddress.SelectionBoxItem.ToString();

            SmartContractDashboard scm = new SmartContractDashboard(this.walletName,selectedAddress);
            this.Content = scm;
        }
        private void dashboardBtn_Click(object sender, RoutedEventArgs e)
        {
            Dashboard mw = new Dashboard();
            this.Hide();
            mw.ShowDialog();
            this.Close();
        }

        private async Task<string> GetAccountAddressesAsync(string walletName)
        {

            string getUrl = this.baseURL + $"/SmartContractWallet/account-addresses?walletName={walletName}";
            var content = "";

            HttpResponseMessage response = await client.GetAsync(getUrl);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                IEnumerable<string> address = JsonConvert.DeserializeObject<IEnumerable<string>>(jsonString);
            }
            else
            {
                MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }

            return content;
        }

    }
}
