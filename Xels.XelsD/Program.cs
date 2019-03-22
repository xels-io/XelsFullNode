using System;
using System.Threading.Tasks;
using NBitcoin.Protocol;
using Xels.Bitcoin;
using Xels.Bitcoin.Builder;
using Xels.Bitcoin.Configuration;
using Xels.Bitcoin.Features.Api;
using Xels.Bitcoin.Features.Apps;
using Xels.Bitcoin.Features.BlockStore;
using Xels.Bitcoin.Features.Consensus;
using Xels.Bitcoin.Features.MemoryPool;
using Xels.Bitcoin.Features.Miner;
using Xels.Bitcoin.Features.RPC;
using Xels.Bitcoin.Features.ColdStaking;
using Xels.Bitcoin.Networks;
using Xels.Bitcoin.Utilities;
using Xels.Bitcoin.Features.BlockExplorer;
using System.Linq;

namespace Xels.XelsD
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                //if (!args.Any(a => a.Contains("datadir")))
                //{
                //    //args = args.Concat(new[] { "apiport=38225" }).ToArray();
                //    args = args.Concat(new[] { "datadir=D:\\MainChainDataFolder\\" }).ToArray();
                //}

                var nodeSettings = new NodeSettings(networksSelector: Networks.Xels, protocolVersion: ProtocolVersion.PROVEN_HEADER_VERSION, args: args)
                {
                    MinProtocolVersion = ProtocolVersion.ALT_PROTOCOL_VERSION
                };

                IFullNode node = new FullNodeBuilder()
                    .UseNodeSettings(nodeSettings)
                    .UseBlockStore()
                    .UsePosConsensus()
                    .UseMempool()
                    .UseColdStakingWallet()
                    .AddPowPosMining()
                    .UseApi()
                    .UseApps()
                    .AddRPC()
                    .UseBlockExplorer()
                    .Build();

                if (node != null)
                    await node.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was a problem initializing the node. Details: '{0}'", ex.ToString());
            }
        }
    }
}
