using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;
using Xels.Bitcoin.Features.Wallet;

namespace XelsDesktopWalletApp.Models
{
    public class SendConfirmation
    {
        public TransactionBuilding transaction { get; set; }
        public Money transactionFee { get; set; }
        public CoinType cointype { get; set; }
    }

    public class SendConfirmationSC
    {
        public TransactionBuildingSidechain transaction { get; set; }
        public Money transactionFee { get; set; }
        //public bool sidechainEnabled { get; set; }
        public Money opReturnAmount { get; set; }
        //public bool hasOpReturn { get; set; }
        public CoinType cointype { get; set; }
    }
}
