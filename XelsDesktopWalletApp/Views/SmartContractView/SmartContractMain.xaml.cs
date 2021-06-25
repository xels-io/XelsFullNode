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

namespace XelsDesktopWalletApp.Views.SmartContractView
{
    /// <summary>
    /// Interaction logic for SmartContractMain.xaml
    /// </summary>
    public partial class SmartContractMain : Window
    {

        private List<string> addressList = new List<string>();
        BackgroundWorker worker;
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

        public List<string> AddressList
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

                SmartContractDashboard scm = new SmartContractDashboard(this.walletName, selectedAddress);
                this.Content = scm;
                this.imgCircle.Visibility = Visibility.Visible; //Make loader visible
                this.useAddressBtn.IsEnabled = false;
            }
            else
            {
                MessageBox.Show("Select Address", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                this.selectaddress.Focus();
                this.imgCircle.Visibility = Visibility.Collapsed; //Make loader visible
                this.useAddressBtn.IsEnabled = true;
            }
            
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
                foreach (var add in address)
                {
                    this.addressList.Add(add);
                }
                
            }
            else
            {
                MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }

            return content;
        }





        //private void Worker_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    long sum = 0;
        //    long total = 100000;
        //    for (long i = 1; i <= total; i++)
        //    {
        //        sum += i;
        //        int percentage = Convert.ToInt32(((double)i / total) * 100);

        //        this.Dispatcher.Invoke(new System.Action(() =>
        //        {
        //            this.worker.ReportProgress(percentage);
        //        }));
        //    }
        //    MessageBox.Show("Sum: " + sum);
        //}

        //private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    this.imgCircle.Visibility = Visibility.Collapsed;
        //    this.PerformTask.IsEnabled = true;
        //}

        //private void PerformTask_Click(object sender, RoutedEventArgs e)
        //{
        //    this.imgCircle.Visibility = Visibility.Visible; //Make Progressbar visible
        //    this.PerformTask.IsEnabled = false; //Disabling the button
        //    this.worker = new BackgroundWorker(); //Initializing the worker object
        //    this.worker.DoWork += Worker_DoWork; //Binding Worker_DoWork method
        //    this.worker.WorkerReportsProgress = true; //telling the worker that it supports reporting progress
        //    this.worker.RunWorkerCompleted += Worker_RunWorkerCompleted; //Binding worker_RunWorkerCompleted method
        //    this.worker.RunWorkerAsync(); //Executing the worker
        //}
    }
}
