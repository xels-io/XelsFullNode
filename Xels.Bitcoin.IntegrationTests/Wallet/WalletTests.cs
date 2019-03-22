using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NBitcoin;
using Xels.Bitcoin.Features.Wallet;
using Xels.Bitcoin.Features.Wallet.Controllers;
using Xels.Bitcoin.Features.Wallet.Interfaces;
using Xels.Bitcoin.Features.Wallet.Models;
using Xels.Bitcoin.IntegrationTests.Common;
using Xels.Bitcoin.IntegrationTests.Common.EnvironmentMockUpHelpers;
using Xels.Bitcoin.Networks;
using Xunit;

namespace Xels.Bitcoin.IntegrationTests.Wallet
{
    public class WalletTests
    {
        private const string Password = "password";
        private const string WalletName = "mywallet";
        private const string Passphrase = "passphrase";
        private const string Account = "account 0";
        private readonly Network network;

        public WalletTests()
        {
            this.network = new BitcoinRegTest();
        }

        [Fact]
        public void WalletCanReceiveAndSendCorrectly()
        {
            using (NodeBuilder builder = NodeBuilder.Create(this))
            {
                CoreNode xelsSender = builder.CreateXelsPowNode(this.network).WithWallet().Start();
                CoreNode xelsReceiver = builder.CreateXelsPowNode(this.network).WithWallet().Start();

                int maturity = (int)xelsSender.FullNode.Network.Consensus.CoinbaseMaturity;
                TestHelper.MineBlocks(xelsSender, maturity + 1 + 5);

                // The mining should add coins to the wallet
                long total = xelsSender.FullNode.WalletManager().GetSpendableTransactionsInWallet(WalletName).Sum(s => s.Transaction.Amount);
                Assert.Equal(Money.COIN * 6 * 50, total);

                // Sync both nodes
                TestHelper.ConnectAndSync(xelsSender, xelsReceiver);

                // Send coins to the receiver
                HdAddress sendto = xelsReceiver.FullNode.WalletManager().GetUnusedAddress(new WalletAccountReference(WalletName, Account));
                Transaction trx = xelsSender.FullNode.WalletTransactionHandler().BuildTransaction(CreateContext(xelsSender.FullNode.Network,
                    new WalletAccountReference(WalletName, Account), Password, sendto.ScriptPubKey, Money.COIN * 100, FeeType.Medium, 101));

                // Broadcast to the other node
                xelsSender.FullNode.NodeService<WalletController>().SendTransaction(new SendTransactionRequest(trx.ToHex()));

                // Wait for the transaction to arrive
                TestHelper.WaitLoop(() => xelsReceiver.CreateRPCClient().GetRawMempool().Length > 0);
                TestHelper.WaitLoop(() => xelsReceiver.FullNode.WalletManager().GetSpendableTransactionsInWallet(WalletName).Any());

                long receivetotal = xelsReceiver.FullNode.WalletManager().GetSpendableTransactionsInWallet(WalletName).Sum(s => s.Transaction.Amount);
                Assert.Equal(Money.COIN * 100, receivetotal);
                Assert.Null(xelsReceiver.FullNode.WalletManager().GetSpendableTransactionsInWallet(WalletName).First().Transaction.BlockHeight);

                // Generate two new blocks so the transaction is confirmed
                TestHelper.MineBlocks(xelsSender, 2);

                // Wait for block repo for block sync to work
                TestHelper.WaitLoop(() => TestHelper.AreNodesSynced(xelsReceiver, xelsSender));

                Assert.Equal(Money.Coins(100), xelsReceiver.FullNode.WalletManager().GetBalances(WalletName, Account).Single().AmountConfirmed);
            }
        }

