using System;
using System.Collections.Generic;
using System.Text;

namespace XelsDesktopWalletApp.Models
{
    public class TransactionSending
    {
        public string hex { get; set; }
    }

    public class Recipient
    {
        //public Recipient(string destinationAddress, string amount)
        //{
        //    this.destinationAddress = destinationAddress;
        //    this.amount = amount;
        //}
        public string destinationAddress { get; set; }
        public string amount { get; set; }
    }

    public class TransactionBuilding
    {
        public string walletName { get; set; }
        public string accountName { get; set; }
        public string password { get; set; }
        //public Recipient[] recipients { get; set; }
        public List<Recipient> recipients { get; set; }
        public double feeAmount { get; set; }
        public bool allowUnconfirmed { get; set; }
        public bool shuffleOutputs { get; set; }
        public string opReturnData { get; set; }
        public string opReturnAmount { get; set; }
    }
}
