using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for ReceiveShowall.xaml
    /// </summary>
    public partial class ReceiveShowall : Window
    {
        #region Base
        static HttpClient client = new HttpClient();
        string baseURL = "http://localhost:37221/api";
        #endregion

        private readonly WalletInfo walletInfo = new WalletInfo();
        private UsedAddresses selectedUsedWalletInfo = new UsedAddresses();
        private UnusedAddresses selectedUnusedWalletInfo = new UnusedAddresses();
        private ChangeAddresses selectedChangeWalletInfo = new ChangeAddresses();
        private ReceiveWalletArray addresses = new ReceiveWalletArray();
        private ReceiveWalletArray rcvWalletListsArray = new ReceiveWalletArray();

        private List<AllAddresses> allAddressesList = new List<AllAddresses>();
        private List<UsedAddresses> usedAddressesList = new List<UsedAddresses>();
        private List<UnusedAddresses> unusedAddressesList = new List<UnusedAddresses>();
        private List<ChangeAddresses> changeAddressesList = new List<ChangeAddresses>();

        #region Needed property

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
        public UsedAddresses SelectedUsedWalletInfo
        {
            get
            {
                return this.selectedUsedWalletInfo;
            }
            set
            {
                this.selectedUsedWalletInfo = value;
            }
        }
        public UnusedAddresses SelectedUnusedWalletInfo
        {
            get
            {
                return this.selectedUnusedWalletInfo;
            }
            set
            {
                this.selectedUnusedWalletInfo = value;
            }
        }
        public ChangeAddresses SelectedChangeWalletInfo
        {
            get
            {
                return this.selectedChangeWalletInfo;
            }
            set
            {
                this.selectedChangeWalletInfo = value;
            }
        }

        //public SelectedAddress Address
        //{
        //    get
        //    {
        //        return this.address;
        //    }
        //    set
        //    {
        //        this.address = value;
        //        //OnPropertyChanged("Addr");
        //    }
        //}
        private string address;
        public string Address
        {
            get
            {
                return this.address;
            }
            set
            {
                this.address = value;
                //OnPropertyChanged("Addr");
            }
        }
        #endregion

        #region Lists
        public List<UsedAddresses> UsedAddressesList
        {
            get
            {
                return this.usedAddressesList;
            }
            set
            {
                this.usedAddressesList = value;
            }
        }

        public List<UnusedAddresses> UnusedAddressesList
        {
            get
            {
                return this.unusedAddressesList;
            }
            set
            {
                this.unusedAddressesList = value;
            }
        }

        public List<ChangeAddresses> ChangeAddressesList
        {
            get
            {
                return this.changeAddressesList;
            }
            set
            {
                this.changeAddressesList = value;
            }
        }
        #endregion

        public ReceiveShowall()
        {
            InitializeComponent();
        }

        public ReceiveShowall(string walletname)
        {
            InitializeComponent();
            this.DataContext = this;

            this.walletName = walletname;
            this.walletInfo.walletName = this.walletName;

            LoadCreate();

            this.lvDataBinding.ItemsSource = this.unusedAddressesList;
        }

        public async void LoadCreate()
        {
            await GetAPIAsync(this.baseURL);

        }


        private void Hyperlink_NavigateReceive(object sender, RequestNavigateEventArgs e)
        {
            Receive receive = new Receive(this.walletName);
            receive.Show();
            this.Close();
        }


        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            Dashboard dashboard = new Dashboard(this.walletName);
            dashboard.Show();
            this.Close();
        }

        private async Task GetAPIAsync(string path)
        {
            string getUrl = path + $"/wallet/addresses?WalletName={this.walletInfo.walletName}&AccountName=account 0";
            var content = "";

            HttpResponseMessage response = await client.GetAsync(getUrl);


            if (response.IsSuccessStatusCode)
            {
                content = await response.Content.ReadAsStringAsync();

                this.addresses = JsonConvert.DeserializeObject<ReceiveWalletArray>(content);
            }
            else
            {
                MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }

            ListConvert(this.addresses);

        }


        private void ListConvert(ReceiveWalletArray _content)
        {
            ReceiveWalletArray convertedList = new ReceiveWalletArray();

            this.rcvWalletListsArray.addresses = _content.addresses;
            var mylist = this.rcvWalletListsArray.addresses;
            AllAddresses allAddresses = new AllAddresses();


            foreach (ReceiveWalletStatus address in mylist)
            {
                if (address.IsUsed)
                {
                    UsedAddresses usedAddresses = new UsedAddresses();
                    usedAddresses.address = address.Address;
                    this.usedAddressesList.Add(usedAddresses);
                }
                else if (address.IsChange)
                {
                    ChangeAddresses changeAddresses = new ChangeAddresses();
                    changeAddresses.address = address.Address;
                    this.changeAddressesList.Add(changeAddresses);
                }
                else
                {
                    UnusedAddresses unusedAddresses = new UnusedAddresses();
                    unusedAddresses.address = address.Address;
                    this.unusedAddressesList.Add(unusedAddresses);
                }
            }

        }

        private void btn_copy_Click(object sender, RoutedEventArgs e)
        {
            var item = ((sender as Button)?.Tag as ListViewItem)?.DataContext;

            if (item != null)
            {

                var theAddress = item;
                if (theAddress.GetType() == typeof(UnusedAddresses))
                {
                    this.selectedUnusedWalletInfo = (UnusedAddresses)theAddress;

                    if (this.selectedUnusedWalletInfo.address != null)
                    {
                        Clipboard.SetText(this.selectedUnusedWalletInfo.address);
                        this.lvDataBinding.ItemsSource = this.UnusedAddressesList;
                    }
                }
                else if (theAddress.GetType() == typeof(UsedAddresses))
                {
                    this.selectedUsedWalletInfo = (UsedAddresses)theAddress;

                    if (this.selectedUsedWalletInfo.address != null)
                    {
                        Clipboard.SetText(this.selectedUsedWalletInfo.address);
                        this.lvDataBinding.ItemsSource = this.usedAddressesList;
                    }
                }
                else if (theAddress.GetType() == typeof(ChangeAddresses))
                {
                    this.selectedChangeWalletInfo = (ChangeAddresses)theAddress;

                    if (this.selectedChangeWalletInfo.address != null)
                    {
                        Clipboard.SetText(this.selectedChangeWalletInfo.address);
                        this.lvDataBinding.ItemsSource = this.changeAddressesList;
                    }
                }
            }


        }

        private void XELS_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SELS_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BELS_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UnusedAddr_Button_Click(object sender, RoutedEventArgs e)
        {
            this.lvDataBinding.ItemsSource = this.unusedAddressesList;
        }

        private void ChangeAddr_Button_Click(object sender, RoutedEventArgs e)
        {
            this.lvDataBinding.ItemsSource = this.changeAddressesList;
        }

        private void UsedAddr_Button_Click(object sender, RoutedEventArgs e)
        {
            this.lvDataBinding.ItemsSource = this.usedAddressesList;
        }

    }
}
