using Microsoft.Extensions.Logging;
using NBitcoin;
using Xels.Bitcoin.Consensus;
using Xels.Bitcoin.Features.Consensus.CoinViews;
using Xels.Bitcoin.Features.MemoryPool;
using Xels.Bitcoin.Features.MemoryPool.Interfaces;
using Xels.Bitcoin.Features.Miner;
using Xels.Bitcoin.Features.PoA.BasePoAFeatureConsensusRules;
using Xels.Bitcoin.Features.SmartContracts.Caching;
using Xels.Bitcoin.Features.SmartContracts.PoW;
using Xels.Bitcoin.Utilities;
using Xels.SmartContracts.CLR;
using Xels.SmartContracts.Core;
using Xels.SmartContracts.Core.State;
using Xels.SmartContracts.Core.Util;

namespace Xels.Bitcoin.Features.SmartContracts.PoA
{
    /// <summary>
    /// Pushes everything to the <see cref="SmartContractBlockDefinition"/>, just amends the block difficulty for PoA.
    /// </summary>
    public class SmartContractPoABlockDefinition : SmartContractBlockDefinition
    {
        public SmartContractPoABlockDefinition(
            IBlockBufferGenerator blockBufferGenerator,
            ICoinView coinView,
            IConsensusManager consensusManager,
            IDateTimeProvider dateTimeProvider,
            IContractExecutorFactory executorFactory,
            ILoggerFactory loggerFactory,
            ITxMempool mempool,
            MempoolSchedulerLock mempoolLock,
            Network network,
            ISenderRetriever senderRetriever,
            IStateRepositoryRoot stateRoot,
            IBlockExecutionResultCache executionCache,
            ICallDataSerializer callDataSerializer,
            MinerSettings minerSettings)
            : base(blockBufferGenerator, coinView, consensusManager, dateTimeProvider, executorFactory, loggerFactory, mempool,
                mempoolLock, minerSettings, network, senderRetriever, stateRoot, executionCache, callDataSerializer)
        {
            // TODO: Fix gross MinerSettings injection ^^
        }

        /// <inheritdoc/>
        public override void UpdateHeaders()
        {
            base.UpdateHeaders();

            this.block.Header.Bits = PoAHeaderDifficultyRule.PoABlockDifficulty;
        }
    }
}
