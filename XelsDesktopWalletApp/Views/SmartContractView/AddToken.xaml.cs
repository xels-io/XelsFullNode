using System;
using System.Collections.Generic;
using System.IO;
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
using XelsDesktopWalletApp.Models.SmartContractModels;

namespace XelsDesktopWalletApp.Views.SmartContractView
{
    /// <summary>
    /// Interaction logic for AddToken.xaml
    /// </summary>
    public partial class AddToken : UserControl
    {

        private List<TokenModel> tokens = new List<TokenModel>();

        string tokenVal;
        string address;
        string ticker;
        string decimals;
        string name;

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

        public List<TokenModel> Tokens
        {
            get
            {
                return this.tokens;
            }
            set
            {
                this.tokens = value;
            }
        }

        public AddToken()
        {
            InitializeComponent();
           
        }

        public AddToken(string walletname)
        {
            InitializeComponent();

            this.DataContext = this;
            this.walletName = walletname;
            this.walletInfo.walletName = this.walletName;


            this.rowTokenContractAddress.Height = new GridLength(0);
            this.rowTokenSymbol.Height = new GridLength(0);
            this.rowTokenName.Height = new GridLength(0);
            this.rowTokenDecimal.Height = new GridLength(0);
            this.gridAllPage.Height = 200;

            //Angular er project a static diya silo

            List<TokenModel> objtokenList = new List<TokenModel>
            {
                new TokenModel { Ticker = "MEDI", Address = "CUwkBGkXrQpMnZeWW2SpAv1Vu9zPvjWNFS",Name = "Mediconnect", Decimals="8", DropDownValue="(MEDI)-CUwkBGkXrQpMnZeWW2SpAv1Vu9zPvjWNFS-(Mediconnect)"},
                new TokenModel { Ticker = "Custom", Address = "custom", Name = "custom",DropDownValue="custom"},
                
            };
            this.tokens.AddRange(objtokenList);
            //LoadAsync();
        }
        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
            TokenManagement tokenManagement = new TokenManagement(this.walletName, GLOBALS.Address);
        }
        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;

        }

        private void token_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TokenModel objtokenModel = this.token.SelectedItem as TokenModel;

            string selectedVlaue = objtokenModel.DropDownValue;
            if (selectedVlaue=="custom")
            {
                this.txtTokenContractAddress.Text = "custom";
                this.txtTokenSymbol.Text = "custom";
                this.tokenNametxt.Text = "custom";
                this.tokenDecimalTxt.Text = "0";

                this.rowTokenContractAddress.Height = new GridLength(50);
                this.rowTokenSymbol.Height = new GridLength(40);
                this.rowTokenName.Height = new GridLength(40);
                this.rowTokenDecimal.Height = new GridLength(40);
                this.gridAllPage.Height = 380;
               
            }
            else
            {
                this.ticker = objtokenModel.Ticker;
                this.address = objtokenModel.Address;
                this.name = objtokenModel.Name;
                this.decimals = objtokenModel.Decimals;


                this.txtTokenContractAddress.Text = "";
                this.txtTokenSymbol.Text = "";
                this.tokenNametxt.Text = "";
                this.tokenDecimalTxt.Text = "";

                this.rowTokenContractAddress.Height = new GridLength(0);
                this.rowTokenSymbol.Height = new GridLength(0);
                this.rowTokenName.Height = new GridLength(0);
                this.rowTokenDecimal.Height = new GridLength(0);
                this.gridAllPage.Height = 200;

            }
           

        }

        public async Task<string> SaveTokenasync(TokenModel tokenModel)
        {
            string msg = "";

            //string path = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)
            // + "\\TokenFile.json";
            string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string tokenFilePath = System.IO.Path.Combine(CurrentDirectory, @"..\..\..\Token\TokenFile.txt");
            string path = System.IO.Path.GetFullPath(tokenFilePath);
            string validcheck = addTokenValidation(tokenModel, path);
            if (validcheck =="")
            {
                string JSONresult = JsonConvert.SerializeObject(tokenModel, Formatting.Indented);

                if (!File.Exists(path))
                {
                    File.Create(path).Dispose();
                }

                using (var sw = new StreamWriter(path, true))
                {
                    sw.WriteLine(JSONresult.ToString());
                    sw.WriteLine(",");
                    sw.Close();
                    msg = "SUCCESS";
                }
            }
            else
            {
                msg = validcheck;
            }
            return msg;
        }

        private void btn_AddTokenSubmit_Click(object sender, RoutedEventArgs e)
        {
            string msg = "";
            TokenModel objtokenModel = this.token.SelectedItem as TokenModel;
          
            string selectedVlaue = objtokenModel.DropDownValue;
            if (selectedVlaue== "custom")
            {
                var objSaveToken = new TokenModel
                {
                    Ticker = this.txtTokenSymbol.Text,
                    Address = this.txtTokenContractAddress.Text,
                    Name= this.tokenNametxt.Text,
                    Decimals =this.tokenDecimalTxt.Text,
                    Balance="0",
                };

                var result =  SaveTokenasync(objSaveToken);
                msg = result.Result;
                MessageBox.Show("Message - " + msg);
                this.Visibility = Visibility.Collapsed;
                TokenManagement tm = new TokenManagement();
                tm.tokenManagementPageContant_Loaded(null, null);


            }
            else
            {
                var objSaveToken = new TokenModel
                {
                    Ticker = objtokenModel.Ticker,
                    Address = objtokenModel.Address,
                    Name = objtokenModel.Name,
                    Decimals = objtokenModel.Decimals,
                    Balance = "0",
                };
                var result = SaveTokenasync(objSaveToken);
                msg = result.Result;
                MessageBox.Show("Message - " + msg);
                this.Visibility = Visibility.Collapsed;
                TokenManagement tm = new TokenManagement();
                tm.tokenManagementPageContant_Loaded(null, null);

            }

        }

        private string addTokenValidation(TokenModel tokenModel,string path)
        {
            string retMsg = "";
            List<TokenRetrieveModel> tokenlist = new List<TokenRetrieveModel>();
            try
            {
                if (File.Exists(path))
                {
                    using (StreamReader r = new StreamReader(path))
                    {
                        string json = r.ReadToEnd();
                        string concateData = '[' + json + ']';
                       // TokenRetrieveModel[] objsdsf = JsonConvert.DeserializeObject<TokenRetrieveModel[]>(concateData);  //same kaj kore jason convdrt
                        tokenlist = JsonConvert.DeserializeObject<List<TokenRetrieveModel>>(concateData);

                        foreach (var item in tokenlist)
                        {
                            if (item.Address == tokenModel.Address.Trim() || item.Ticker == tokenModel.Ticker.Trim())
                            {
                                retMsg = tokenModel.Address + "- is already added.";
                                return retMsg;
                            }
                        }

                    }
                }
            }
            catch (Exception e)
            {
                retMsg = e.Message.ToString();
                return retMsg;
            }
           
            return retMsg;
        }

       
    }
}
