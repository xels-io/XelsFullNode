using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using XelsDesktopWalletApp.Views.SmartContractView;

namespace XelsDesktopWalletApp.Views.ViewPage
{
    /// <summary>
    /// Interaction logic for SmtAddressSelection.xaml
    /// </summary>
    public partial class SmtAddressSelection : Page
    {
        public class AddressModel
        {
            public string Address { get; set; }
        }

        private List<AddressModel> addressList = new List<AddressModel>();
        private AddressModel selectedAddress = new AddressModel();

        #region Base
        static HttpClient client = new HttpClient();
        string baseURL = URLConfiguration.BaseURL;// Common Url
        #endregion
        #region Wallet Info
        private readonly WalletInfo walletInfo = new WalletInfo();

        private string walletName;
        private object imgCircle;

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

        public List<AddressModel> AddressList
        {
            get
            {
                return this.addressList;
            }
            set
            {
                this.addressList = value;
            }
        }
        public string Selectedaddress { get; set; }

        public AddressModel SelectedAddress
        {
            get
            {
                return this.selectedAddress;
            }
            set
            {
                this.selectedAddress = value;
            }
        }


        public SmtAddressSelection()
        {
            InitializeComponent();
        }

        public SmtAddressSelection(string walletname)
        {
            InitializeComponent();
            this.DataContext = this;
            this.walletName = walletname;
            this.walletInfo.walletName = this.walletName;

            LoadAsync();
        }
        public async void LoadAsync()
        {
            try
            {
                await GetAccountAddressesAsync(this.walletName);
            }
            catch (Exception e)
            {

                throw;
            }

        }

        private void useAddressBtn_Click(object sender, RoutedEventArgs e)
        {

            string selectedAddress = this.selectaddress.SelectionBoxItem.ToString();
            if (selectedAddress != "")
            {
                NavigationService navService = NavigationService.GetNavigationService(this);
                SmtContractDashboard page2Obj = new SmtContractDashboard(); //Create object of Page2
                navService.Navigate(page2Obj);
                // this.imgCircle.Visibility = Visibility.Visible; //Make loader visible
                //  this.useAddressBtn.IsEnabled = false;
            }
            else
            {
                MessageBox.Show("Select Address", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                this.selectaddress.Focus();
                //this.imgCircle.Visibility = Visibility.Collapsed; //Make loader visible
                this.useAddressBtn.IsEnabled = true;
            }

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
                foreach (var add in address)
                {
                    AddressModel addressModel = new AddressModel();
                    addressModel.Address = add;
                    this.addressList.Add(addressModel);
                }

            }
            else
            {
                MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }

            return content;
        }

    }
}
