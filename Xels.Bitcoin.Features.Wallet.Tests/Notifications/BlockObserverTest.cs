using Moq;
using NBitcoin;
using Xels.Bitcoin.Features.Wallet.Interfaces;
using Xels.Bitcoin.Features.Wallet.Notifications;
using Xels.Bitcoin.Primitives;
using Xels.Bitcoin.Tests.Common;
using Xunit;

namespace Xels.Bitcoin.Features.Wallet.Tests.Notifications
{
    public class BlockObserverTest
    {
        [Fact]
        public void OnNextCoreProcessesOnTheWalletSyncManager()
        {
            var walletSyncManager = new Mock<IWalletSyncManager>();
            var observer = new BlockObserver(walletSyncManager.Object);
            Block block = KnownNetworks.XelsMain.CreateBlock();
            ChainedHeader header = ChainedHeadersHelper.CreateGenesisChainedHeader();
            var chainedHeaderBlock = new ChainedHeaderBlock(block, header);
            
            observer.OnNext(chainedHeaderBlock);

            walletSyncManager.Verify(w => w.ProcessBlock(block), Times.Exactly(1));
        }
    }
}