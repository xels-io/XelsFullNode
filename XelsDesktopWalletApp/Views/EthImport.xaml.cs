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

            this.walletName = walletname;
            this.walletInfo.walletName = this.walletName;

        }


        private void TransactionIDCopyButton_Click(object sender, RoutedEventArgs e)
        {

        }


    }
}
