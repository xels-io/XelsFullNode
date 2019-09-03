using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NBitcoin;
using Xels.Bitcoin.Features.BlockStore;
using Xels.Bitcoin.IntegrationTests.Common;
using Xels.Bitcoin.IntegrationTests.Common.EnvironmentMockUpHelpers;
using Xels.Bitcoin.IntegrationTests.Common.ReadyData;
using Xels.Bitcoin.Networks;
using Xels.Bitcoin.Tests.Common;
using Xels.Bitcoin.Utilities;
using Xunit;

namespace Xels.Bitcoin.IntegrationTests.BlockStore
{
    public class BlockStoreTests
    {
        private readonly ILoggerFactory loggerFactory;
        private readonly Network network;
        private readonly DBreezeSerializer dBreezeSerializer;

        public BlockStoreTests()
        {
            this.loggerFactory = new LoggerFactory();

            this.network = new BitcoinRegTest();
            this.dBreezeSerializer = new DBreezeSerializer(this.network.Consensus.ConsensusFactory);
        }

        [Fact]
        public void BlockRepositoryPutBatch()
        {
            using (var blockRepository = new BlockRepository(this.network, TestBase.CreateDataFolder(this), this.loggerFactory, this.dBreezeSerializer))
            {
                blockRepository.SetTxIndex(true);

                var blocks = new List<Block>();
                for (int i = 0; i < 5; i++)
                {
                    Block block = this.network.CreateBlock();
                    block.AddTransaction(this.network.CreateTransaction());
                    block.AddTransaction(this.network.CreateTransaction());
                    block.Transactions[0].AddInput(new TxIn(Script.Empty));
                    block.Transactions[0].AddOutput(Money.COIN + i * 2, Script.Empty);
                    block.Transactions[1].AddInput(new TxIn(Script.Empty));
                    block.Transactions[1].AddOutput(Money.COIN + i * 2 + 1, Script.Empty);
                    block.UpdateMerkleRoot();
                    block.Header.HashPrevBlock = blocks.Any() ? blocks.Last().GetHash() : this.network.GenesisHash;
                    blocks.Add(block);
                }

                // put
                blockRepository.PutBlocks(new HashHeightPair(blocks.Last().GetHash(), blocks.Count), blocks);

                // check the presence of each block in the repository
                foreach (Block block in blocks)
                {
                    Block received = blockRepository.GetBlock(block.GetHash());
                    Assert.True(block.ToBytes().SequenceEqual(received.ToBytes()));

                    foreach (Transaction transaction in block.Transactions)
                    {
                        Transaction trx = blockRepository.GetTransactionById(transaction.GetHash());
                        Assert.True(trx.ToBytes().SequenceEqual(transaction.ToBytes()));
                    }
                }

                // delete
                blockRepository.Delete(new HashHeightPair(blocks.ElementAt(2).GetHash(), 2), new[] { blocks.ElementAt(2).GetHash() }.ToList());
                Block deleted = blockRepository.GetBlock(blocks.ElementAt(2).GetHash());
                Assert.Null(deleted);
            }
        }

        [Fact]
        public void BlockBroadcastInv()
        {
            using (NodeBuilder builder = NodeBuilder.Create(this))
            {
                CoreNode xelsNodeSync = builder.CreateXelsPowNode(this.network, "bs-1-xelsNodeSync").WithReadyBlockchainData(ReadyBlockchain.BitcoinRegTest10Miner).Start();
                CoreNode xelsNode1 = builder.CreateXelsPowNode(this.network, "bs-1-xelsNode1").WithReadyBlockchainData(ReadyBlockchain.BitcoinRegTest10NoWallet).Start();
                CoreNode xelsNode2 = builder.CreateXelsPowNode(this.network, "bs-1-xelsNode2").WithReadyBlockchainData(ReadyBlockchain.BitcoinRegTest10NoWallet).Start();

                // Sync both nodes
                TestHelper.ConnectAndSync(xelsNode1, xelsNodeSync);
                TestHelper.ConnectAndSync(xelsNode2, xelsNodeSync);

                // Set node2 to use inv (not headers).
                xelsNode2.FullNode.ConnectionManager.ConnectedPeers.First().Behavior<BlockStoreBehavior>().PreferHeaders = false;

                // Generate two new blocks.
                TestHelper.MineBlocks(xelsNodeSync, 2);

                // Wait for the other nodes to pick up the newly generated blocks
                TestBase.WaitLoop(() => TestHelper.AreNodesSynced(xelsNode1, xelsNodeSync));
                TestBase.WaitLoop(() => TestHelper.AreNodesSynced(xelsNode2, xelsNodeSync));
            }
        }

