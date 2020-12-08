using Microsoft.Extensions.Logging;
using NBitcoin;
using Xels.Bitcoin.Consensus;
using Xels.Bitcoin.Features.Consensus.CoinViews;
using Xels.Bitcoin.Features.MemoryPool;
using Xels.Bitcoin.Features.MemoryPool.Interfaces;
using Xels.Bitcoin.Features.Miner;
using Xels.Bitcoin.Features.SmartContracts;
using Xels.Bitcoin.Features.SmartContracts.Caching;
using Xels.Bitcoin.Features.SmartContracts.PoA;
using Xels.Bitcoin.Mining;
using Xels.Bitcoin.Utilities;
using Xels.SmartContracts.CLR;
using Xels.SmartContracts.Core;
using Xels.SmartContracts.Core.State;
using Xels.SmartContracts.Core.Util;

namespace Xels.Features.FederatedPeg.IntegrationTests.Utils
{
    /// <summary>
    /// Exact same as FederatedPegBlockDefinition, just gives the premine to a wallet for convenience.
    /// </summary>
    public class TestFederatedPegBlockDefinition : SmartContractPoABlockDefinition
    {
        public TestFederatedPegBlockDefinition(
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
            : base(blockBufferGenerator, coinView, consensusManager, dateTimeProvider, executorFactory, loggerFactory, mempool, mempoolLock, network, senderRetriever, stateRoot, executionCache, callDataSerializer, minerSettings)
        {
        }

        public override BlockTemplate Build(ChainedHeader chainTip, Script scriptPubKey)
        {
            return base.Build(chainTip, scriptPubKey);
        }
    }
}