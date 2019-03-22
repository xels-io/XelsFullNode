using System;
using System.Threading.Tasks;
using NBitcoin.Protocol;
using Xels.Bitcoin.Builder;
using Xels.Bitcoin.Configuration;
using Xels.Bitcoin.Features.Api;
using Xels.Bitcoin.Features.BlockStore;
using Xels.Bitcoin.Features.MemoryPool;
using Xels.Bitcoin.Features.RPC;
using Xels.Bitcoin.Features.SmartContracts;
using Xels.Bitcoin.Features.SmartContracts.Networks;
using Xels.Bitcoin.Features.SmartContracts.PoA;
using Xels.Bitcoin.Features.SmartContracts.PoS;
using Xels.Bitcoin.Features.SmartContracts.Wallet;
using Xels.Bitcoin.Utilities;
using Xels.SmartContracts.Networks;

namespace Xels.XelsSmartContractsD
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
                NodeSettings nodeSettings = new NodeSettings(new SmartContractsPoATest(), ProtocolVersion.ALT_PROTOCOL_VERSION, "XelsSC", args: args);

                Bitcoin.IFullNode node = new FullNodeBuilder()
                    .UseNodeSettings(nodeSettings)
                    .UseBlockStore()
                    .AddRPC()
                        .AddSmartContracts()
                        .UseSmartContractPoAConsensus()
                        .UseSmartContractPoAMining()
                        .UseSmartContractWallet()
                        .UseReflectionExecutor()
                    .UseApi()
                    .UseMempool()
                    .Build();

                await node.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was a problem initializing the node. Details: '{0}'", ex.ToString());
            }
        }
    }
}