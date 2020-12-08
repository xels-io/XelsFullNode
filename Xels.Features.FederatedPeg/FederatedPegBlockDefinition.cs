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
using Xels.Features.FederatedPeg.Interfaces;
using Xels.SmartContracts.CLR;
using Xels.SmartContracts.Core;
using Xels.SmartContracts.Core.State;
using Xels.SmartContracts.Core.Util;

namespace Xels.Features.FederatedPeg
{
    public class FederatedPegBlockDefinition : SmartContractPoABlockDefinition
    {
        /// <summary>
        /// The number of outputs we break the premine reward up into, so that the federation can build more than one transaction at once.
        /// </summary>
        public const int FederationWalletOutputs = 10;

        private readonly Script payToMultisigScript;

        private readonly ICoinbaseSplitter premineSplitter;

        /// <inheritdoc />
        public FederatedPegBlockDefinition(
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
            ICoinbaseSplitter premineSplitter,
            IBlockExecutionResultCache executionCache,
            ICallDataSerializer callDataSerializer,
            MinerSettings minerSettings,
            FederatedPegSettings federatedPegSettings)
            : base(blockBufferGenerator, coinView, consensusManager, dateTimeProvider, executorFactory, loggerFactory, mempool, mempoolLock, network, senderRetriever, stateRoot, executionCache, callDataSerializer, minerSettings)
        {
            this.payToMultisigScript = federatedPegSettings.MultiSigAddress.ScriptPubKey;

            this.premineSplitter = premineSplitter;
        }

        public override BlockTemplate Build(ChainedHeader chainTip, Script scriptPubKey)
        {
            // Note: When creating a new chain, ensure that the first nodes mining are the federated peg nodes, 
            // so that the premine goes to the federated peg wallet.

            // The other nodes don't know about the federated wallet in the current design.
            // If this changes, a consensus rule should be built that enforces that the premine goes to that address.

            bool miningPremine = (chainTip.Height + 1) == this.Network.Consensus.PremineHeight;

            // If we are not mining the premine, then the reward should fall back to what was selected by the caller.
            Script rewardScript = miningPremine ? this.payToMultisigScript : scriptPubKey;

            BlockTemplate built = base.Build(chainTip, rewardScript);

            if (miningPremine)
            {
                this.premineSplitter.SplitReward(this.coinbase);
            }

            return built;
        }

    }
}