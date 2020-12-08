using System;
using System.Threading.Tasks;
using NBitcoin.Protocol;
using Xels.Bitcoin;
using Xels.Bitcoin.Builder;
using Xels.Bitcoin.Configuration;
using Xels.Bitcoin.Features.Api;
using Xels.Bitcoin.Features.BlockStore;
using Xels.Bitcoin.Features.Dns;
using Xels.Bitcoin.Features.MemoryPool;
using Xels.Bitcoin.Features.RPC;
using Xels.Bitcoin.Features.SmartContracts;
using Xels.Bitcoin.Features.SmartContracts.PoA;
using Xels.Bitcoin.Features.SmartContracts.Wallet;
using Xels.Bitcoin.Utilities;
using Xels.Features.Collateral;
using Xels.Features.SQLiteWalletRepository;
using Xels.Sidechains.Networks;

namespace Xels.XoyDnsD
{
    /// <summary>
    /// Main entry point.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The async entry point for the Xoy Dns process.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        /// <returns>A task used to await the operation.</returns>
        public static async Task Main(string[] args)
        {
            try
            {
                var nodeSettings = new NodeSettings(networksSelector: XoyNetwork.NetworksSelector, protocolVersion: ProtocolVersion.CIRRUS_VERSION, args: args)
                {
                    MinProtocolVersion = ProtocolVersion.ALT_PROTOCOL_VERSION
                };

                var dnsSettings = new DnsSettings(nodeSettings);

                if (string.IsNullOrWhiteSpace(dnsSettings.DnsHostName) || string.IsNullOrWhiteSpace(dnsSettings.DnsNameServer) || string.IsNullOrWhiteSpace(dnsSettings.DnsMailBox))
                    throw new ConfigurationException("When running as a DNS Seed service, the -dnshostname, -dnsnameserver and -dnsmailbox arguments must be specified on the command line.");

                IFullNode node;
                if (dnsSettings.DnsFullNode)
                {
                    // Build the Dns full node.
                    node = GetFederatedPegFullNode(nodeSettings);
                }
                else
                {
                    // Build the Dns node.
                    node = GetFederatedPegDnsNode(nodeSettings);
                }

                // Run node.
                if (node != null)
                    await node.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was a problem initializing the node. Details: '{0}'", ex.ToString());
            }
        }

        private static IFullNode GetFederatedPegFullNode(NodeSettings nodeSettings)
        {
            IFullNode node = new FullNodeBuilder()
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
                .UseDns()
                .Build();

            return node;
        }

        private static IFullNode GetFederatedPegDnsNode(NodeSettings nodeSettings)
        {
            IFullNode node = new FullNodeBuilder()
                .UseNodeSettings(nodeSettings)
                .UseSmartContractPoAConsensus()
                .UseApi()
                .AddRPC()
                .UseDns()
                .AddRPC()
                .Build();

            return node;
        }
    }
}