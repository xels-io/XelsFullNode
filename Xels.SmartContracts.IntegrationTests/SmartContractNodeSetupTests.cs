using NBitcoin;
using Xels.Bitcoin.Features.MemoryPool;
using Xels.Bitcoin.IntegrationTests.Common.EnvironmentMockUpHelpers;
using Xels.Bitcoin.Tests.Common;
using Xels.Bitcoin.Utilities;
using Xels.SmartContracts.Networks;
using Xels.SmartContracts.Tests.Common;
using Xunit;

namespace Xels.SmartContracts.IntegrationTests
{
    public class SmartContractNodeSetupTests
    {
        [Fact]
        public void Mainnet_RequireStandard_False()
        {
            var network = new FakeSmartContractMain();
            Assert.False(network.IsTest());

            using (SmartContractNodeBuilder builder = SmartContractNodeBuilder.Create(this))
            {
                var node = builder.CreateSmartContractPoANode(network, 0);
                node.Start();
                TestBase.WaitLoop(() => node.State == CoreNodeState.Running);
                Assert.False(node.FullNode.NodeService<MempoolSettings>().RequireStandard);
            }
        }

        private class FakeSmartContractMain : SmartContractsPoARegTest
        {
            public FakeSmartContractMain()
            {
                this.NetworkType = NetworkType.Mainnet;
            }
        }
    }
}
