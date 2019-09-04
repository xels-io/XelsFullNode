using NBitcoin;
using Xels.Bitcoin;
using Xels.Bitcoin.Builder;
using Xels.Bitcoin.Configuration;
using Xels.Bitcoin.Features.Api;
using Xels.Bitcoin.Features.BlockStore;
using Xels.Bitcoin.Features.MemoryPool;
using Xels.Bitcoin.Features.Notifications;
using Xels.Bitcoin.Features.PoA.IntegrationTests.Common;
using Xels.Bitcoin.Features.RPC;
using Xels.Bitcoin.Features.SmartContracts;
using Xels.Bitcoin.Features.SmartContracts.PoA;
using Xels.Bitcoin.Features.SmartContracts.Wallet;
using Xels.Bitcoin.IntegrationTests.Common;
using Xels.Bitcoin.IntegrationTests.Common.Runners;
using Xels.Bitcoin.Utilities;
using Xels.Features.FederatedPeg.Collateral;
using Xels.Features.FederatedPeg.CounterChain;

namespace Xels.Features.FederatedPeg.IntegrationTests.Utils
{
    public class SidechainFederationNodeRunner : NodeRunner
    {
        private bool testingFederation;

        private readonly IDateTimeProvider timeProvider;

        private readonly Network counterChainNetwork;

        public SidechainFederationNodeRunner(string dataDir, string agent, Network network, Network counterChainNetwork, bool testingFederation, IDateTimeProvider dateTimeProvider)
            : base(dataDir, agent)
        {
            this.Network = network;

            this.counterChainNetwork = counterChainNetwork;

            this.testingFederation = testingFederation;

            this.timeProvider = dateTimeProvider;
        }

        public override void BuildNode()
        {
            var settings = new NodeSettings(this.Network, args: new string[] { "-conf=poa.conf", "-datadir=" + this.DataFolder });

            var builder = new FullNodeBuilder()
                .UseNodeSettings(settings)
                .UseBlockStore()
                .SetCounterChainNetwork(this.counterChainNetwork)
                .UseFederatedPegPoAMining()
                .AddFederatedPeg()
                .CheckForPoAMembersCollateral()
                .UseTransactionNotification()
                .UseBlockNotification()
                .UseApi()
                .UseMempool()
                .AddRPC()
                .AddSmartContracts(options =>
                {
                    options.UseReflectionExecutor();
                    options.UsePoAWhitelistedContracts();
                })
                .UseSmartContractWallet()
                .MockIBD()
                .ReplaceTimeProvider(this.timeProvider)
                .AddFastMiningCapability();

            if (!this.testingFederation)
            {
                builder.UseTestFedPegBlockDefinition();
            }

            this.FullNode = (FullNode) builder.Build();
        }
    }
}
