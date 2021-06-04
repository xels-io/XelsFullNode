using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;

namespace XelsDesktopWalletApp.Models
{
    public class BuildTransaction
    {
        public Money fee { get; set; }
        public string hex { get; set; }
        public uint256 transactionId { get; set; }
    }
}
