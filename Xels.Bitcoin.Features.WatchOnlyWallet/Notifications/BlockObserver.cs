using NBitcoin;
using Xels.Bitcoin.Primitives;
using Xels.Bitcoin.Signals;

namespace Xels.Bitcoin.Features.WatchOnlyWallet.Notifications
{
    /// <summary>
    /// Observer that receives notifications about the arrival of new <see cref="ChainedHeaderBlock"/>s.
    /// </summary>
    public class BlockObserver : SignalObserver<ChainedHeaderBlock>
    {
        private readonly IWatchOnlyWalletManager walletManager;

        public BlockObserver(IWatchOnlyWalletManager walletManager)
        {
            this.walletManager = walletManager;
        }

        /// <summary>
        /// Manages what happens when a new block is received.
        /// </summary>
        /// <param name="chainedHeaderBlock">The new chained header block.</param>
        protected override void OnNextCore(ChainedHeaderBlock chainedHeaderBlock)
        {
            this.walletManager.ProcessBlock(chainedHeaderBlock.Block);
        }
    }
}
