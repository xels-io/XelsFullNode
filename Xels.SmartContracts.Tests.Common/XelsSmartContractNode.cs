using NBitcoin;
using Xels.Bitcoin;
using Xels.Bitcoin.Builder;
using Xels.Bitcoin.Configuration;
using Xels.Bitcoin.Features.BlockStore;
using Xels.Bitcoin.Features.MemoryPool;
using Xels.Bitcoin.Features.RPC;
using Xels.Bitcoin.Features.SmartContracts;
using Xels.Bitcoin.Features.SmartContracts.PoW;
using Xels.Bitcoin.Features.SmartContracts.Wallet;
using Xels.Bitcoin.IntegrationTests.Common;
using Xels.Bitcoin.IntegrationTests.Common.Runners;

namespace Xels.SmartContracts.Tests.Common
{
    public sealed class XelsSmartContractNode : NodeRunner
    {
        public XelsSmartContractNode(string dataDir, Network network)
            : base(dataDir, null)
        {
            this.Network = network;
        }

        public override void BuildNode()
        {
            var settings = new NodeSettings(this.Network, args: new string[] { "-conf=xels.conf", "-datadir=" + this.DataFolder });

            this.FullNode = (FullNode)new FullNodeBuilder()
                .UseNodeSettings(settings)
                .UseBlockStore()
                .UseMempool()
                .AddRPC()
                .AddSmartContracts()
                .UseSmartContractPowConsensus()
                .UseSmartContractWallet()
                .UseSmartContractPowMining()
                .UseReflectionExecutor()
                .MockIBD()
                .UseTestChainedHeaderTree()
                .Build();
        }
    }
}