        [Retry(1)]
        public void WalletCanReorg()
        {
            // This test has 4 parts:
            // Send first transaction from one wallet to another and wait for it to be confirmed
            // Send a second transaction and wait for it to be confirmed
            // Connect to a longer chain that causes a reorg so that the second trasnaction is undone
            // Mine the second transaction back in to the main chain
            using (NodeBuilder builder = NodeBuilder.Create(this))
            {
                CoreNode xelsSender = builder.CreateXelsPowNode(this.network).WithWallet().Start();
                CoreNode xelsReceiver = builder.CreateXelsPowNode(this.network).WithWallet().Start();
                CoreNode xelsReorg = builder.CreateXelsPowNode(this.network).WithWallet().Start();

                int maturity = (int)xelsSender.FullNode.Network.Consensus.CoinbaseMaturity;
                TestHelper.MineBlocks(xelsSender, maturity + 1 + 15);

                int currentBestHeight = maturity + 1 + 15;

                // The mining should add coins to the wallet.
                long total = xelsSender.FullNode.WalletManager().GetSpendableTransactionsInWallet(WalletName).Sum(s => s.Transaction.Amount);
                Assert.Equal(Money.COIN * 16 * 50, total);

                // Sync all nodes.
                TestHelper.ConnectAndSync(xelsReceiver, xelsSender);
                TestHelper.ConnectAndSync(xelsReceiver, xelsReorg);
                TestHelper.ConnectAndSync(xelsSender, xelsReorg);

                // Build Transaction 1.
                // Send coins to the receiver.
                HdAddress sendto = xelsReceiver.FullNode.WalletManager().GetUnusedAddress(new WalletAccountReference(WalletName, Account));
                Transaction transaction1 = xelsSender.FullNode.WalletTransactionHandler().BuildTransaction(CreateContext(xelsSender.FullNode.Network, new WalletAccountReference(WalletName, Account), Password, sendto.ScriptPubKey, Money.COIN * 100, FeeType.Medium, 101));

                // Broadcast to the other node.
                xelsSender.FullNode.NodeService<WalletController>().SendTransaction(new SendTransactionRequest(transaction1.ToHex()));

                // Wait for the transaction to arrive.
                TestHelper.WaitLoop(() => xelsReceiver.CreateRPCClient().GetRawMempool().Length > 0);
                Assert.NotNull(xelsReceiver.CreateRPCClient().GetRawTransaction(transaction1.GetHash(), false));
                TestHelper.WaitLoop(() => xelsReceiver.FullNode.WalletManager().GetSpendableTransactionsInWallet(WalletName).Any());

                long receivetotal = xelsReceiver.FullNode.WalletManager().GetSpendableTransactionsInWallet(WalletName).Sum(s => s.Transaction.Amount);
                Assert.Equal(Money.COIN * 100, receivetotal);
                Assert.Null(xelsReceiver.FullNode.WalletManager().GetSpendableTransactionsInWallet(WalletName).First().Transaction.BlockHeight);

                // Generate two new blocks so the transaction is confirmed.
                TestHelper.MineBlocks(xelsSender, 1);
                int transaction1MinedHeight = currentBestHeight + 1;
                TestHelper.MineBlocks(xelsSender, 1);
                currentBestHeight = currentBestHeight + 2;

                // Wait for block repo for block sync to work.
                TestHelper.WaitLoop(() => TestHelper.AreNodesSynced(xelsReceiver, xelsSender));
                TestHelper.WaitLoop(() => TestHelper.AreNodesSynced(xelsReceiver, xelsReorg));
                Assert.Equal(currentBestHeight, xelsReceiver.FullNode.Chain.Tip.Height);
                TestHelper.WaitLoop(() => transaction1MinedHeight == xelsReceiver.FullNode.WalletManager().GetSpendableTransactionsInWallet(WalletName).First().Transaction.BlockHeight);

                // Build Transaction 2.
                // Remove the reorg node.
                TestHelper.Disconnect(xelsReceiver, xelsReorg);
                TestHelper.Disconnect(xelsSender, xelsReorg);

                ChainedHeader forkblock = xelsReceiver.FullNode.Chain.Tip;

                // Send more coins to the wallet
                sendto = xelsReceiver.FullNode.WalletManager().GetUnusedAddress(new WalletAccountReference(WalletName, Account));
                Transaction transaction2 = xelsSender.FullNode.WalletTransactionHandler().BuildTransaction(CreateContext(xelsSender.FullNode.Network, new WalletAccountReference(WalletName, Account), Password, sendto.ScriptPubKey, Money.COIN * 10, FeeType.Medium, 101));
                xelsSender.FullNode.NodeService<WalletController>().SendTransaction(new SendTransactionRequest(transaction2.ToHex()));

                // Wait for the transaction to arrive
                TestHelper.WaitLoop(() => xelsReceiver.CreateRPCClient().GetRawMempool().Length > 0);
                Assert.NotNull(xelsReceiver.CreateRPCClient().GetRawTransaction(transaction2.GetHash(), false));
                TestHelper.WaitLoop(() => xelsReceiver.FullNode.WalletManager().GetSpendableTransactionsInWallet(WalletName).Any());
                long newamount = xelsReceiver.FullNode.WalletManager().GetSpendableTransactionsInWallet(WalletName).Sum(s => s.Transaction.Amount);
                Assert.Equal(Money.COIN * 110, newamount);
                Assert.Contains(xelsReceiver.FullNode.WalletManager().GetSpendableTransactionsInWallet(WalletName), b => b.Transaction.BlockHeight == null);

                // Mine more blocks so it gets included in the chain.
                TestHelper.MineBlocks(xelsSender, 1);
                int transaction2MinedHeight = currentBestHeight + 1;
                TestHelper.MineBlocks(xelsSender, 1);
                currentBestHeight = currentBestHeight + 2;
                TestHelper.WaitLoop(() => TestHelper.AreNodesSynced(xelsReceiver, xelsSender));
                Assert.Equal(currentBestHeight, xelsReceiver.FullNode.Chain.Tip.Height);
                TestHelper.WaitLoop(() => xelsReceiver.FullNode.WalletManager().GetSpendableTransactionsInWallet(WalletName).Any(b => b.Transaction.BlockHeight == transaction2MinedHeight));

                // Create a reorg by mining on two different chains.
                // Advance both chains, one chain is longer.
                TestHelper.MineBlocks(xelsSender, 2);
                TestHelper.MineBlocks(xelsReorg, 10);
                currentBestHeight = forkblock.Height + 10;

                // Connect the reorg chain.
                TestHelper.Connect(xelsReceiver, xelsReorg);
                TestHelper.Connect(xelsSender, xelsReorg);

                // Wait for the chains to catch up.
                TestHelper.WaitLoop(() => TestHelper.AreNodesSynced(xelsReceiver, xelsSender));
                TestHelper.WaitLoop(() => TestHelper.AreNodesSynced(xelsReceiver, xelsReorg, true));
                Assert.Equal(currentBestHeight, xelsReceiver.FullNode.Chain.Tip.Height);

                // Ensure wallet reorg completes.
                TestHelper.WaitLoop(() => xelsReceiver.FullNode.WalletManager().WalletTipHash == xelsReorg.CreateRPCClient().GetBestBlockHash());

                // Check the wallet amount was rolled back.
                long newtotal = xelsReceiver.FullNode.WalletManager().GetSpendableTransactionsInWallet(WalletName).Sum(s => s.Transaction.Amount);
                Assert.Equal(receivetotal, newtotal);
                TestHelper.WaitLoop(() => maturity + 1 + 16 == xelsReceiver.FullNode.WalletManager().GetSpendableTransactionsInWallet(WalletName).First().Transaction.BlockHeight);

                // ReBuild Transaction 2.
                // After the reorg transaction2 was returned back to mempool.
                xelsSender.FullNode.NodeService<WalletController>().SendTransaction(new SendTransactionRequest(transaction2.ToHex()));
                TestHelper.WaitLoop(() => xelsReceiver.CreateRPCClient().GetRawMempool().Length > 0);

                // Mine the transaction again.
                TestHelper.MineBlocks(xelsSender, 1);
                transaction2MinedHeight = currentBestHeight + 1;
                TestHelper.MineBlocks(xelsSender, 1);
                currentBestHeight = currentBestHeight + 2;

                TestHelper.WaitLoop(() => TestHelper.AreNodesSynced(xelsReceiver, xelsSender));
                TestHelper.WaitLoop(() => TestHelper.AreNodesSynced(xelsReceiver, xelsReorg));

                Assert.Equal(currentBestHeight, xelsReceiver.FullNode.Chain.Tip.Height);
                long newsecondamount = xelsReceiver.FullNode.WalletManager().GetSpendableTransactionsInWallet(WalletName).Sum(s => s.Transaction.Amount);
                Assert.Equal(newamount, newsecondamount);
                TestHelper.WaitLoop(() => xelsReceiver.FullNode.WalletManager().GetSpendableTransactionsInWallet(WalletName).Any(b => b.Transaction.BlockHeight == transaction2MinedHeight));
            }
        }

