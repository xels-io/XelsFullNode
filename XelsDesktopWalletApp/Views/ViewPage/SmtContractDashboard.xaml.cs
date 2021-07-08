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
using XelsDesktopWalletApp.Models.CommonModels;
using XelsDesktopWalletApp.Models.SmartContractModels;
using XelsDesktopWalletApp.Views.SmartContractView;

namespace XelsDesktopWalletApp.Views.ViewPage
{
    /// <summary>
    /// Interaction logic for SmtContractDashboard.xaml
    /// </summary>
    public partial class SmtContractDashboard : Page
    {
        private List<SmartContractTransactionItem> SmthistoryList = new List<SmartContractTransactionItem>();

        #region Base
        static HttpClient client = new HttpClient();
        string baseURL = URLConfiguration.BaseURL;// Common Url
        #endregion

        public SmtContractDashboard()
        {
            InitializeComponent();
        }

        public SmtContractDashboard(string selectedAddress)
        {
            InitializeComponent();
            this.DataContext = this;
           // this.walletName = GlobalPropertyModel.WalletName;
            this.lab_ActiveAddress.Text = selectedAddress;
            LoadCreateAsync();
        }

        public async void LoadCreateAsync()
        {
            await GetAddressBalanceAsync(GlobalPropertyModel.Address);
            await GetHistoryAsync(GlobalPropertyModel.Address, GlobalPropertyModel.WalletName);
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
                    this.lab_addBalance.Text = content;
                    GlobalPropertyModel.AddressBalance = content;
                }
                else
                {
                    MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
                    this.lab_addBalance.Text = "000000000000";
                    GLOBALS.AddressBalance = "000000000000";
                    GlobalPropertyModel.AddressBalance = "000000000000";
                }
            }
            catch (Exception e)
            {

                throw;
            }


            return content;
        }

        private async Task<string> GetHistoryAsync(string address, string walletName)
        {
            var content = "";
            try
            {
                string getUrl = URLConfiguration.BaseURL + $"/SmartContractWallet/historyForWPF?walletName={walletName}&&address={address}";

                try
                {
                    HttpResponseMessage response = await client.GetAsync(getUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        content = await response.Content.ReadAsStringAsync();

                        this.SmthistoryList = JsonConvert.DeserializeObject<List<SmartContractTransactionItem>>(content);
                        //if (this.SmthistoryList.Count == 0)
                        //{
                        //    var item = new SmartContractTransactionItem
                        //    {
                        //        blockHeight = 11,
                        //        hash = "dsfdsfdsfhdsjhf",
                        //        to = "sdkhkjdhfdsjfdskkdsja",
                        //        amount = 242,
                        //        transactionFee = Convert.ToDecimal(10.2),
                        //        gasFee = Convert.ToDecimal(0.2),

                        //    };
                        //    this.SmthistoryList.Add(item);
                        //    var item2 = new SmartContractTransactionItem
                        //    {
                        //        blockHeight = 34,
                        //        hash = "dsfdsfdsf434343hdsjhf",
                        //        to = "sdkhkjdhfdsjfd4333434skkdsja",
                        //        amount = 24342,
                        //        transactionFee = Convert.ToDecimal(3434),
                        //        gasFee = Convert.ToDecimal(0.2),

                        //    };
                        //    this.SmthistoryList.Add(item2);
                        //}
                        this.SmartContractHistoryList.ItemsSource = this.SmthistoryList;

                    }
                    else
                    {
                        MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);

                    }
                }
                catch (Exception e)
                {

                    throw;
                }
            }
            catch (Exception e)
            {

                throw;
            }



            return content;
        }



        private void Btn_AddressCopy_Click(object sender, RoutedEventArgs e)
        {
            string activeAddress = this.lab_ActiveAddress.Text.ToString();
            Clipboard.SetText(activeAddress);
            MessageBox.Show(activeAddress + "  COPIED");
        }

        private void CallContract_Click(object sender, RoutedEventArgs e)
        {
            this.smtpageContent.Children.Clear();
            this.smtpageContent.Children.Add(new CallContract(GlobalPropertyModel.WalletName,GlobalPropertyModel.Address,GlobalPropertyModel.AddressBalance));
        }

        private void CreateContract_Click(object sender, RoutedEventArgs e)
        {
            this.smtpageContent.Children.Clear();
            this.smtpageContent.Children.Add(new CreateContract(GlobalPropertyModel.WalletName, GlobalPropertyModel.Address, GlobalPropertyModel.AddressBalance));
        }

        private void btnAddToken_Click(object sender, RoutedEventArgs e)
        {
            this.smtpageContent.Children.Clear();
            this.smtpageContent.Children.Add(new AddToken(GlobalPropertyModel.WalletName));

        }

        private void btnIssueToken_Click(object sender, RoutedEventArgs e)
        {
            this.smtpageContent.Children.Clear();
            this.smtpageContent.Children.Add(new IssueToken(GlobalPropertyModel.WalletName, GlobalPropertyModel.Address, GlobalPropertyModel.AddressBalance));

        }

        private void btnsmtDashboard_Click(object sender, RoutedEventArgs e)
        {
            NavigationService navService = NavigationService.GetNavigationService(this);
            SmtContractDashboard page2Obj = new SmtContractDashboard(GlobalPropertyModel.Address); //Create object of Page2
            navService.Navigate(page2Obj);
        }
    }
}
