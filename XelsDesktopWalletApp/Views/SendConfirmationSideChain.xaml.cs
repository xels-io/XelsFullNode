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
    /// Interaction logic for SendConfirmationSideChain.xaml
    /// </summary>
    public partial class SendConfirmationSideChain : Window
    {
        private string walletName;

        public SendConfirmationSideChain()
        {
            InitializeComponent();
        }

        public SendConfirmationSideChain(SendConfirmationSC sendConf, string walletname)
        {
            InitializeComponent();
            this.DataContext = this;

            this.walletName = walletname;
            bindData(sendConf);
        }

        private void bindData(SendConfirmationSC data)
        {
            Money amountSent = data.transaction.feeAmount - data.transactionFee;
            this.AmountSent.Content = amountSent;
            this.AmountSentType.Content = data.cointype;

            this.Fee.Content = data.transactionFee;
            this.FeeType.Content = data.cointype;

            this.OPreturn.Content = data.opReturnAmount;
            this.OPreturnType.Content = data.cointype;

            this.Total.Content = data.transaction.feeAmount;
            this.TotalType.Content = data.cointype;

            this.DestinationFederation.Content = data.transaction.recipients[0].federationAddress;
            this.DestinationAddress.Content = data.transaction.opReturnData;

        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            SendSideChain sendSc = new SendSideChain(this.walletName);
            sendSc.Show();
            this.Close();
        }
    }
}
