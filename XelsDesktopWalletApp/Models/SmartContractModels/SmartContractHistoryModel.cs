using System;
using System.Collections.Generic;
using System.Text;

namespace XelsDesktopWalletApp.Models.SmartContractModels
{
    public class SmartContractHistoryModel
    {
        public SmartContractHistoryAccountModel[] history { get; set; }
    }

    public class SmartContractHistoryAccountModel
    {
        public string accountName { get; set; }
        public string accountHdPath { get; set; }
        public string coinType { get; set; }
        public TransactionsHistoryItem[] transactionsHistory { get; set; }

    }
    public class TransactionsHistoryItem
    {
        public string type { get; set; }
        public string toAddress { get; set; }
        public string id { get; set; }

        public long amount { get; set; }
        public string[] payments { get; set; }
        public int confirmedInBlock { get; set; }

        public int timestamp { get; set; }
        public int txOutputIndex { get; set; }
        public int blockIndex { get; set; }
        public int fee { get; set; }
    }
    
}
