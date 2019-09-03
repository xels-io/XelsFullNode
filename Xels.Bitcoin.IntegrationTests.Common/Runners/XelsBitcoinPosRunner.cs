using NBitcoin;
using NBitcoin.Protocol;
using Xels.Bitcoin.Base;
using Xels.Bitcoin.Builder;
using Xels.Bitcoin.Configuration;
using Xels.Bitcoin.Features.Api;
using Xels.Bitcoin.Features.BlockStore;
using Xels.Bitcoin.Features.Consensus;
using Xels.Bitcoin.Features.MemoryPool;
using Xels.Bitcoin.Features.Miner;
using Xels.Bitcoin.Features.RPC;
using Xels.Bitcoin.Features.Wallet;
using Xels.Bitcoin.IntegrationTests.Common.EnvironmentMockUpHelpers;
using Xels.Bitcoin.P2P;

namespace Xels.Bitcoin.IntegrationTests.Common.Runners
{
    public sealed class XelsBitcoinPosRunner : NodeRunner
    {
        public XelsBitcoinPosRunner(string dataDir, Network network, string agent = "XelsBitcoin")
            : base(dataDir, agent)
        {
            this.Network = network;
        }

        public override void BuildNode()
        {
            var settings = new NodeSettings(this.Network, ProtocolVersion.PROVEN_HEADER_VERSION, this.Agent, args: new string[] { "-conf=xels.conf", "-datadir=" + this.DataFolder });

            var builder = new FullNodeBuilder()
                .UseNodeSettings(settings)
                .UseBlockStore()
                .UsePosConsensus()
                .UseMempool()
                .UseWallet()
                .AddPowPosMining()
                .AddRPC()
                .UseApi()
                .UseTestChainedHeaderTree()
                .MockIBD();

            if (this.OverrideDateTimeProvider)
                builder.OverrideDateTimeProviderFor<MiningFeature>();

            if (!this.EnablePeerDiscovery)
            {
                builder.RemoveImplementation<PeerConnectorDiscovery>();
                builder.ReplaceService<IPeerDiscovery, BaseFeature>(new PeerDiscoveryDisabled());
            }

            this.FullNode = (FullNode)builder.Build();
        }

        /// <summary>
        /// Builds a node with POS miner and RPC enabled.
        /// </summary>
        /// <param name="dataDir">Data directory that the node should use.</param>
        /// <param name="staking">Flag to signal that the node should the start staking on start up or not.</param>
        /// <returns>Interface to the newly built node.</returns>
        /// <remarks>Currently the node built here does not actually stake as it has no coins in the wallet,
        /// but all the features required for it are enabled.</remarks>
        public static IFullNode BuildStakingNode(string dataDir, bool staking = true)
        {
            var nodeSettings = new NodeSettings(networksSelector: Networks.Networks.Xels, protocolVersion: ProtocolVersion.PROVEN_HEADER_VERSION, args: new string[] { $"-datadir={dataDir}", $"-stake={(staking ? 1 : 0)}", "-walletname=dummy", "-walletpassword=dummy" });
            var fullNodeBuilder = new FullNodeBuilder(nodeSettings);
            IFullNode fullNode = fullNodeBuilder
                                .UseBlockStore()
                                .UsePosConsensus()
                                .UseMempool()
                                .UseWallet()
                                .AddPowPosMining()
                                .AddRPC()
                                .MockIBD()
                                .UseTestChainedHeaderTree()
                                .Build();

            return fullNode;
        }
    }
}