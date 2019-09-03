using NBitcoin;
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
using Xels.Bitcoin.Interfaces;
using Xels.Bitcoin.P2P;

namespace Xels.Bitcoin.IntegrationTests.Common.Runners
{
    public sealed class XelsBitcoinPowRunner : NodeRunner
    {
        public XelsBitcoinPowRunner(string dataDir, Network network, string agent)
            : base(dataDir, agent)
        {
            this.Network = network;
        }

        public override void BuildNode()
        {
            NodeSettings settings = null;

            if (string.IsNullOrEmpty(this.Agent))
                settings = new NodeSettings(this.Network, args: new string[] { "-conf=bitcoin.conf", "-datadir=" + this.DataFolder });
            else
                settings = new NodeSettings(this.Network, agent: this.Agent, args: new string[] { "-conf=bitcoin.conf", "-datadir=" + this.DataFolder });

            var builder = new FullNodeBuilder()
                            .UseNodeSettings(settings)
                            .UseBlockStore()
                            .UsePowConsensus()
                            .UseMempool()
                            .AddMining()
                            .UseWallet()
                            .AddRPC()
                            .UseApi()
                            .UseTestChainedHeaderTree()
                            .MockIBD();

            if (this.ServiceToOverride != null)
                builder.OverrideService<BaseFeature>(this.ServiceToOverride);

            if (!this.EnablePeerDiscovery)
            {
                builder.RemoveImplementation<PeerConnectorDiscovery>();
                builder.ReplaceService<IPeerDiscovery, BaseFeature>(new PeerDiscoveryDisabled());
            }

            if (this.AlwaysFlushBlocks)
            {
                builder.ReplaceService<IBlockStoreQueueFlushCondition, BlockStoreFeature>(new BlockStoreAlwaysFlushCondition());
            }

            this.FullNode = (FullNode)builder.Build();
        }
    }
}