using NBitcoin;
using Xels.Bitcoin;
using Xels.Bitcoin.Base;
using Xels.Bitcoin.Builder;
using Xels.Bitcoin.Configuration;
using Xels.Bitcoin.Features.BlockStore;
using Xels.Bitcoin.Features.MemoryPool;
using Xels.Bitcoin.Features.PoA.IntegrationTests.Common;
using Xels.Bitcoin.Features.RPC;
using Xels.Bitcoin.Features.SmartContracts;
using Xels.Bitcoin.Features.SmartContracts.PoA;
using Xels.Bitcoin.Features.SmartContracts.Wallet;
using Xels.Bitcoin.IntegrationTests.Common;
using Xels.Bitcoin.IntegrationTests.Common.EnvironmentMockUpHelpers;
using Xels.Bitcoin.IntegrationTests.Common.Runners;
using Xels.Bitcoin.P2P;
using Xels.Bitcoin.Utilities;

namespace Xels.SmartContracts.Tests.Common
{
    public sealed class SmartContractPoARunner : NodeRunner
    {
        private readonly IDateTimeProvider timeProvider;

        public SmartContractPoARunner(string dataDir, Network network, EditableTimeProvider timeProvider)
            : base(dataDir, null)
        {
            this.Network = network;
            this.timeProvider = timeProvider;
        }

        public override void BuildNode()
        {
            var settings = new NodeSettings(this.Network, args: new string[] { "-conf=poa.conf", "-datadir=" + this.DataFolder });

            IFullNodeBuilder builder = new FullNodeBuilder()
                            .UseNodeSettings(settings)
                            .UseBlockStore()
                            .UseMempool()
                            .AddRPC()
                            .AddSmartContracts(options =>
                            {
                                options.UseReflectionExecutor();
                            })
                            .UseSmartContractPoAConsensus()
                            .UseSmartContractPoAMining()
                            .UseSmartContractWallet()
                            .ReplaceTimeProvider(this.timeProvider)
                            .MockIBD()
                            .AddFastMiningCapability();

            if (!this.EnablePeerDiscovery)
            {
                builder.RemoveImplementation<PeerConnectorDiscovery>();
                builder.ReplaceService<IPeerDiscovery, BaseFeature>(new PeerDiscoveryDisabled());
            }

            this.FullNode = (FullNode)builder.Build();
        }
    }
}
