using System;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;
using NBitcoin.Protocol;
using Xels.Bitcoin;
using Xels.Bitcoin.Builder;
using Xels.Bitcoin.Configuration;
using Xels.Bitcoin.Features.Api;
using Xels.Bitcoin.Features.BlockStore;
using Xels.Bitcoin.Features.Dns;
using Xels.Bitcoin.Features.MemoryPool;
using Xels.Bitcoin.Features.PoA;
using Xels.Bitcoin.Features.RPC;
using Xels.Bitcoin.Features.SmartContracts;
using Xels.Bitcoin.Features.SmartContracts.PoA;
using Xels.Bitcoin.Features.SmartContracts.Wallet;
using Xels.Bitcoin.Utilities;
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
                var nodeSettings = new NodeSettings(networksSelector: XoyNetwork.NetworksSelector, protocolVersion: ProtocolVersion.ALT_PROTOCOL_VERSION, args: args);

                bool keyGenerationRequired = nodeSettings.ConfigReader.GetOrDefault("generateKeyPair", false);
                if (keyGenerationRequired)
                {
                    GenerateFederationKey(nodeSettings.DataFolder);
                    return;
                }

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
                .UseSmartContractPoAMining()
                .UseSmartContractWallet()
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

        private static void GenerateFederationKey(DataFolder dataFolder)
        {
            var tool = new KeyTool(dataFolder);
            Key key = tool.GeneratePrivateKey();

            string savePath = tool.GetPrivateKeySavePath();
            tool.SavePrivateKey(key);

            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"Federation key pair was generated and saved to {savePath}.");
            stringBuilder.AppendLine("Make sure to back it up!");
            stringBuilder.AppendLine($"Your public key is {key.PubKey}.");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("Press eny key to exit...");

            Console.WriteLine(stringBuilder.ToString());

            Console.ReadKey();
        }
    }
}