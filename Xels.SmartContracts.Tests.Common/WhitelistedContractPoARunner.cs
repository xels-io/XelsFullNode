using NBitcoin;
using Xels.Bitcoin;
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
using Xels.Bitcoin.IntegrationTests.Common.Runners;
using Xels.Bitcoin.Utilities;
using Xels.Features.SQLiteWalletRepository;

namespace Xels.SmartContracts.Tests.Common
{
    public class WhitelistedContractPoARunner : NodeRunner
    {
        private readonly IDateTimeProvider dateTimeProvider;

        public WhitelistedContractPoARunner(string dataDir, Network network, EditableTimeProvider timeProvider)
            : base(dataDir, null)
        {
            this.Network = network;
            this.dateTimeProvider = timeProvider;
        }

        public override void BuildNode()
        {
            var settings = new NodeSettings(this.Network, args: new string[] { "-conf=poa.conf", "-datadir=" + this.DataFolder });

            this.FullNode = (FullNode)new FullNodeBuilder()
                .UseNodeSettings(settings)
                .UseBlockStore()
                // Defer injection of the mempool rules until the smart contract feature.
                .UseMempool()
                .AddRPC()
                .AddSmartContracts(options =>
                {
                    options.UseReflectionExecutor();
                    options.UsePoAWhitelistedContracts();
                })
                .UseSmartContractPoAConsensus()
                .UseSmartContractPoAMining()
                .UseSmartContractWallet()
                .AddSQLiteWalletRepository()
                .ReplaceTimeProvider(this.dateTimeProvider)
                .MockIBD()
                .AddFastMiningCapability()
                .Build();
        }
    }
}