using System;
using System.Collections.Generic;
using System.Text;
using Xels.Bitcoin.Features.Wallet;

namespace XelsDesktopWalletApp.Models
{
    public class HistoryModel
    {
        public HistoryModel()
        {
            this.transactionsHistory = new TransactionItemModelArray();
        }
        public string accountName { get; set; }
        public string accountHdPath { get; set; }
        public CoinType coinType { get; set; }
        public TransactionItemModelArray transactionsHistory { get; set; }
    }

    public class HistoryModelArray
    {
        public HistoryModel[] history { get; set; }
    }
}
