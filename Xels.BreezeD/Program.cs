using System;
using System.Linq;
using System.Threading.Tasks;
using NBitcoin.Protocol;
using Xels.Bitcoin;
using Xels.Bitcoin.Builder;
using Xels.Bitcoin.Configuration;
using Xels.Bitcoin.Features.Api;
using Xels.Bitcoin.Features.BlockStore;
using Xels.Bitcoin.Features.Consensus;
using Xels.Bitcoin.Features.LightWallet;
using Xels.Bitcoin.Features.Notifications;
using Xels.Bitcoin.Networks;
using Xels.Bitcoin.Utilities;
using Xels.Features.SQLiteWalletRepository;

namespace Xels.BreezeD
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                bool isXels = args.Contains("xels");

                NodeSettings nodeSettings;

                IFullNodeBuilder fullNodeBuilder = null;

                if (!args.Any(a => a.Contains("datadirroot")))
                    args = args.Concat(new[] { "-datadirroot=XelsBreeze" }).ToArray();

                if (isXels)
                {
                    nodeSettings = new NodeSettings(networksSelector: Networks.Xels, protocolVersion: ProtocolVersion.PROVEN_HEADER_VERSION, agent: "Breeze", args: args)
                    {
                        MinProtocolVersion = ProtocolVersion.ALT_PROTOCOL_VERSION
                    };

                    fullNodeBuilder = new FullNodeBuilder()
                                    .UseNodeSettings(nodeSettings)
                                    .UseApi()
                                    .UseBlockStore()
                                    .UsePosConsensus()
                                    .UseLightWallet()
                                    .AddSQLiteWalletRepository()
                                    .UseBlockNotification()
                                    .UseTransactionNotification();
                }
                else
                {
                    nodeSettings = new NodeSettings(networksSelector: Networks.Bitcoin, agent: "Breeze", args: args);

                    fullNodeBuilder = new FullNodeBuilder()
                                    .UseNodeSettings(nodeSettings)
                                    .UseApi()
                                    .UseBlockStore()
                                    .UsePowConsensus()
                                    .UseLightWallet()
                                    .AddSQLiteWalletRepository()
                                    .UseBlockNotification()
                                    .UseTransactionNotification();
                }

                IFullNode node = fullNodeBuilder.Build();

                await node.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There was a problem initializing the node: '{ex}'");
            }
        }
    }
}
