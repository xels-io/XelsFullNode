using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;

namespace XelsDesktopWalletApp.Models
{
    public class TransactionItemModel
    {
        public TransactionItemType type { get; set; }
        public string toAddress { get; set; }
        public Money amount { get; set; }
        public uint256 id { get; set; }
        public DateTimeOffset timestamp { get; set; }
        public int? confirmedInBlock { get; set; }
        public int? blockIndex { get; set; }
        public Money fee { get; set; }
    }

    public  class TransactionItemModelArray  
    {
        public TransactionItemModel[] Transactions { get; set; }
    }

    public enum TransactionItemType
    {
        Received,
        Send,
        Staked,
        Mined
    }
    

}
