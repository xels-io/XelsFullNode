using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NBitcoin;
using Xels.Bitcoin.IntegrationTests.Common;
using Xels.Bitcoin.IntegrationTests.Common.EnvironmentMockUpHelpers;
using Xels.Bitcoin.Networks;
using Xels.Bitcoin.Tests.Common;
using Xunit;

namespace Xels.Bitcoin.IntegrationTests
{
    public class NodeSyncTests
    {
        private readonly Network powNetwork;
        private readonly Network posNetwork;

        public NodeSyncTests()
        {
            this.powNetwork = new BitcoinRegTest();
            this.posNetwork = new XelsRegTest();
        }

        public class XelsRegTestMaxReorg : XelsRegTest
        {
            public XelsRegTestMaxReorg()
            {
                this.Name = Guid.NewGuid().ToString();

                Type consensusType = typeof(NBitcoin.Consensus);
                consensusType.GetProperty("MaxReorgLength").SetValue(this.Consensus, (uint)10);
            }
        }

        [Fact]
        public void Pow_CanXelsSyncFromCore()
        {
            using (NodeBuilder builder = NodeBuilder.Create(this))
            {
                CoreNode xelsNode = builder.CreateXelsPowNode(this.powNetwork).Start();
                CoreNode coreNode = builder.CreateBitcoinCoreNode().Start();

                Block tip = coreNode.FindBlock(10).Last();
                TestHelper.ConnectAndSync(xelsNode, coreNode);

                TestHelper.Disconnect(xelsNode, coreNode);

                coreNode.FindBlock(10).Last();
                TestHelper.ConnectAndSync(coreNode, xelsNode);
            }
        }

        [Fact]
        public void Pow_CanXelsSyncFromXels()
        {
            using (NodeBuilder builder = NodeBuilder.Create(this))
            {
                CoreNode xelsNode = builder.CreateXelsPowNode(this.powNetwork).Start();
                CoreNode xelsNodeSync = builder.CreateXelsPowNode(this.powNetwork).Start();
                CoreNode coreCreateNode = builder.CreateBitcoinCoreNode().Start();

                // first seed a core node with blocks and sync them to a xels node
                // and wait till the xels node is fully synced
                Block tip = coreCreateNode.FindBlock(5).Last();
                TestHelper.ConnectAndSync(xelsNode, coreCreateNode);

                TestBase.WaitLoop(() => xelsNode.FullNode.ConsensusManager().Tip.Block.GetHash() == tip.GetHash());

                // Add a new xels node which will download
                // the blocks using the GetData payload
                TestHelper.ConnectAndSync(xelsNodeSync, xelsNode);
                TestBase.WaitLoop(() => xelsNodeSync.FullNode.ConsensusManager().Tip.Block.GetHash() == tip.GetHash());
            }
        }

        [Fact]
        public void Pow_CanCoreSyncFromXels()
        {
            using (NodeBuilder builder = NodeBuilder.Create(this))
            {
                CoreNode xelsNode = builder.CreateXelsPowNode(this.powNetwork).Start();
                CoreNode coreNodeSync = builder.CreateBitcoinCoreNode().Start();
                CoreNode coreCreateNode = builder.CreateBitcoinCoreNode().Start();

                // first seed a core node with blocks and sync them to a xels node
                // and wait till the xels node is fully synced
                Block tip = coreCreateNode.FindBlock(5).Last();
                TestHelper.ConnectAndSync(xelsNode, coreCreateNode);
                TestBase.WaitLoop(() => xelsNode.FullNode.ConsensusManager().Tip.Block.GetHash() == tip.GetHash());

                // add a new xels node which will download
                // the blocks using the GetData payload
                TestHelper.ConnectAndSync(coreNodeSync, xelsNode);
            }
        }

