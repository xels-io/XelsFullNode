using System;
using System.Collections.Generic;
using System.Text;
using Xels.Bitcoin.Features.MemoryPool;
using Xels.Bitcoin.IntegrationTests.Common;
using Xels.Bitcoin.IntegrationTests.Common.EnvironmentMockUpHelpers;
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
                TestHelper.WaitLoop(() => node.State == CoreNodeState.Running);
                Assert.False(node.FullNode.NodeService<MempoolSettings>().RequireStandard);
            }
        }

        private class FakeSmartContractMain : SmartContractsPoARegTest
        {
            public FakeSmartContractMain()
            {
                this.Name = "MainnetName"; // Doesn't contain "test" so IsTest() returns false.
            }
        }
    }
}