        [Fact]
        public void Given_TheNodeHadAReorg_And_WalletTipIsBehindConsensusTip_When_ANewBlockArrives_Then_WalletCanRecover()
        {
            using (NodeBuilder builder = NodeBuilder.Create(this))
            {
                CoreNode xelsSender = builder.CreateXelsPowNode(this.network).WithWallet().Start();
                CoreNode xelsReceiver = builder.CreateXelsPowNode(this.network).Start();
                CoreNode xelsReorg = builder.CreateXelsPowNode(this.network).WithWallet().Start();

                TestHelper.MineBlocks(xelsSender, 10);

                // Sync all nodes.
                TestHelper.ConnectAndSync(xelsReceiver, xelsSender);
                TestHelper.ConnectAndSync(xelsReceiver, xelsReorg);
                TestHelper.ConnectAndSync(xelsSender, xelsReorg);

                // Remove the reorg node.
                TestHelper.Disconnect(xelsReceiver, xelsReorg);
                TestHelper.Disconnect(xelsSender, xelsReorg);

                // Create a reorg by mining on two different chains.
                // Advance both chains, one chain is longer.
                TestHelper.MineBlocks(xelsSender, 2);
                TestHelper.MineBlocks(xelsReorg, 10);

                // Rewind the wallet for the xelsReceiver node.
                (xelsReceiver.FullNode.NodeService<IWalletSyncManager>() as WalletSyncManager).SyncFromHeight(5);

                // Connect the reorg chain.
                TestHelper.ConnectAndSync(xelsReceiver, xelsReorg);
                TestHelper.ConnectAndSync(xelsSender, xelsReorg);

                // Wait for the chains to catch up.
                TestHelper.WaitLoop(() => TestHelper.AreNodesSynced(xelsReceiver, xelsSender));
                TestHelper.WaitLoop(() => TestHelper.AreNodesSynced(xelsReceiver, xelsReorg));
                Assert.Equal(20, xelsReceiver.FullNode.Chain.Tip.Height);

                TestHelper.MineBlocks(xelsSender, 5);

                TestHelper.WaitLoop(() => TestHelper.AreNodesSynced(xelsReceiver, xelsSender));
                Assert.Equal(25, xelsReceiver.FullNode.Chain.Tip.Height);
            }
        }