        [Fact]
        public void Pos_Given_NodesAreSynced_When_ABigReorgHappens_Then_TheReorgIsIgnored()
        {
            using (NodeBuilder builder = NodeBuilder.Create(this))
            {
                var xelsRegTestMaxReorg = new XelsRegTestMaxReorg();

                CoreNode miner = builder.CreateXelsPosNode(xelsRegTestMaxReorg, "ns-5-miner").WithDummyWallet().Start();
                CoreNode syncer = builder.CreateXelsPosNode(xelsRegTestMaxReorg, "ns-5-syncer").Start();
                CoreNode reorg = builder.CreateXelsPosNode(xelsRegTestMaxReorg, "ns-5-reorg").WithDummyWallet().Start();

                TestHelper.MineBlocks(miner, 1);

                // Sync miner with syncer and reorg
                TestHelper.ConnectAndSync(miner, reorg);
                TestHelper.ConnectAndSync(miner, syncer);

                // Create a reorg by mining on two different chains
                TestHelper.Disconnect(miner, reorg);
                TestHelper.Disconnect(miner, syncer);
                TestHelper.MineBlocks(miner, 11);
                TestHelper.MineBlocks(reorg, 12);

                // Make sure the nodes are actually on different chains.
                Assert.NotEqual(miner.FullNode.ChainIndexer.GetHeader(2).HashBlock, reorg.FullNode.ChainIndexer.GetHeader(2).HashBlock);

                TestHelper.ConnectAndSync(miner, syncer);

                // The hash before the reorg node is connected.
                uint256 hashBeforeReorg = miner.FullNode.ChainIndexer.Tip.HashBlock;

                // Connect the reorg chain
                TestHelper.ConnectNoCheck(miner, reorg);
                TestHelper.ConnectNoCheck(syncer, reorg);

                // Wait for the synced chain to get headers updated.
                TestBase.WaitLoop(() => !TestHelper.IsNodeConnected(reorg));

                TestBase.WaitLoop(() => TestHelper.AreNodesSynced(miner, syncer));
                TestBase.WaitLoop(() => TestHelper.AreNodesSynced(reorg, miner) == false);
                TestBase.WaitLoop(() => TestHelper.AreNodesSynced(reorg, syncer) == false);

                // Check that a reorg did not happen.
                Assert.Equal(hashBeforeReorg, syncer.FullNode.ChainIndexer.Tip.HashBlock);
            }
        }

        /// <summary>
        /// This test simulates scenario from issue #862.
        /// <para>
        /// Connection scheme:
        /// Network - Node1 - MiningNode
        /// </para>
        /// </summary>
        [Fact]
        public void Pow_MiningNodeWithOneConnection_AlwaysSynced()
        {
            string testFolderPath = Path.Combine(this.GetType().Name, nameof(Pow_MiningNodeWithOneConnection_AlwaysSynced));

            using (NodeBuilder nodeBuilder = NodeBuilder.Create(testFolderPath))
            {
                CoreNode minerNode = nodeBuilder.CreateXelsPowNode(this.powNetwork).WithDummyWallet().Start();
                CoreNode connectorNode = nodeBuilder.CreateXelsPowNode(this.powNetwork).WithDummyWallet().Start();
                CoreNode firstNode = nodeBuilder.CreateXelsPowNode(this.powNetwork).WithDummyWallet().Start();
                CoreNode secondNode = nodeBuilder.CreateXelsPowNode(this.powNetwork).WithDummyWallet().Start();

                TestHelper.Connect(minerNode, connectorNode);
                TestHelper.Connect(connectorNode, firstNode);
                TestHelper.Connect(connectorNode, secondNode);
                TestHelper.Connect(firstNode, secondNode);

                List<CoreNode> nodes = new List<CoreNode> { minerNode, connectorNode, firstNode, secondNode };

                nodes.ForEach(n =>
                {
                    TestHelper.MineBlocks(n, 1);
                    TestHelper.WaitForNodeToSync(nodes.ToArray());
                });

                Assert.Equal(minerNode.FullNode.ChainIndexer.Height, nodes.Count);

                // Random node on network generates a block.
                TestHelper.MineBlocks(firstNode, 1);
                TestHelper.WaitForNodeToSync(firstNode, connectorNode, secondNode);

                // Miner mines the block.
                TestHelper.MineBlocks(minerNode, 1);
                TestHelper.WaitForNodeToSync(minerNode, connectorNode);

                TestHelper.MineBlocks(connectorNode, 1);

                TestHelper.WaitForNodeToSync(nodes.ToArray());
            }
        }
    }
}
