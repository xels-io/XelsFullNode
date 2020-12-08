using Xels.Bitcoin.Builder;
using Xels.Bitcoin.Configuration;
using Xels.Bitcoin.Features.BlockStore;
using Xels.Bitcoin.Features.Consensus;
using Xels.Bitcoin.Features.MemoryPool;
using Xels.Bitcoin.Features.RPC;
using Xels.Bitcoin.Networks;
using Xels.Bitcoin.Tests.Common;

namespace Xels.Bitcoin.IntegrationTests.RPC
{
    /// <summary>
    /// Base class for RPC tests.
    /// </summary>
    public abstract class BaseRPCControllerTest : TestBase
    {
        protected BaseRPCControllerTest() : base(new BitcoinRegTest())
        {
        }

        /// <summary>
        /// Builds a node with basic services and RPC enabled.
        /// </summary>
        /// <param name="dir">Data directory that the node should use.</param>
        /// <returns>Interface to the newly built node.</returns>
        public IFullNode BuildServicedNode(string dir)
        {
            var nodeSettings = new NodeSettings(this.Network, args: new string[] { $"-datadir={dir}" });
            var fullNodeBuilder = new FullNodeBuilder(nodeSettings);
            IFullNode fullNode = fullNodeBuilder
                .UseBlockStore()
                .UsePowConsensus()
                .UseMempool()
                .AddRPC()
                .Build();

            return fullNode;
        }
    }
}
