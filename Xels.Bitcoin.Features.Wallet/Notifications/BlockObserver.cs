using NBitcoin;
using Xels.Bitcoin.Features.Wallet.Interfaces;
using Xels.Bitcoin.Primitives;
using Xels.Bitcoin.Signals;
using Xels.Bitcoin.Utilities;

namespace Xels.Bitcoin.Features.Wallet.Notifications
{
    /// <summary>
    /// Observer that receives notifications about the arrival of new <see cref="ChainedHeaderBlock"/>s.
    /// </summary>
    public class BlockObserver : SignalObserver<ChainedHeaderBlock>
    {
        private readonly IWalletSyncManager walletSyncManager;

        public BlockObserver(IWalletSyncManager walletSyncManager)
        {
            Guard.NotNull(walletSyncManager, nameof(walletSyncManager));

            this.walletSyncManager = walletSyncManager;
        }

        /// <summary>
        /// Manages what happens when a new chained header block is received.
        /// </summary>
        /// <param name="chainedHeaderBlock">The new chained header block</param>
        protected override void OnNextCore(ChainedHeaderBlock chainedHeaderBlock)
        {
            this.walletSyncManager.ProcessBlock(chainedHeaderBlock.Block);
        }
    }
}
