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
using NBitcoin;
using XelsDesktopWalletApp.Models;

namespace XelsDesktopWalletApp.Views
{
    /// <summary>
    /// Interaction logic for SendConfirmationMainChain.xaml
    /// </summary>
    public partial class SendConfirmationMainChain : Window
    {
        private readonly WalletInfo walletInfo = new WalletInfo();
        private string walletName;
        
        public SendConfirmationMainChain()
        {
            InitializeComponent();
        }

        public SendConfirmationMainChain(SendConfirmation sendConf, string walletname)
        {
            InitializeComponent();
            this.DataContext = this;

            this.walletName = walletname;
            bindData(sendConf);
        }

        private void bindData(SendConfirmation data)
        {
            Money amountSent = data.transaction.feeAmount - data.transactionFee;
            this.AmountSent.Content = amountSent;
            this.AmountSentType.Content = data.cointype;

            this.Fee.Content = data.transactionFee;
            this.FeeType.Content = data.cointype;

            this.Total.Content = data.transaction.feeAmount;
            this.TotalType.Content = data.cointype;

            this.Destination.Content = data.transaction.recipients[0].destinationAddress;

        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Send send = new Send(this.walletName);
            send.Show();
            this.Close();
        }


    }
}
