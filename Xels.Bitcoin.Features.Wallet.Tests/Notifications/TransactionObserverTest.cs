using Moq;
using NBitcoin;
using Xels.Bitcoin.Features.Wallet.Interfaces;
using Xels.Bitcoin.Features.Wallet.Notifications;
using Xunit;

namespace Xels.Bitcoin.Features.Wallet.Tests.Notifications
{
    public class TransactionObserverTest
    {
        [Fact]
        public void OnNextCoreProcessesOnTheWalletSyncManager()
        {
            var walletSyncManager = new Mock<IWalletSyncManager>();
            var observer = new TransactionObserver(walletSyncManager.Object);
            var transaction = new Transaction();

            observer.OnNext(transaction);

            walletSyncManager.Verify(w => w.ProcessTransaction(transaction), Times.Exactly(1));
        }
    }
}
