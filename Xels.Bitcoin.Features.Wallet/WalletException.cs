using System;

namespace Xels.Bitcoin.Features.Wallet
{
    public class WalletException : Exception
    {
        public WalletException(string message) : base(message)
        {
        }
    }
}
