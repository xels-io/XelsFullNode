using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using XelsDesktopWalletApp.Models;

namespace XelsDesktopWalletApp.Views
{
    /// <summary>
    /// Interaction logic for EthImport.xaml
    /// </summary>
    public partial class EthImport : Window
    {
        private readonly WalletInfo walletInfo = new WalletInfo();
        private string walletName;

        private string[] walletHashArray;
        private string walletHash;
        private bool isCheckBoxChecked = false;
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
        public EthImport()
        {
            InitializeComponent();
        }

        public EthImport(string walletname)
        {
            InitializeComponent();
            this.DataContext = this;
            HidePrivateKeyTxt();

            this.walletName = walletname;
            this.walletInfo.walletName = this.walletName;
        }

        public bool isValid()
        {
            if (this.MnemonicTxt.Text == string.Empty)
            {
                MessageBox.Show("Mnemonic is required!", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                this.MnemonicTxt.Focus();
                return false;
            }

            return true;
        }



        public bool isValidPKey()
        {
            if (isValid())
            {
                if (this.SELSPrivateKeyTxt.Text == string.Empty)
                {
                    MessageBox.Show("SELS Private Key is required!", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.SELSPrivateKeyTxt.Focus();
                    return false;
                }

                if (this.BELSPrivateKeyTxt.Text == string.Empty)
                {
                    MessageBox.Show("BELS Private Key is required!", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.BELSPrivateKeyTxt.Focus();
                    return false;
                }
            }

            return true;
        }


        private string MnemonicToHash(string mnemonic)
        {
            this.walletHashArray = new string[mnemonic.Length];
            if (mnemonic.Length != 0)
            {
                int ind = 0;
                foreach (char c in mnemonic)
                {
                    int unicode = c;
                    string code = Convert.ToString(unicode, 2);
                    this.walletHashArray[ind] = code;
                    ind++;
                }
            }
            string hashvalue = string.Join("", this.walletHashArray);
            
            return hashvalue;
        }


        private void TransactionIDCopyButton_Click(object sender, RoutedEventArgs e)
        {
            this.walletHash = MnemonicToHash(this.MnemonicTxt.Text); 
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.isCheckBoxChecked = true;

            this.SPSELS.Visibility = Visibility.Visible;
            this.SPSELSTxt.Visibility = Visibility.Visible;
            this.SPBELS.Visibility = Visibility.Visible;
            this.SPBELSTxt.Visibility = Visibility.Visible;
        }
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.isCheckBoxChecked = false;
            HidePrivateKeyTxt();
        }

        private void HidePrivateKeyTxt()
        {
            this.SPSELS.Visibility = Visibility.Hidden;
            this.SPSELSTxt.Visibility = Visibility.Hidden;
            this.SPBELS.Visibility = Visibility.Hidden;
            this.SPBELSTxt.Visibility = Visibility.Hidden;
        }

    }
}
