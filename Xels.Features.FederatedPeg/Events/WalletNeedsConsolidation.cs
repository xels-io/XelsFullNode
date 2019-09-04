using System;
using NBitcoin;
using Xels.Bitcoin.EventBus;

namespace Xels.Features.FederatedPeg.Events
{
    public class WalletNeedsConsolidation : EventBase
    {
        /// <summary>
        /// The amount required for the next withdrawal transaction..
        /// </summary>
        public Money Amount { get; }

        public WalletNeedsConsolidation(Money amount)
        {
            this.Amount = amount;
        }
    }
}
