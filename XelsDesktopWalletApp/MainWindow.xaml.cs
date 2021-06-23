using System;
using System.Collections.Generic;
using System.Linq;
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
using XelsDesktopWalletApp.Views;

namespace XelsDesktopWalletApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        static HttpClient client = new HttpClient();
        string baseURL = URLConfiguration.BaseURL;

        //public List<WalletLoadRequest> _myList { get; set; }
        private List<WalletLoadRequest> myList = new List<WalletLoadRequest>();
        private WalletLoadRequest selectedWallet = new WalletLoadRequest();

        public List<WalletLoadRequest> MyList
        {
            get
            {
                return this.myList;
            }
            set
            {
                this.myList = value;
            }
        }
        public WalletLoadRequest SelectedWallet
        {
            get
            {
                return this.selectedWallet;
            }
            set
            {
                this.selectedWallet = value;
            }
        }


        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;

            LoadLoginAsync();
        }


        public async void LoadLoginAsync()
        {
            try
            {
                await GetAPIAsync(this.baseURL);
            }
            catch (Exception e)
            {

                throw;
            }
           
        }


        private async Task GetAPIAsync(string path)
        {
            try
            {
                string getUrl = path + "/wallet/list-wallets";
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
                converted(content);
            }
            catch (Exception e)
            {

                throw;
            }
            
        }


        private void converted(string data)
        {
            string[] rowData = data.Split(':');
            string[] rowDataMain = rowData[1].Split('\"');

            foreach (var d in rowDataMain)
            {
                WalletLoadRequest wlr = new WalletLoadRequest();
                wlr.name = d;
                if (!(d.Contains("[") || d.Contains(",") || d.Contains("]")))
                {
                    this.myList.Add(wlr);

                }
            }
        }


        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            CreateOrRestore cr = new CreateOrRestore();
            cr.Show();
            this.Close();
        }



        private async void DecryptButton_ClickAsync(object sender, RoutedEventArgs e)
        {

            if (this.SelectedWallet.name != null)
            {
                this.selectedWallet.password = this.password.Password;

                string postUrl = this.baseURL + "/wallet/load/";

                HttpResponseMessage response = await client.PostAsync(postUrl, new StringContent(JsonConvert.SerializeObject(this.SelectedWallet), Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    // MessageBox.Show("Successfully logged in by " + this.SelectedWallet.name);

                    Dashboard db = new Dashboard(this.SelectedWallet.name);
                    db.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
                }

            }


        }


    }
}
