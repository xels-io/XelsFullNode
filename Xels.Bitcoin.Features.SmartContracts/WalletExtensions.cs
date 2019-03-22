using System.Collections.Generic;
using System.Linq;
using NBitcoin;
using Xels.Bitcoin.Features.Wallet.Interfaces;

namespace Xels.Bitcoin.Features.SmartContracts
{
    public static class WalletExtensions
    {
        private const int MinConfirmationsAllChecks = 0;

        public static List<OutPoint> GetSpendableInputsForAddress(this IWalletManager walletManager, string walletName, string address)
        {
            return walletManager.GetSpendableTransactionsInWallet(walletName, MinConfirmationsAllChecks).Where(x => x.Address.Address == address).Select(x => x.ToOutPoint()).ToList();
        }
    }
}
