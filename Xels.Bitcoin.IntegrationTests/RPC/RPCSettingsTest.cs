using Xels.Bitcoin.Builder;
using Xels.Bitcoin.Configuration;
using Xels.Bitcoin.Features.Consensus;
using Xels.Bitcoin.Features.RPC;
using Xels.Bitcoin.Tests.Common;
using Xunit;

namespace Xels.Bitcoin.IntegrationTests.RPC
{
    public class RPCSettingsTest : TestBase
    {
        public RPCSettingsTest() : base(KnownNetworks.Main)
        {
        }

        [Fact]
        public void CanSpecifyRPCSettings()
        {
            string dir = CreateTestDir(this);

            var nodeSettings = new NodeSettings(this.Network, args: new string[] { $"-datadir={dir}", "-rpcuser=abc", "-rpcpassword=def", "-rpcport=91", "-server=1" });

            IFullNode node = new FullNodeBuilder()
                .UseNodeSettings(nodeSettings)
                .UsePowConsensus()
                .AddRPC()
                .Build();

            var settings = node.NodeService<RpcSettings>();

            Assert.Equal("abc", settings.RpcUser);
            Assert.Equal("def", settings.RpcPassword);
            Assert.Equal(91, settings.RPCPort);
        }
    }
}