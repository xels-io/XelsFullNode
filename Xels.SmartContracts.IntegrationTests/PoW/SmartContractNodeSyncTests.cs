using System.Linq;
using Xels.Bitcoin.Connection;
using Xels.Bitcoin.IntegrationTests.Common;
using Xels.Bitcoin.Tests.Common;
using Xels.SmartContracts.Tests.Common;
using Xunit;

namespace Xels.SmartContracts.IntegrationTests.PoW
{
    public sealed class SmartContractNodeSyncTests
    {
        [Fact]
        public void NodesCanConnectToEachOthers()
        {
            using (SmartContractNodeBuilder builder = SmartContractNodeBuilder.Create(this))
            {
                var node1 = builder.CreateSmartContractPowNode().Start();
                var node2 = builder.CreateSmartContractPowNode().Start();

                Assert.Empty(node1.FullNode.ConnectionManager.ConnectedPeers);
                Assert.Empty(node2.FullNode.ConnectionManager.ConnectedPeers);

                TestHelper.Connect(node1, node2);

                TestBase.WaitLoop(() => node1.FullNode.ConnectionManager.ConnectedPeers.Any());
                TestBase.WaitLoop(() => node2.FullNode.ConnectionManager.ConnectedPeers.Any());

                var behavior = node1.FullNode.ConnectionManager.ConnectedPeers.First().Behaviors.OfType<IConnectionManagerBehavior>().FirstOrDefault();
                Assert.False(behavior.AttachedPeer.Inbound);
                Assert.True(behavior.OneTry);

                behavior = node2.FullNode.ConnectionManager.ConnectedPeers.First().Behaviors.OfType<IConnectionManagerBehavior>().FirstOrDefault();
                Assert.True(behavior.AttachedPeer.Inbound);
                Assert.False(behavior.OneTry);
            }
        }
    }
}