        [Fact]
        public void Given_TheNodeHadAReorg_And_ConensusTipIsdifferentFromWalletTip_When_ANewBlockArrives_Then_WalletCanRecover()
        {
            using (NodeBuilder builder = NodeBuilder.Create(this))
            {
                CoreNode xelsSender = builder.CreateXelsPowNode(this.network).WithDummyWallet().Start();
                CoreNode xelsReceiver = builder.CreateXelsPowNode(this.network).Start();
                CoreNode xelsReorg = builder.CreateXelsPowNode(this.network).WithDummyWallet().Start();

                TestHelper.MineBlocks(xelsSender, 10);

                // Sync all nodes.
                TestHelper.ConnectAndSync(xelsReceiver, xelsSender);
                TestHelper.ConnectAndSync(xelsReceiver, xelsReorg);
                TestHelper.ConnectAndSync(xelsSender, xelsReorg);

                // Remove the reorg node and wait for node to be disconnected.
                TestHelper.Disconnect(xelsReceiver, xelsReorg);
                TestHelper.Disconnect(xelsSender, xelsReorg);

                // Create a reorg by mining on two different chains.
                // Advance both chains, one chain is longer.
                TestHelper.MineBlocks(xelsSender, 2);
                TestHelper.MineBlocks(xelsReorg, 10);

                // Connect the reorg chain.
                TestHelper.ConnectAndSync(xelsReceiver, xelsReorg);
                TestHelper.ConnectAndSync(xelsSender, xelsReorg);

                // Wait for the chains to catch up.
                TestHelper.WaitLoop(() => TestHelper.AreNodesSynced(xelsReceiver, xelsSender));
                TestHelper.WaitLoop(() => TestHelper.AreNodesSynced(xelsReceiver, xelsReorg));
                Assert.Equal(20, xelsReceiver.FullNode.Chain.Tip.Height);

                // Rewind the wallet in the xelsReceiver node.
                (xelsReceiver.FullNode.NodeService<IWalletSyncManager>() as WalletSyncManager).SyncFromHeight(10);

                TestHelper.MineBlocks(xelsSender, 5);

                TestHelper.WaitLoop(() => TestHelper.AreNodesSynced(xelsReceiver, xelsSender));
                Assert.Equal(25, xelsReceiver.FullNode.Chain.Tip.Height);
            }
        }

