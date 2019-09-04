using System;
using Xels.Bitcoin.Consensus;
using Xels.Bitcoin.IntegrationTests.Common;
using Xels.Bitcoin.IntegrationTests.Common.EnvironmentMockUpHelpers;
using Xels.Bitcoin.Networks;
using Xunit;

namespace Xels.Bitcoin.IntegrationTests.Miners
{
    public class ProofOfStakeMiningTests
    {
        private class XelsRegTestLastPowBlock : XelsRegTest
        {
            public XelsRegTestLastPowBlock()
            {
                this.Name = Guid.NewGuid().ToString();
            }
        }

        [Fact]
        public void MiningAndPropagatingPOS_MineBlockCheckPeerHasNewBlock()
        {
            using (NodeBuilder nodeBuilder = NodeBuilder.Create(this))
            {
                var network = new XelsRegTest();

                CoreNode node = nodeBuilder.CreateXelsPosNode(network, "posmining-1-node").WithDummyWallet().Start();
                CoreNode syncer = nodeBuilder.CreateXelsPosNode(network, "posmining-1-syncer").Start();

                TestHelper.MineBlocks(node, 1);
                Assert.NotEqual(node.FullNode.ConsensusManager().Tip, syncer.FullNode.ConsensusManager().Tip);

                TestHelper.ConnectAndSync(node, syncer);
                Assert.Equal(node.FullNode.ConsensusManager().Tip, syncer.FullNode.ConsensusManager().Tip);
            }
        }

        [Fact]
        public void MiningAndPropagatingPOS_MineBlockStakeAtInsufficientHeightError()
        {
            using (NodeBuilder nodeBuilder = NodeBuilder.Create(this))
            {
                var network = new XelsRegTestLastPowBlock();

                CoreNode node = nodeBuilder.CreateXelsPosNode(network, "posmining-2-node").WithDummyWallet().Start();

                // Mine two blocks (OK).
                TestHelper.MineBlocks(node, 2);

                // Mine another block after LastPOWBlock height (Error).
                node.FullNode.Network.Consensus.LastPOWBlock = 2;
                var error = Assert.Throws<ConsensusRuleException>(() => TestHelper.MineBlocks(node, 1));
                Assert.True(error.ConsensusError.Message == ConsensusErrors.ProofOfWorkTooHigh.Message);
            }
        }
    }
}