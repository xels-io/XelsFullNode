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
using Newtonsoft.Json.Linq;
using XelsDesktopWalletApp.Models;

namespace XelsDesktopWalletApp.Views
{
    /// <summary>
    /// Interaction logic for AddressBook.xaml
    /// </summary>
    public partial class AddressBook : Window
    {

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
        static HttpClient client = new HttpClient();
        string baseURL = "http://localhost:37221/api";
        //AddressLabel[] addresses = null;
        List<AddressLabel> addresses = new List<AddressLabel>();

        public AddressBook()
        {
            InitializeComponent();

            this.DataContext = this;
        }
        public AddressBook(string walletname)
        {
            InitializeComponent();

            this.DataContext = this;


            this.walletName = walletname;
            LoadAddresses();

        }

        public async void LoadAddresses()
        {
            this.addresses = await GetAPIAsync(this.baseURL);
        }

        private async Task<List<AddressLabel>> GetAPIAsync(string path)
        {
            string getUrl = path + "/AddressBook";
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

            List<AddressLabel> addresslist = ProcessAddresses(content);
            return addresslist;
        }


        public List<AddressLabel> ProcessAddresses(string _content)
        {
            JObject json = JObject.Parse(_content);

            AddressLabel addresslist = new AddressLabel();

            //addresslist.label = json.addresses.label;

            return null ;
        }

        private void Show_Click(object sender, RoutedEventArgs e)
        {
            Send send = new Send();
            send.Show();
            this.Close();
            //MyPopup.IsOpen = true;
        }

        private void receiveButton_Click(object sender, RoutedEventArgs e)
        {
            Receive receive = new Receive();
            receive.Show();
            this.Close();
        }
        private void createButton_Click(object sender, RoutedEventArgs e)
        {

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
        private void Hyperlink_NavigateLogout(object sender, RequestNavigateEventArgs e)
        {
            LogoutConfirm lc = new LogoutConfirm(this.walletName);
            lc.Show();
            this.Close();
        }

        private void Hyperlink_NavigateAddressBook(object sender, RequestNavigateEventArgs e)
        {
            AddressBook ex = new AddressBook(this.walletName);
            ex.Show();
            this.Close();
        }


        private void Hyperlink_NavigateAdvanced(object sender, RequestNavigateEventArgs e)
        {
            Advanced adv = new Advanced(this.walletName);
            adv.Show();
            this.Close();
        }


        private void Hyperlink_NavigateAddAddress(object sender, RequestNavigateEventArgs e)
        {
            AddressBookAddNew addaddr = new AddressBookAddNew(this.walletName);
            addaddr.Show();
            this.Close();
        }

    }
}