        [Fact]
        public void WalletCanCatchupWithBestChain()
        {
            using (NodeBuilder builder = NodeBuilder.Create(this))
            {
                CoreNode xelsminer = builder.CreateXelsPowNode(this.network).WithWallet().Start();

                TestHelper.MineBlocks(xelsminer, 10);

                // Push the wallet back.
                xelsminer.FullNode.Services.ServiceProvider.GetService<IWalletSyncManager>().SyncFromHeight(5);

                TestHelper.MineBlocks(xelsminer, 5);
            }
        }

        [Retry]
        public void WalletCanRecoverOnStartup()
        {
            using (NodeBuilder builder = NodeBuilder.Create(this))
            {
                CoreNode xelsNodeSync = builder.CreateXelsPowNode(this.network).WithWallet().Start();

                TestHelper.MineBlocks(xelsNodeSync, 10);

                // Set the tip of best chain some blocks in the past
                xelsNodeSync.FullNode.Chain.SetTip(xelsNodeSync.FullNode.Chain.GetBlock(xelsNodeSync.FullNode.Chain.Height - 5));

                // Stop the node (it will persist the chain with the reset tip)
                xelsNodeSync.FullNode.Dispose();

                CoreNode newNodeInstance = builder.CloneXelsNode(xelsNodeSync);

                // Load the node, this should hit the block store recover code
                newNodeInstance.Start();

                // Check that store recovered to be the same as the best chain.
                Assert.Equal(newNodeInstance.FullNode.Chain.Tip.HashBlock, newNodeInstance.FullNode.WalletManager().WalletTipHash);
            }
        }

        public static TransactionBuildContext CreateContext(Network network, WalletAccountReference accountReference, string password,
            Script destinationScript, Money amount, FeeType feeType, int minConfirmations)
        {
            return new TransactionBuildContext(network)
            {
                AccountReference = accountReference,
                MinConfirmations = minConfirmations,
                FeeType = feeType,
                WalletPassword = password,
                Recipients = new[] { new Recipient { Amount = amount, ScriptPubKey = destinationScript } }.ToList()
            };
        }

        /// <summary>
        /// Copies the test wallet into data folder for node if it isn't already present.
        /// </summary>
        /// <param name="path">The path of the folder to move the wallet to.</param>
        private void InitializeTestWallet(string path)
        {
            string testWalletPath = Path.Combine(path, "test.wallet.json");
            if (!File.Exists(testWalletPath))
                File.Copy("Data/test.wallet.json", testWalletPath);
        }
    }
}