using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        ObservableCollection<TokenRetrieveModel> tokenlist = new ObservableCollection<TokenRetrieveModel>();

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
        TokenModel TokenModel = new TokenModel();

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
            this.row2.Height = new GridLength(0);
            this.row3.Height = new GridLength(0);
            this.row4.Height = new GridLength(0);
            this.row5.Height = new GridLength(0);

            // this.gridAllPage.Height = 200;

            //Angular er project a static diya silo

            List<TokenModel> objtokenList = new List<TokenModel>
            {
                new TokenModel { Ticker = "MEDI", Address = "CUwkBGkXrQpMnZeWW2SpAv1Vu9zPvjWNFS",Name = "Mediconnect", Decimals="8", DropDownValue="(MEDI)-CUwkBGkXrQpMnZeWW2SpAv1Vu9zPvjWNFS-(Mediconnect)"},
                new TokenModel { Ticker = "Custom", Address = "custom", Name = "custom",DropDownValue="custom"},
                
            };
            this.tokens.AddRange(objtokenList);
            //LoadAsync();
            AddTokenList();
        }


        public void ResetPage()
        {
            this.txtTokenSymbol.Text = "";
            this.txtTokenContractAddress.Text = "";
            this.tokenNametxt.Text = "";
            this.tokenDecimalTxt.Text = "0";
        }

            public  void AddTokenList()
        {
            string retMsg = "";

            try
            {
                string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

                string tokenFilePath = System.IO.Path.Combine(CurrentDirectory, @"..\..\..\Token\TokenFile.txt");
                string path = System.IO.Path.GetFullPath(tokenFilePath);

                if (File.Exists(path))
                {
                    using (StreamReader r = new StreamReader(path))
                    {
                        string json = r.ReadToEnd();


                        string concateData = '[' + json + ']';
                        this.tokenlist = JsonConvert.DeserializeObject<ObservableCollection<TokenRetrieveModel>>(concateData);
                        this.DataGrid1.ItemsSource = this.tokenlist;
                    }
                }
                else
                {
                    this.tokenlist = new ObservableCollection<TokenRetrieveModel>();
                    this.DataGrid1.ItemsSource = this.tokenlist;
                }
            }
            catch (Exception e)
            {
                retMsg = e.Message.ToString();

            }

        }

        //private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    this.Visibility = Visibility.Collapsed;
        //    TokenManagement tokenManagement = new TokenManagement(this.walletName, GLOBALS.Address);
        //}


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

                this.rowTokenContractAddress.Height = new GridLength(25);
                this.rowTokenSymbol.Height = new GridLength(25);
                this.rowTokenName.Height = new GridLength(25);
                this.rowTokenDecimal.Height = new GridLength(25);
                // this.gridAllPage.Height = 380;
                this.row2.Height = new GridLength(5);
                this.row3.Height = new GridLength(5);
                this.row4.Height = new GridLength(5);
                this.row5.Height = new GridLength(5);

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
                //this.gridAllPage.Height = 200;

                this.row2.Height = new GridLength(0);
                this.row3.Height = new GridLength(0);
                this.row4.Height = new GridLength(0);
                this.row5.Height = new GridLength(0);


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
                ResetPage();
                AddTokenList();


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
                ResetPage();
                AddTokenList();

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

        
        private void btn_Delete_Token_Click(object sender, RoutedEventArgs e)
        {
            DataGrid dataGrid = this.DataGrid1;
            DataGridRow Row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
            DataGridCell RowAndColumn = (DataGridCell)dataGrid.Columns[3].GetCellContent(Row).Parent;
            string CellValue = ((TextBlock)RowAndColumn.Content).Text;
            DeleteToken(CellValue);
            AddTokenList();
        }

        public string DeleteToken(string address)
        {
            string msg = "";
            string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string tokenFilePath = System.IO.Path.Combine(CurrentDirectory, @"..\..\..\Token\TokenFile.txt");
            string path = System.IO.Path.GetFullPath(tokenFilePath);

            List<TokenRetrieveModel> tokenlist = new List<TokenRetrieveModel>();

            if (File.Exists(path))
            {
                using (StreamReader r = new StreamReader(path))
                {
                    string json = r.ReadToEnd();
                    string concateData = '[' + json + ']';
                    tokenlist = JsonConvert.DeserializeObject<List<TokenRetrieveModel>>(concateData);

                    foreach (var item in tokenlist)
                    {
                        if (item.Address == address)
                        {
                            tokenlist.Remove(item);
                            break;
                        }
                    }

                }

                File.Delete(path);
                if (tokenlist.Count > 0)
                {
                    string JSONresult = JsonConvert.SerializeObject(tokenlist, Formatting.Indented);

                    string FinalResult = JSONresult.TrimStart('[').TrimEnd(']').TrimStart().TrimEnd();
                    File.Create(path).Dispose();

                    using (var sw = new StreamWriter(path, true))
                    {
                        sw.WriteLine(FinalResult.ToString());
                        sw.WriteLine(",");
                        sw.Flush();
                        sw.Close();

                    }


                }

            }
            return msg;
        }

    }
}
