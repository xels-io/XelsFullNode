using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;

namespace XelsDesktopWalletApp.Models
{
    public class TransactionItemModel
    {
        public TransactionItemModel()
        {
            this.payments = new PaymentDetailModelArray();
        }

        public TransactionItemType type { get; set; }
        public string toAddress { get; set; }
        public uint256 id { get; set; }
        public Money amount { get; set; }
        public PaymentDetailModelArray payments { get; set; }
        public Money fee { get; set; }
        public int? confirmedInBlock { get; set; }
        public DateTimeOffset timestamp { get; set; }
        public long txOutputTime => this.timestamp.ToUnixTimeSeconds();
        public int txOutputIndex { get; set; }
        public int? blockIndex { get; set; }
    }

    public class TransactionItemModelArray
    {
        public TransactionItemModel[] transactionsItem { get; set; }
    }

    public enum TransactionItemType
    {
        Received,
        Send,
        Staked,
        Mined
    }

    public class PaymentDetailModel
    {
        public string destinationAddress { get; set; }
        public Money amount { get; set; }
    }

    public class PaymentDetailModelArray
    {
        public PaymentDetailModel[] payments { get; set; }
    }


    public class TransactionInfo
    {
        public string transactionType { get; set; }
        public uint256 transactionId { get; set; }
        public Money transactionAmount { get; set; }
        public Money transactionFee { get; set; }
        public int? transactionConfirmedInBlock { get; set; }
        public DateTimeOffset transactionTimestamp { get; set; }

    }

    public class TransactionInfoArray
    {
        public TransactionInfo[] transactionInfos { get; set; }
    }
    

}
