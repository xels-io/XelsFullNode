using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;

namespace XelsDesktopWalletApp.Models
{
    public class TransactionSending
    {
        public string hex { get; set; }
    }

    public class Recipient
    {
        public string destinationAddress { get; set; }
        public string amount { get; set; }
    }

    public class RecipientSidechain
    {
        public string federationAddress { get; set; }
        public string amount { get; set; }
    }

    public class TransactionBuilding
    {
        public string walletName { get; set; }
        public string accountName { get; set; }
        public string password { get; set; }

        public Recipient[] recipients { get; set; }

        //public string destinationAddress { get; set; }
        //public string amount { get; set; }
        
        public Money feeAmount { get; set; }
        public bool allowUnconfirmed { get; set; }
        public bool shuffleOutputs { get; set; }

        //public string opReturnData { get; set; }
        //public string opReturnAmount { get; set; }
    }

    public class TransactionBuildingSidechain
    {
        public string walletName { get; set; }
        public string accountName { get; set; }
        public string password { get; set; }

        public RecipientSidechain[] recipients { get; set; }

        //public string destinationAddress { get; set; }
        //public string amount { get; set; }

        public Money feeAmount { get; set; }
        public bool allowUnconfirmed { get; set; }
        public bool shuffleOutputs { get; set; }

        public string opReturnData { get; set; }
        public string opReturnAmount { get; set; }
    }

    public class MaximumBalance
    {
        public string WalletName { get; set; }
        public string AccountName { get; set; }
        public string FeeType { get; set; }
        public bool AllowUnconfirmed { get; set; }
    }

}
