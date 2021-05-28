using System;
using System.Collections.Generic;
using System.Text;

namespace XelsDesktopWalletApp.Models
{
    public class FeeEstimation
    {
        public string walletName { get; set; }
        public string accountName { get; set; }
        public List<Recipient> recipients { get; set; }
        public string feeType { get; set; }
        public bool allowUnconfirmed { get; set; }

       
    }
}
