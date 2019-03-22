using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using NBitcoin;
using NBitcoin.BouncyCastle.Math;
using Xels.Bitcoin.Connection;
using Xels.Bitcoin.IntegrationTests.Common;
using Xels.Bitcoin.IntegrationTests.Common.EnvironmentMockUpHelpers;
using Xels.Bitcoin.Networks;
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

        private class XelsRegTestMaxReorg : XelsRegTest
        {
            public XelsRegTestMaxReorg()
            {
                this.Consensus = new NBitcoin.Consensus(
                consensusFactory: base.Consensus.ConsensusFactory,
                consensusOptions: base.Consensus.Options,
                coinType: 105,
                hashGenesisBlock: base.GenesisHash,
                subsidyHalvingInterval: 210000,
                majorityEnforceBlockUpgrade: 750,
                majorityRejectBlockOutdated: 950,
                majorityWindow: 1000,
                buriedDeployments: base.Consensus.BuriedDeployments,
                bip9Deployments: base.Consensus.BIP9Deployments,
                bip34Hash: new uint256("0x000000000000024b89b42a942fe0d9fea3bb44ab7bd1b19115dd6a759c0808b8"),
                ruleChangeActivationThreshold: 1916, // 95% of 2016
                minerConfirmationWindow: 2016, // nPowTargetTimespan / nPowTargetSpacing
                maxReorgLength: 10,
                defaultAssumeValid: null, // turn off assumevalid for regtest.
                maxMoney: long.MaxValue,
                coinbaseMaturity: 10,
                premineHeight: 2,
                premineReward: Money.Coins(98000000),
                proofOfWorkReward: Money.Coins(4),
                powTargetTimespan: TimeSpan.FromSeconds(14 * 24 * 60 * 60), // two weeks
                powTargetSpacing: TimeSpan.FromSeconds(10 * 60),
                powAllowMinDifficultyBlocks: true,
                powNoRetargeting: true,
                powLimit: base.Consensus.PowLimit,
                minimumChainWork: null,
                isProofOfStake: true,
                lastPowBlock: 12500,
                proofOfStakeLimit: new BigInteger(uint256.Parse("00000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffff").ToBytes(false)),
                proofOfStakeLimitV2: new BigInteger(uint256.Parse("000000000000ffffffffffffffffffffffffffffffffffffffffffffffffffff").ToBytes(false)),
                proofOfStakeReward: Money.COIN);

                this.Name = Guid.NewGuid().ToString();
            }
        }

        [Fact]
        public void Pow_NodesCanConnectToEachOthers()
        {
            using (NodeBuilder builder = NodeBuilder.Create(this))
            {
                CoreNode node1 = builder.CreateXelsPowNode(this.powNetwork).Start();
                CoreNode node2 = builder.CreateXelsPowNode(this.powNetwork).Start();

                Assert.Empty(node1.FullNode.ConnectionManager.ConnectedPeers);
                Assert.Empty(node2.FullNode.ConnectionManager.ConnectedPeers);

                TestHelper.Connect(node1, node2);
                Assert.Single(node1.FullNode.ConnectionManager.ConnectedPeers);
                Assert.Single(node2.FullNode.ConnectionManager.ConnectedPeers);

                var behavior = node1.FullNode.ConnectionManager.ConnectedPeers.First().Behaviors.OfType<IConnectionManagerBehavior>().FirstOrDefault();
                Assert.False(behavior.AttachedPeer.Inbound);
                Assert.True(behavior.OneTry);
                behavior = node2.FullNode.ConnectionManager.ConnectedPeers.First().Behaviors.OfType<IConnectionManagerBehavior>().FirstOrDefault();
                Assert.True(behavior.AttachedPeer.Inbound);
                Assert.False(behavior.OneTry);
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

                TestHelper.WaitLoop(() => xelsNode.FullNode.ConsensusManager().Tip.Block.GetHash() == tip.GetHash());

                // Add a new xels node which will download
                // the blocks using the GetData payload
                TestHelper.ConnectAndSync(xelsNodeSync, xelsNode);
                TestHelper.WaitLoop(() => xelsNodeSync.FullNode.ConsensusManager().Tip.Block.GetHash() == tip.GetHash());
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
                TestHelper.WaitLoop(() => xelsNode.FullNode.ConsensusManager().Tip.Block.GetHash() == tip.GetHash());

                // add a new xels node which will download
                // the blocks using the GetData payload
                TestHelper.ConnectAndSync(coreNodeSync, xelsNode);
            }
        }

        [Fact]
        [Trait("Unstable", "True")]
        public void Pos_Given_NodesAreSynced_When_ABigReorgHappens_Then_TheReorgIsIgnored()
        {
            using (NodeBuilder builder = NodeBuilder.Create(this))
            {
                var xelsRegTestMaxReorg = new XelsRegTestMaxReorg();
                CoreNode xelsMiner = builder.CreateXelsPosNode(xelsRegTestMaxReorg, nameof(xelsMiner)).WithDummyWallet().Start();
                CoreNode xelsSyncer = builder.CreateXelsPosNode(xelsRegTestMaxReorg, nameof(xelsSyncer)).Start();
                CoreNode xelsReorg = builder.CreateXelsPosNode(xelsRegTestMaxReorg, nameof(xelsReorg)).WithDummyWallet().Start();

                TestHelper.MineBlocks(xelsMiner, 1);

                // Wait for block repo for block sync to work
                TestHelper.ConnectAndSync(xelsMiner, xelsReorg);
                TestHelper.ConnectAndSync(xelsMiner, xelsSyncer);

                // Create a reorg by mining on two different chains
                TestHelper.Disconnect(xelsMiner, xelsReorg);
                TestHelper.Disconnect(xelsMiner, xelsSyncer);

                TestHelper.MineBlocks(xelsMiner, 11);
                TestHelper.MineBlocks(xelsReorg, 12);

                // Make sure the nodes are actually on different chains.
                Assert.NotEqual(xelsMiner.FullNode.Chain.GetBlock(2).HashBlock, xelsReorg.FullNode.Chain.GetBlock(2).HashBlock);

                TestHelper.ConnectAndSync(xelsSyncer, xelsMiner);

                // The hash before the reorg node is connected.
                uint256 hashBeforeReorg = xelsMiner.FullNode.Chain.Tip.HashBlock;

                // Connect the reorg chain
                TestHelper.Connect(xelsMiner, xelsReorg);
                TestHelper.Connect(xelsSyncer, xelsReorg);

                // Trigger nodes to sync
                TestHelper.TriggerSync(xelsMiner);
                TestHelper.TriggerSync(xelsReorg);
                TestHelper.TriggerSync(xelsSyncer);

                // Wait for the synced chain to get headers updated.
                TestHelper.WaitLoop(() => !xelsReorg.FullNode.ConnectionManager.ConnectedPeers.Any());

                TestHelper.WaitLoop(() => TestHelper.AreNodesSynced(xelsMiner, xelsSyncer));
                TestHelper.WaitLoop(() => TestHelper.AreNodesSynced(xelsReorg, xelsMiner) == false);
                TestHelper.WaitLoop(() => TestHelper.AreNodesSynced(xelsReorg, xelsSyncer) == false);

                // Check that a reorg did not happen.
                Assert.Equal(hashBeforeReorg, xelsSyncer.FullNode.Chain.Tip.HashBlock);
            }
        }

        /// <summary>
        /// This tests simulates scenario 2 from issue 636.
        /// <para>
        /// The test mines a block and roughly at the same time, but just after that, a new block at the same height
        /// arrives from the puller. Then another block comes from the puller extending the chain without the block we mined.
        /// </para>
        /// </summary>
        /// <seealso cref="https://github.com/xelsproject/XelsBitcoinFullNode/issues/636"/>
        [Fact]
        [Trait("Unstable", "True")]
        public void Pos_PullerVsMinerRaceCondition()
        {
            using (NodeBuilder builder = NodeBuilder.Create(this))
            {
                var xelsRegTest = new XelsRegTest();

                // This represents local node.
                CoreNode xelsMinerLocal = builder.CreateXelsPosNode(xelsRegTest).OverrideDateTimeProvider().WithDummyWallet().Start();

                // This represents remote, which blocks are received by local node using its puller.
                CoreNode xelsMinerRemote = builder.CreateXelsPosNode(xelsRegTest).OverrideDateTimeProvider().WithDummyWallet().Start();

                // Let's mine block Ap and Bp.
                TestHelper.MineBlocks(xelsMinerRemote, 2);

                // Wait for block repository for block sync to work.
                TestHelper.ConnectAndSync(xelsMinerLocal, xelsMinerRemote);

                // Now disconnect the peers and mine block C2p on remote.
                TestHelper.Disconnect(xelsMinerLocal, xelsMinerRemote);

                // Mine block C2p.
                TestHelper.MineBlocks(xelsMinerRemote, 1);
                Thread.Sleep(2000);

                // Now reconnect nodes and mine block C1s before C2p arrives.
                TestHelper.Connect(xelsMinerLocal, xelsMinerRemote);
                TestHelper.MineBlocks(xelsMinerLocal, 1);

                // Mine block Dp.
                uint256 dpHash = TestHelper.MineBlocks(xelsMinerRemote, 1, false).BlockHashes[0];

                // Now we wait until the local node's chain tip has correct hash of Dp.
                TestHelper.WaitLoop(() => xelsMinerLocal.FullNode.Chain.Tip.HashBlock.Equals(dpHash));

                // Then give it time to receive the block from the puller.
                Thread.Sleep(2500);

                // Check that local node accepted the Dp as consensus tip.
                Assert.Equal(xelsMinerLocal.FullNode.ChainBehaviorState.ConsensusTip.HashBlock, dpHash);
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

                Assert.Equal(minerNode.FullNode.Chain.Height, nodes.Count);

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

        [Fact]
        [Trait("Unstable", "True")]
        public void Pos_NodesCanConnect_AndSync_AndMineBlocks()
        {
            using (NodeBuilder builder = NodeBuilder.Create(this))
            {
                var minerA = builder.CreateXelsPosNode(this.posNetwork).WithDummyWallet().Start();
                var minerB = builder.CreateXelsPosNode(this.posNetwork).WithDummyWallet().Start();
                var syncer = builder.CreateXelsPosNode(this.posNetwork).WithDummyWallet().Start();

                // MinerA mines to height 1.
                TestHelper.MineBlocks(minerA, 1);

                // Sync the network to height 1.
                TestHelper.ConnectAndSync(syncer, minerA);

                // MinerA mines to height 2.
                TestHelper.MineBlocks(minerA, 1);

                // Sync minerB to height 2.
                TestHelper.ConnectAndSync(syncer, minerB);

                // MinerB mines to height 3.
                TestHelper.MineBlocks(minerB, 1);

                TestHelper.WaitLoop(() => TestHelper.AreNodesSynced(syncer, minerA));
                TestHelper.WaitLoop(() => TestHelper.AreNodesSynced(syncer, minerB));
            }
        }
    }
}
