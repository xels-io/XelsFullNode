using NBitcoin;
using Xels.Bitcoin.EventBus;

namespace Xels.Bitcoin.Features.MemoryPool
{
    /// <summary>
    /// Event that is executed when a transaction is removed from the mempool.
    /// </summary>
    /// <seealso cref="Xels.Bitcoin.EventBus.EventBase" />
    public class TransactionAddedToMemoryPool : EventBase
    {
        public Transaction AddedTransaction { get; }

        public TransactionAddedToMemoryPool(Transaction addedTransaction)
        {
            this.AddedTransaction = addedTransaction;
        }
    }
}