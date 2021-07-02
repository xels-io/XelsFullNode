using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for IssueToken.xaml
    /// </summary>
    public partial class IssueToken : UserControl
    {

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

        long gasCallLimitMinimum = 10000;
        long gasCallRecommendedLimit = 50000;

        long gasCreateLimitMinimum = 12000;
        long gasCreateTokenLimitMinimum = 15000;
        long gasLimitMaximum = 250000;
        long gasPriceMinimum = 1;
        long gasPriceMaximum = 10000;

        long amountA = 0;
        double feeAmt = 0.001;
        double gasPrice = 100;
        long gasLimit = 15000;

        long decimals = 0;
        long totalSupply = 21 * 1000 * 1000;
        string tokenName = "My token";
        string tokenSymbol = "MTK";


        string newTokenByteCode = "4D5A90000300000004000000FFFF0000B800000000000000400000000000000000000000000000000000000000000000000000000000000000000000800000000E1FBA0E00B409CD21B8014CCD21546869732070726F6772616D2063616E6E6F742062652072756E20696E20444F53206D6F64652E0D0D0A2400000000000000504500004C010200E1F046E10000000000000000E00022200B013000000E00000002000000000000522C0000002000000040000000000010002000000002000004000000000000000400000000000000006000000002000000000000030040850000100000100000000010000010000000000000100000000000000000000000002C00004F000000000000000000000000000000000000000000000000000000004000000C000000E42B00001C0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000200000080000000000000000000000082000004800000000000000000000002E74657874000000580C000000200000000E000000020000000000000000000000000000200000602E72656C6F6300000C000000004000000002000000100000000000000000000000000000400000420000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000342C000000000000480000000200050050230000940800000100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000C20203280500000A0204280700000602052805000006020E0428030000060202280600000A6F0700000A0428090000062A4602280800000A72010000706F0900000A2A4A02280800000A7201000070036F0A00000A2A4602280800000A720F0000706F0900000A2A4A02280800000A720F000070036F0A00000A2A4602280800000A72190000706F0B00000A2A4A02280800000A7219000070036F0C00000A2A7202280800000A7231000070038C08000001280D00000A6F0B00000A2A7602280800000A7231000070038C08000001280D00000A046F0C00000A2A0013300400A600000001000011042D34021201FE1503000002120102280600000A6F0700000A7D010000041201037D020000041201166A7D0300000407280100002B172A0202280600000A6F0700000A28080000060A06043402162A0202280600000A6F0700000A0604DB280900000602030203280800000604D72809000006021201FE1503000002120102280600000A6F0700000A7D010000041201037D020000041201047D0300000407280100002B172A000013300500AA00000002000011052D2A021202FE15030000021202037D010000041202047D020000041202166A7D0300000408280100002B172A020302280600000A6F0700000A280E0000060A020328080000060B0605370407053402162A020302280600000A6F0700000A0605DB280D00000602030705DB280900000602040204280800000605D72809000006021202FE15030000021202037D010000041202047D020000041202057D0300000408280100002B172A00001330040065000000030000110202280600000A6F0700000A03280E000006042E02162A0202280600000A6F0700000A0305280D000006021200FE1504000002120002280600000A6F0700000A7D040000041200037D050000041200057D070000041200047D0600000406280200002B172A8E02280800000A7249000070038C08000001048C08000001280F00000A056F0C00000A2A8A02280800000A7249000070038C08000001048C08000001280F00000A6F0B00000A2A42534A4201000100000000000C00000076342E302E33303331390000000005006C00000098030000237E0000040400000403000023537472696E67730000000008070000700000002355530078070000100000002347554944000000880700000C01000023426C6F6200000000000000020000015717A201090A000000FA013300160000010000000E00000004000000070000000E00000017000000010000000F00000007000000030000000100000003000000060000000100000003000000020000000200000000007E010100000000000600EB00440206001A0144020600D70010020F00640200000A00A10283020E00C60123020A008B0083020A007302830206008100AD010A000B0183020A00550083020A00B200830206005301AD010600AF02AD01000000001500000000000100010001001000C70100001500010001000A01100066010000250001000F000A0110005A010000250004000F000600BC0174000600DD0174000600C70278000600FE0174000600EE0174000600B60278000600C702780050200000000086180A027B000100812000000000860890018400050093200000000081089B0188000500A6200000000086086A0084000600B820000000008108730088000600CB2000000000E609D5028D000700DD20000000008108E50291000700F02000000000E6013500960008000D2100000000810040009C0009002C2100000000E601D501A3000B00E02100000000E601B401AA000D00982200000000E6013E01B300100009230000000081007201BB0013002D2300000000E6014B00C4001600000001009F0000000200F502000003007C0000000400A601000001003801000001003801000001003801000001007B02000001007B0200000200380100000100E00100000200CE0200000100C10100000200E00100000300CE0200000100F60100000200C00200000300CE0200000100040200000200F60100000300380100000100040200000200F6010200190009000A02010011000A02060019000A020A0051000A02060029000A02100029005E0016005900E3011B002900C3002000610046012500610050012A0061000100300061000B00350069009A023B0029006E01470069009A0264002100230005012E000B00D4002E001300DD002E001B00FC00410023000501810023000501A10023000501410053005A000200010000009F01CC0000007700CC000000E902D000020002000300010003000300020004000500010005000500020006000700010007000700048000000000000000000000000000000000A10200000400000000000000000000006B001E00000000000100020001000000000000000000830200000000010000000000000000000000000023020000000003000200040002001D004E001D005F00000000000047657455496E7436340053657455496E743634003C4D6F64756C653E0053797374656D2E507269766174652E436F72654C69620047657442616C616E63650053657442616C616E636500416C6C6F77616E636500494D657373616765006765745F4D657373616765006765745F4E616D65007365745F4E616D65006E616D650056616C7565547970650049536D617274436F6E7472616374537461746500736D617274436F6E74726163745374617465004950657273697374656E745374617465006765745F50657273697374656E7453746174650044656275676761626C6541747472696275746500436F6D70696C6174696F6E52656C61786174696F6E7341747472696275746500496E6465784174747269627574650052756E74696D65436F6D7061746962696C6974794174747269627574650076616C756500417070726F766500476574537472696E6700536574537472696E6700417070726F76616C4C6F67005472616E736665724C6F6700536574417070726F76616C00536D617274436F6E74726163742E646C6C006765745F53796D626F6C007365745F53796D626F6C0073796D626F6C0053797374656D005472616E7366657246726F6D0066726F6D00495374616E64617264546F6B656E005472616E73666572546F00746F006765745F53656E646572005370656E646572007370656E646572004F776E6572006F776E6572002E63746F720053797374656D2E446961676E6F737469637300537472617469732E536D617274436F6E7472616374732E5374616E64617264730053797374656D2E52756E74696D652E436F6D70696C6572536572766963657300446562756767696E674D6F6465730041646472657373006164647265737300537472617469732E536D617274436F6E74726163747300466F726D617400536D617274436F6E7472616374004F626A656374004F6C64416D6F756E740063757272656E74416D6F756E7400616D6F756E74006765745F546F74616C537570706C79007365745F546F74616C537570706C7900746F74616C537570706C7900000000000D530079006D0062006F006C0000094E0061006D006500001754006F00740061006C0053007500700070006C0079000017420061006C0061006E00630065003A007B0030007D00002341006C006C006F00770061006E00630065003A007B0030007D003A007B0031007D0000000000F5148117F9C6F840A461EEE65ADF72F40004200101080320000105200101111105200101121D042000122D042000112104200012310420010E0E052002010E0E0420010B0E052002010E0B0500020E0E1C0507020B110C06300101011E00040A01110C0607030B0B110C0407011110040A0111100600030E0E1C1C087CEC85D7BEA7798E0306112102060B08200401121D0B0E0E0320000E042001010E0320000B042001010B0520010B11210620020111210B0620020211210B08200302112111210B0720030211210B0B08200301112111210B0720020B112111210328000E0328000B0801000800000000001E01000100540216577261704E6F6E457863657074696F6E5468726F7773010801000200000000000401000000000000000000000000000000000010000000000000000000000000000000282C00000000000000000000422C0000002000000000000000000000000000000000000000000000342C0000000000000000000000005F436F72446C6C4D61696E006D73636F7265652E646C6C0000000000FF250020001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000002000000C000000543C00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";

        public IssueToken()
        {
            InitializeComponent();

        }

        public IssueToken(string walletname, string selectedAddress, string balance)
        {
            InitializeComponent();

            this.DataContext = this;
            this.walletName = walletname;
            this.walletInfo.walletName = this.walletName;
            this.txtSender.Text = selectedAddress;
            this.txtBalance.Text = balance;
            this.txtTokenName.Text = this.tokenName;
            this.txtTokenSymbol.Text = this.tokenSymbol;
            this.txtTotalSupply.Text = this.totalSupply.ToString();
            this.txtDecimal.Text = this.decimals.ToString();
            this.txtFee.Text = this.feeAmt.ToString();
            this.txtGasPrice.Text = this.gasPrice.ToString();
            this.txtGasLimit.Text = this.gasLimit.ToString();

            //LoadAsync();
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
        private void btn_Create_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        private async void btn_IssueTokenSubmit_Click(object sender, RoutedEventArgs e)
        {
            string result = "";
            if (IntCheck())
            {
            IssueTokenModel issueTokenModel = new IssueTokenModel();
            List<string> param = new List<string>();
            issueTokenModel.amount = "0";
            issueTokenModel.feeAmount = this.txtFee.Text;
            issueTokenModel.gasPrice = Convert.ToUInt64(this.txtGasPrice.Text);
            issueTokenModel.gasLimit = Convert.ToUInt64(this.txtGasLimit.Text);

            param.Add(this.txtTotalSupply.Text);
            param.Add(this.txtTokenName.Text);
            param.Add(this.txtTokenSymbol.Text);
            issueTokenModel.parameters = param.ToArray();
            issueTokenModel.ContractCode = this.newTokenByteCode;
            issueTokenModel.password = this.txtPassword.Password.ToString();
            issueTokenModel.walletName = this.walletName;
            issueTokenModel.sender = this.txtSender.Text;

          
                if (ValidationCheck())
                {
                    result = await IssueTokenSubmitAsync(issueTokenModel);
                }

            }

        }


        private async Task<string> IssueTokenSubmitAsync(IssueTokenModel ObjissueTokenModel)
        {
            string retMsg = "";
            try
            {
                string postUrl = this.baseURL + "/SmartContractWallet/create";

                HttpResponseMessage response = await client.PostAsync(postUrl, new StringContent(JsonConvert.SerializeObject(ObjissueTokenModel), Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {

                    retMsg = "Successfully Create Contract";
                    // MessageBox.Show("");

                    this.Visibility = Visibility.Collapsed;
                }
                else
                {
                    MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                return retMsg;
            }

            return retMsg;
        }

        public bool ValidationCheck()
        {
            
           double varBalnce= double.Parse(this.txtBalance.Text);
            //double.Parse(this.txtBalance.Text)
            if (double.Parse(this.txtFee.Text) > varBalnce || double.Parse(this.txtFee.Text) == varBalnce)
            {
                MessageBox.Show("Fee must be less than your balance", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                this.txtFee.Focus();
                return false;
            }

            if (double.Parse(this.txtFee.Text) < 0.001 || this.txtFee.Text == "")
            {
                MessageBox.Show("The amount cannot be negative", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                this.txtFee.Focus();
                return false;
            }


            if (double.Parse(this.txtGasPrice.Text) < Convert.ToInt64(this.gasPriceMinimum))
            {
                MessageBox.Show("Gas price must be greater than  " + this.gasPriceMinimum, "Failed", MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }
            if (double.Parse(this.txtGasPrice.Text) > Convert.ToInt64(this.gasPriceMaximum))
            {
                MessageBox.Show("Gas price must be less than  " + this.gasPriceMaximum, "Failed", MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }
            if (double.Parse(this.txtGasPrice.Text) < 0 || this.txtGasPrice.Text == "" || !Regex.IsMatch(this.txtGasPrice.Text, @"^[0-9][0-9]*$") || !Regex.IsMatch(this.txtGasPrice.Text, @"^[+]?([0-9]{0,})*[.]?([0-9]{0,2})?$"))
            {
                MessageBox.Show("The gas price cannot be negative", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                this.txtGasPrice.Focus();
                return false;
            }


            if (Convert.ToInt64(this.txtGasLimit.Text) > Convert.ToInt64(this.gasLimitMaximum) || !Regex.IsMatch(this.txtGasPrice.Text, @"^[0-9][0-9]*$") || !Regex.IsMatch(this.txtGasPrice.Text, @"^[+]?([0-9]{0,})*[.]?([0-9]{0,2})?$"))
            {
                MessageBox.Show("Gas limit must be less than  " + this.gasLimitMaximum, "Failed", MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }

            if (Convert.ToInt64(this.txtGasLimit.Text) < Convert.ToInt64(this.gasCreateTokenLimitMinimum))
            {
                MessageBox.Show("Gas limit must be greater than  " + this.gasCallLimitMinimum, "Failed", MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }

            if (Convert.ToInt64(this.txtGasLimit.Text) < 0 || this.txtGasLimit.Text.Trim() == "")
            {
                MessageBox.Show("The amount cannot be negative", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                this.txtGasLimit.Focus();
                return false;
            }

            if (Convert.ToInt64(this.txtDecimal.Text) < 0 || this.txtDecimal.Text.Trim() == "")
            {
                MessageBox.Show("The Decimal cannot be negative or cant be empty!.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                this.txtDecimal.Focus();
                return false;
            }

            if (Convert.ToInt64(this.txtDecimal.Text.Trim()) > 8)
            {
                MessageBox.Show("Max Limit must be less then 8", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                this.txtDecimal.Focus();
                return false;
            }


            if (Convert.ToInt64(this.txtTotalSupply.Text) < 1 || this.txtTotalSupply.Text.Trim() == "")
            {
                MessageBox.Show("The TotalSupply cannot be negative or cant be empty!.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                this.txtDecimal.Focus();
                return false;
            }

            if (this.txtTokenName.Text.Trim() == "")
            {
                MessageBox.Show("Token Name is required..", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                this.txtTokenName.Focus();
                return false;
            }
            if (this.txtTokenSymbol.Text.Trim() == "")
            {
                MessageBox.Show("Token Symbol is required..", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                this.txtTokenName.Focus();
                return false;
            }

            if (this.txtPassword.Password == "")
            {
                MessageBox.Show("Password is reuired. Please enter the password for wallet: " + this.walletName, "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                this.txtPassword.Focus();
                return false;
            }


            return true;
        }

        public bool IntCheck()
        {
            double k;
            bool intDeccheck = double.TryParse(this.txtFee.Text, out k);
            if (!intDeccheck)
            {
                MessageBox.Show("Value Is not valid must be number.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                this.txtFee.Focus();
                return false;
            }

            int i;
            bool success = int.TryParse(this.txtDecimal.Text, out i);
            if (!success)
            {
                MessageBox.Show("Value Is not valid must be number.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                this.txtDecimal.Focus();
                return false;
            }
            double l;
            bool gaspriceCheck = double.TryParse(this.txtGasPrice.Text, out l);
            if (!gaspriceCheck)
            {
                MessageBox.Show("Value Is not valid must be number.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                this.txtGasPrice.Focus();
                return false;
            }

            double m;
            bool gaslimitCheck = double.TryParse(this.txtGasLimit.Text, out m);
            if (!gaslimitCheck)
            {
                MessageBox.Show("Value Is not valid must be number.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                this.txtGasLimit.Focus();
                return false;
            }


            int j;
            bool success2 = int.TryParse(this.txtTotalSupply.Text, out j);
            if (!success2)
            {
                MessageBox.Show("Value Is not valid must be number.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                this.txtTotalSupply.Focus();
                return false;
            }

            return true;
        }



        }
}
