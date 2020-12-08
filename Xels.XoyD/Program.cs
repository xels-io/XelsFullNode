using System;
using System.Threading.Tasks;
using NBitcoin.Protocol;
using Xels.Bitcoin;
using Xels.Bitcoin.Builder;
using Xels.Bitcoin.Configuration;
using Xels.Bitcoin.Features.Api;
using Xels.Bitcoin.Features.BlockStore;
using Xels.Bitcoin.Features.MemoryPool;
using Xels.Bitcoin.Features.RPC;
using Xels.Bitcoin.Features.SignalR;
using Xels.Bitcoin.Features.SignalR.Broadcasters;
using Xels.Bitcoin.Features.SignalR.Events;
using Xels.Bitcoin.Features.SmartContracts;
using Xels.Bitcoin.Features.SmartContracts.PoA;
using Xels.Bitcoin.Features.SmartContracts.Wallet;
using Xels.Bitcoin.Utilities;
using Xels.Features.Collateral;
using Xels.Features.Diagnostic;
using Xels.Features.SQLiteWalletRepository;
using Xels.Sidechains.Networks;

namespace Xels.XoyD
{
    class Program
    {
        public static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        public static async Task MainAsync(string[] args)
        {
            try
            {
                var nodeSettings = new NodeSettings(networksSelector: XoyNetwork.NetworksSelector,
                    protocolVersion: ProtocolVersion.CIRRUS_VERSION, args: args)
                {
                    MinProtocolVersion = ProtocolVersion.ALT_PROTOCOL_VERSION
                };

                IFullNode node = GetSideChainFullNode(nodeSettings);

                if (node != null)
                    await node.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was a problem initializing the node. Details: '{0}'", ex.Message);
            }
        }

        private static IFullNode GetSideChainFullNode(NodeSettings nodeSettings)
        {
            IFullNodeBuilder nodeBuilder = new FullNodeBuilder()
                .UseNodeSettings(nodeSettings)
                .UseBlockStore()
                .UseMempool()
                .AddSmartContracts(options =>
                {
                    options.UseReflectionExecutor();
                    options.UsePoAWhitelistedContracts();
                })
                .UseSmartContractPoAConsensus()
                .UseSmartContractPoAMining() // TODO: this needs to be refactored and removed as it does not make sense to call this for non-mining nodes.
                .CheckForPoAMembersCollateral(false) // This is a non-mining node so we will only check the commitment height data and not do the full set of collateral checks.
                .UseSmartContractWallet()
                .AddSQLiteWalletRepository()
                .UseApi()
                .AddRPC()
                .UseDiagnosticFeature();

            if (nodeSettings.EnableSignalR)
            {
                nodeBuilder.AddSignalR(options =>
                {
                    options.EventsToHandle = new[]
                    {
                        (IClientEvent) new BlockConnectedClientEvent(),
                        new TransactionReceivedClientEvent()
                    };

                    options.ClientEventBroadcasters = new[]
                    {
                        (Broadcaster: typeof(XoyWalletInfoBroadcaster),
                            ClientEventBroadcasterSettings: new ClientEventBroadcasterSettings
                            {
                                BroadcastFrequencySeconds = 5
                            })
                    };
                });
            }

            return nodeBuilder.Build();
        }
    }
}