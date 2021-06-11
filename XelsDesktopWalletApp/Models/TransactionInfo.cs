using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;

namespace XelsDesktopWalletApp.Models
{
    public class TransactionInfo
    {
        public string transactionType { get; set; }
        public TransactionItemTypeName transactionTypeName { get; set; }
        public uint256 transactionId { get; set; }
        public string transactionFinalAmount { get; set; }
        public Money transactionAmount { get; set; }
        public Money transactionFee { get; set; }
        public int? transactionConfirmedInBlock { get; set; }
        public DateTimeOffset transactionTimestamp { get; set; }

    }

    public enum TransactionItemTypeName
    {
        Confirmed,
        Unconfirmed
    }
}
