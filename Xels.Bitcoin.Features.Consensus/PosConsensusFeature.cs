using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NBitcoin;
using Xels.Bitcoin.Base;
using Xels.Bitcoin.Base.Deployments;
using Xels.Bitcoin.Builder.Feature;
using Xels.Bitcoin.Configuration.Settings;
using Xels.Bitcoin.Connection;
using Xels.Bitcoin.Consensus;
using Xels.Bitcoin.Features.Consensus.Behaviors;
using Xels.Bitcoin.Interfaces;
using Xels.Bitcoin.P2P.Peer;
using Xels.Bitcoin.Utilities;

[assembly: InternalsVisibleTo("Xels.Bitcoin.Features.Miner.Tests")]
[assembly: InternalsVisibleTo("Xels.Bitcoin.Features.Consensus.Tests")]

namespace Xels.Bitcoin.Features.Consensus
{
    public class PosConsensusFeature : ConsensusFeature
    {
        private readonly Network network;
        private readonly IChainState chainState;
        private readonly IConnectionManager connectionManager;
        private readonly IConsensusManager consensusManager;
        private readonly NodeDeployments nodeDeployments;
        private readonly ConcurrentChain chain;
        private readonly IInitialBlockDownloadState initialBlockDownloadState;
        private readonly IPeerBanning peerBanning;
        private readonly ILoggerFactory loggerFactory;
        private readonly ICheckpoints checkpoints;
        private readonly IProvenBlockHeaderStore provenBlockHeaderStore;
        private readonly ConnectionManagerSettings connectionManagerSettings;

        public PosConsensusFeature(
            Network network,
            IChainState chainState,
            IConnectionManager connectionManager,
            IConsensusManager consensusManager,
            NodeDeployments nodeDeployments,
            ConcurrentChain chain,
            IInitialBlockDownloadState initialBlockDownloadState,
            IPeerBanning peerBanning,
            Signals.Signals signals,
            ILoggerFactory loggerFactory,
            ICheckpoints checkpoints,
            IProvenBlockHeaderStore provenBlockHeaderStore,
            ConnectionManagerSettings connectionManagerSettings) : base(network, chainState, connectionManager, signals, consensusManager, nodeDeployments)
        {
            this.network = network;
            this.chainState = chainState;
            this.connectionManager = connectionManager;
            this.consensusManager = consensusManager;
            this.nodeDeployments = nodeDeployments;
            this.chain = chain;
            this.initialBlockDownloadState = initialBlockDownloadState;
            this.peerBanning = peerBanning;
            this.loggerFactory = loggerFactory;
            this.checkpoints = checkpoints;
            this.provenBlockHeaderStore = provenBlockHeaderStore;
            this.connectionManagerSettings = connectionManagerSettings;

            this.chainState.MaxReorgLength = network.Consensus.MaxReorgLength;
        }

        /// <inheritdoc />
        public override Task InitializeAsync()
        {
            NetworkPeerConnectionParameters connectionParameters = this.connectionManager.Parameters;

            var defaultConsensusManagerBehavior = connectionParameters.TemplateBehaviors.FirstOrDefault(behavior => behavior is ConsensusManagerBehavior);
            if (defaultConsensusManagerBehavior == null)
            {
                throw new MissingServiceException(typeof(ConsensusManagerBehavior), "Missing expected ConsensusManagerBehavior.");
            }

            // Replace default ConsensusManagerBehavior with ProvenHeadersConsensusManagerBehavior
            connectionParameters.TemplateBehaviors.Remove(defaultConsensusManagerBehavior);
            connectionParameters.TemplateBehaviors.Add(new ProvenHeadersConsensusManagerBehavior(this.chain, this.initialBlockDownloadState, this.consensusManager, this.peerBanning, this.loggerFactory, this.network, this.chainState, this.checkpoints, this.provenBlockHeaderStore, this.connectionManagerSettings));

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override void Dispose()
        {
        }
    }
}