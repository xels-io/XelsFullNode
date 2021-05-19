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
//using System.Web.Script.Serialization;

namespace XelsDesktopWalletApp.Views
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {

        static HttpClient client = new HttpClient();
        string baseURL = "http://localhost:37221/api/wallet";
        List<string> listData;
        WalletLoadRequest walletLoadRequestModel = new WalletLoadRequest();

        public class ComboData
        {
            public ComboData()
            {
                this.Name = "Empty Name";
                this.Names = null;
            }
            public string Name { get; set; }
            public List<string> Names { get; set; }

        }
        ComboData comboData = new ComboData();

        //public class ComboData
        //{
        //    public int Id { get; set; }
        //    public string Value { get; set; }
        //}

        public Login()
        {
            InitializeComponent();
            LoadLogin();
            //ItemsSource = this.walletLoadRequestModel.walletNames;
            this.walletLoadRequestModel.walletname = "--select wallet--";
            comboLoad();
            //this.comboData.Name = this.walletLoadRequestModel.walletname;
            //this.comboData.Names = this.walletLoadRequestModel.walletNames;
        }

        private void comboLoad()
        {
            //this.cmbWalletList.DisplayMemberPath = this.walletLoadRequestModel.walletname;
            //this.cmbWalletList.SelectedValuePath = this.walletLoadRequestModel.walletname;
            //this.cmbWalletList.ItemsSource = this.walletLoadRequestModel.walletNames;
            //this.cmbWalletList.Text = "Choose Wallet";
            //this.cmbWalletList.SelectedIndex = 0;

            //ItemsSource = "{Binding Path=walletLoadRequestModel.walletNames}"
            //  DisplayMemberPath = "walletLoadRequestModel.walletname"
            //  SelectedValuePath = "walletLoadRequestModel.walletname"
            //  SelectedValue = "{Binding Path=walletLoadRequestModel.walletname}" />
        }

        public async void LoadLogin()
        {
            //this.walletLoadRequestModel = await GetAPIAsync(this.baseURL);
            await GetAPIAsync(this.baseURL);
        }


        private async Task GetAPIAsync(string path)
        {
            string getUrl = path + "/list-wallets";
            var content = "";
            List<WalletLoadRequest> model = null ;

            HttpResponseMessage response = await client.GetAsync(getUrl);
            if (response.IsSuccessStatusCode)
            {
                content = await response.Content.ReadAsStringAsync();
                //model = JsonConvert.DeserializeObject<List<WalletLoadRequest>>(content);
            }
            else
            {
                MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }

            //return model;

            // var jsonData = content;

            //// name deserializedName = JsonConvert.DeserializeObject<name>(jsonData);

            // //JavaScriptSerializer ser = new JavaScriptSerializer();

            // nameList myNames = JavaScriptSerializer.ser.Deserialize<nameList>(jsonData);

            // return ser.Serialize(myNames);



            converted(content);
        }


        private void converted(string data)
        {
            string[] rowData = data.Split(':');
            string[] rowDataMain = rowData[1].Split('\"');
            this.listData = rowDataMain.ToList();
            //this.walletLoadRequestModel.walletNames = rowDataMain.ToList();

            foreach (var i in rowDataMain)
            {
                this.walletLoadRequestModel.walletNames.Add(i);
            }

            for (int i = 0; i < rowDataMain.Length; i++)
            {

                if (i % 2 == 0)
                {
                    this.walletLoadRequestModel.walletNames.Remove(this.listData[i]);
                }

            }

        }


        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            CreateOrRestore cr = new CreateOrRestore();
            cr.Show();
            this.Close();
        }
        private void decryptButton_Click(object sender, RoutedEventArgs e)
        {
            Dashboard db = new Dashboard();
            db.Show();
            this.Close();
        }

    }
}
