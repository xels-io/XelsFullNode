using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;
using Xels.Bitcoin.Base;
using Xels.Bitcoin.Base.Deployments;
using Xels.Bitcoin.Builder.Feature;
using Xels.Bitcoin.Configuration.Settings;
using Xels.Bitcoin.Connection;
using Xels.Bitcoin.Consensus;
using Xels.Bitcoin.P2P.Protocol.Payloads;

[assembly: InternalsVisibleTo("Xels.Bitcoin.Features.Miner.Tests")]
[assembly: InternalsVisibleTo("Xels.Bitcoin.Features.Consensus.Tests")]

namespace Xels.Bitcoin.Features.Consensus
{
    public class ConsensusFeature : FullNodeFeature
    {
        private readonly IChainState chainState;

        private readonly IConnectionManager connectionManager;

        private readonly Signals.ISignals signals;

        private readonly IConsensusManager consensusManager;

        private readonly NodeDeployments nodeDeployments;

        public ConsensusFeature(
            Network network,
            IChainState chainState,
            IConnectionManager connectionManager,
            Signals.ISignals signals,
            IConsensusManager consensusManager,
            NodeDeployments nodeDeployments)
        {
            this.chainState = chainState;
            this.connectionManager = connectionManager;
            this.signals = signals;
            this.consensusManager = consensusManager;
            this.nodeDeployments = nodeDeployments;

            this.chainState.MaxReorgLength = network.Consensus.MaxReorgLength;
        }

        /// <inheritdoc />
        public override Task InitializeAsync()
        {
            DeploymentFlags flags = this.nodeDeployments.GetFlags(this.consensusManager.Tip);
            if (flags.ScriptFlags.HasFlag(ScriptVerify.Witness))
                this.connectionManager.AddDiscoveredNodesRequirement(NetworkPeerServices.NODE_WITNESS);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Prints command-line help.
        /// </summary>
        /// <param name="network">The network to extract values from.</param>
        public static void PrintHelp(Network network)
        {
            ConsensusSettings.PrintHelp(network);
        }

        /// <summary>
        /// Get the default configuration.
        /// </summary>
        /// <param name="builder">The string builder to add the settings to.</param>
        /// <param name="network">The network to base the defaults off.</param>
        public static void BuildDefaultConfigurationFile(StringBuilder builder, Network network)
        {
            ConsensusSettings.BuildDefaultConfigurationFile(builder, network);
        }

        /// <inheritdoc />
        public override void Dispose()
        {
        }
    }
}