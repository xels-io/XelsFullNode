using Microsoft.Extensions.Logging;
using NBitcoin;
using Xels.Bitcoin.Configuration;
using Xels.Bitcoin.Consensus;
using Xels.Bitcoin.Features.MemoryPool;
using Xels.Bitcoin.Features.MemoryPool.Interfaces;
using Xels.Bitcoin.Features.Miner;
using Xels.Bitcoin.Features.PoA.ConsensusRules;
using Xels.Bitcoin.Mining;
using Xels.Bitcoin.Utilities;

namespace Xels.Bitcoin.Features.PoA
{
    public class PoABlockDefinition : BlockDefinition
    {
        public PoABlockDefinition(
            IConsensusManager consensusManager,
            IDateTimeProvider dateTimeProvider,
            ILoggerFactory loggerFactory,
            ITxMempool mempool,
            MempoolSchedulerLock mempoolLock,
            Network network)
            : base(consensusManager, dateTimeProvider, loggerFactory, mempool, mempoolLock, new MinerSettings(NodeSettings.Default(network)), network)
        {
        }

        /// <inheritdoc/>
        public override void AddToBlock(TxMempoolEntry mempoolEntry)
        {
            this.AddTransactionToBlock(mempoolEntry.Transaction);
            this.UpdateBlockStatistics(mempoolEntry);
            this.UpdateTotalFees(mempoolEntry.Fee);
        }

        /// <inheritdoc/>
        public override BlockTemplate Build(ChainedHeader chainTip, Script scriptPubKey)
        {
            base.OnBuild(chainTip, scriptPubKey);

            return this.BlockTemplate;
        }

        /// <inheritdoc/>
        public override void UpdateHeaders()
        {
            base.UpdateBaseHeaders();

            this.block.Header.Bits = PoAHeaderDifficultyRule.PoABlockDifficulty;
        }
    }
}
