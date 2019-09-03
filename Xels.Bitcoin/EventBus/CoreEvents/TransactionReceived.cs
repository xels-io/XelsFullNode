using NBitcoin;

namespace Xels.Bitcoin.EventBus.CoreEvents
{
    /// <summary>
    /// Event that is executed when a transaction is received from another peer.
    /// </summary>
    /// <seealso cref="Xels.Bitcoin.EventBus.EventBase" />
    public class TransactionReceived : EventBase
    {
        public Transaction ReceivedTransaction { get; }

        public TransactionReceived(Transaction receivedTransaction)
        {
            this.ReceivedTransaction = receivedTransaction;
        }
    }
}