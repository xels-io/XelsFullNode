using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;
using Xels.Bitcoin.Features.Wallet;
using Xels.Bitcoin.Features.Wallet.Models;

namespace XelsDesktopWalletApp.Models
{
    public class WalletBalance
    {
        public string accountName { get; set; }
        public string accountHdPath { get; set; }

        public CoinType coinType { get; set; }

        public Money amountConfirmed { get; set; }
        public Money amountUnconfirmed { get; set; }
        public Money spendableAmount { get; set; }
        public IEnumerable<AddressModel> addresses { get; set; }

    }

    public class WalletBalanceArray
    {
        public WalletBalance[] balances { get; set; }
    }
}
