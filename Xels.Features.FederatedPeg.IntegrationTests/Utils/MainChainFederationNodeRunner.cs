using NBitcoin;
using NBitcoin.Protocol;
using Xels.Bitcoin;
using Xels.Bitcoin.Builder;
using Xels.Bitcoin.Configuration;
using Xels.Bitcoin.Features.Api;
using Xels.Bitcoin.Features.BlockStore;
using Xels.Bitcoin.Features.Consensus;
using Xels.Bitcoin.Features.MemoryPool;
using Xels.Bitcoin.Features.Miner;
using Xels.Bitcoin.Features.Notifications;
using Xels.Bitcoin.Features.RPC;
using Xels.Bitcoin.Features.Wallet;
using Xels.Bitcoin.IntegrationTests.Common;
using Xels.Bitcoin.IntegrationTests.Common.Runners;
using Xels.Features.FederatedPeg.CounterChain;

namespace Xels.Features.FederatedPeg.IntegrationTests.Utils
{
    /// <summary>
    /// To emulate the behaviour of a main chain node in FederationGatewayD.
    /// </summary>
    public class MainChainFederationNodeRunner : NodeRunner
    {
        private Network counterChainNetwork;

        public MainChainFederationNodeRunner(string dataDir, string agent, Network network, Network counterChainNetwork)
            : base(dataDir, agent)
        {
            this.Network = network;
            this.counterChainNetwork = counterChainNetwork;
        }

        public override void BuildNode()
        {
            var settings = new NodeSettings(this.Network, ProtocolVersion.PROVEN_HEADER_VERSION, args: new string[] { "-conf=xels.conf", "-datadir=" + this.DataFolder });

            this.FullNode = (FullNode)new FullNodeBuilder()
                .UseNodeSettings(settings)
                .UseBlockStore()
                .SetCounterChainNetwork(this.counterChainNetwork)
                .AddFederatedPeg()
                .UseTransactionNotification()
                .UseBlockNotification()
                .UseApi()
                .UseMempool()
                .AddRPC()
                .UsePosConsensus()
                .UseWallet()
                .AddPowPosMining()
                .MockIBD()
                .Build();
        }
    }
}
