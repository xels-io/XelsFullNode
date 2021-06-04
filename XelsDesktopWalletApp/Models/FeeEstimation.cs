using System;
using System.Collections.Generic;
using System.Text;

namespace XelsDesktopWalletApp.Models
{
    public class FeeEstimation
    {
        public string walletName { get; set; }
        public string accountName { get; set; }

        public Recipient[] recipients { get; set; }
        public string feeType { get; set; }
        public bool allowUnconfirmed { get; set; }


    }
    public class FeeEstimationSideChain
    {
        public string walletName { get; set; }
        public string accountName { get; set; }

        public RecipientSidechain[] recipients { get; set; }
        public string opreturndata { get; set; } // destinationaddr

        public string feeType { get; set; }
        public bool allowUnconfirmed { get; set; }

    }
}