        [Fact(Skip = "Investigate PeerConnector shutdown timeout issue")]
        public void BlockStoreCanRecoverOnStartup()
        {
            using (NodeBuilder builder = NodeBuilder.Create(this))
            {
                CoreNode xelsNodeSync = builder.CreateXelsPowNode(this.network, "bs-2-xelsNodeSync").WithReadyBlockchainData(ReadyBlockchain.BitcoinRegTest10Miner).Start();

                // Set the tip of the best chain to some blocks in the past.
                xelsNodeSync.FullNode.ChainIndexer.SetTip(xelsNodeSync.FullNode.ChainIndexer.GetHeader(xelsNodeSync.FullNode.ChainIndexer.Height - 5));

                // Stop the node to persist the chain with the reset tip.
                xelsNodeSync.FullNode.Dispose();

                CoreNode newNodeInstance = builder.CloneXelsNode(xelsNodeSync);

                // Start the node, this should hit the block store recover code.
                newNodeInstance.Start();

                // Check that the store recovered to be the same as the best chain.
                Assert.Equal(newNodeInstance.FullNode.ChainIndexer.Tip.HashBlock, newNodeInstance.FullNode.GetBlockStoreTip().HashBlock);
            }
        }

        [Fact]
        public void BlockStoreCanReorg()
        {
            using (NodeBuilder builder = NodeBuilder.Create(this))
            {
                CoreNode xelsNodeSync = builder.CreateXelsPowNode(this.network, "bs-3-xelsNodeSync").WithReadyBlockchainData(ReadyBlockchain.BitcoinRegTest10Miner).Start();
                CoreNode xelsNode1 = builder.CreateXelsPowNode(this.network, "bs-3-xelsNode1").WithReadyBlockchainData(ReadyBlockchain.BitcoinRegTest10Listener).Start();
                CoreNode xelsNode2 = builder.CreateXelsPowNode(this.network, "bs-3-xelsNode2").WithReadyBlockchainData(ReadyBlockchain.BitcoinRegTest10Listener).Start();

                // Sync both nodes.
                TestHelper.ConnectAndSync(xelsNodeSync, xelsNode1);
                TestHelper.ConnectAndSync(xelsNodeSync, xelsNode2);

                // Remove node 2.
                TestHelper.Disconnect(xelsNodeSync, xelsNode2);

                // Mine some more with node 1
                TestHelper.MineBlocks(xelsNode1, 10);

                // Wait for node 1 to sync
                TestBase.WaitLoop(() => xelsNode1.FullNode.GetBlockStoreTip().Height == 20);
                TestBase.WaitLoop(() => xelsNode1.FullNode.GetBlockStoreTip().HashBlock == xelsNodeSync.FullNode.GetBlockStoreTip().HashBlock);

                // Remove node 1.
                TestHelper.Disconnect(xelsNodeSync, xelsNode1);

                // Mine a higher chain with node 2.
                TestHelper.MineBlocks(xelsNode2, 20);
                TestBase.WaitLoop(() => xelsNode2.FullNode.GetBlockStoreTip().Height == 30);

                // Add node 2.
                TestHelper.Connect(xelsNodeSync, xelsNode2);

                // Node2 should be synced.
                TestBase.WaitLoop(() => TestHelper.AreNodesSynced(xelsNode2, xelsNodeSync));
            }
        }

        [Fact]
        public void BlockStoreIndexTx()
        {
            using (NodeBuilder builder = NodeBuilder.Create(this))
            {
                CoreNode xelsNode1 = builder.CreateXelsPowNode(this.network, "bs-4-xelsNode1").WithReadyBlockchainData(ReadyBlockchain.BitcoinRegTest10Miner).Start();
                CoreNode xelsNode2 = builder.CreateXelsPowNode(this.network, "bs-4-xelsNode2").WithReadyBlockchainData(ReadyBlockchain.BitcoinRegTest10NoWallet).Start();

                // Sync both nodes.
                TestHelper.ConnectAndSync(xelsNode1, xelsNode2);

                TestBase.WaitLoop(() => xelsNode1.FullNode.GetBlockStoreTip().Height == 10);
                TestBase.WaitLoop(() => xelsNode1.FullNode.GetBlockStoreTip().HashBlock == xelsNode2.FullNode.GetBlockStoreTip().HashBlock);

                Block bestBlock1 = xelsNode1.FullNode.BlockStore().GetBlock(xelsNode1.FullNode.ChainIndexer.Tip.HashBlock);
                Assert.NotNull(bestBlock1);

                // Get the block coinbase trx.
                Transaction trx = xelsNode2.FullNode.BlockStore().GetTransactionById(bestBlock1.Transactions.First().GetHash());
                Assert.NotNull(trx);
                Assert.Equal(bestBlock1.Transactions.First().GetHash(), trx.GetHash());
            }
        }
    }
}
