using System;
using System.IO;
using FluentAssertions;
using NBitcoin;
using Xels.Bitcoin.Features.Wallet;
using Xels.Bitcoin.Features.Wallet.Controllers;
using Xels.Bitcoin.Features.Wallet.Models;
using Xels.Bitcoin.IntegrationTests.Common;
using Xels.Bitcoin.IntegrationTests.Common.EnvironmentMockUpHelpers;
using Xels.Bitcoin.Networks;
using Xunit.Abstractions;

namespace Xels.Bitcoin.IntegrationTests.BlockStore
{
    public partial class ProofOfWorkSpendingSpecification
    {
        private const string WalletAccountName = "account 0";
        private const string WalletName = "mywallet";
        private const string WalletPassword = "password";

        private CoreNode sendingXelsBitcoinNode;
        private CoreNode receivingXelsBitcoinNode;
        private int coinbaseMaturity;
        private Exception caughtException;
        private Transaction lastTransaction;

        private NodeBuilder nodeBuilder;
        private Network network;

        public ProofOfWorkSpendingSpecification(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        protected override void BeforeTest()
        {
            this.nodeBuilder = NodeBuilder.Create(Path.Combine(this.GetType().Name, this.CurrentTest.DisplayName));
            this.network = new BitcoinRegTest();
        }

        protected override void AfterTest()
        {
            this.nodeBuilder.Dispose();
        }

        protected void a_sending_and_receiving_xels_bitcoin_node_and_wallet()
        {
            this.sendingXelsBitcoinNode = this.nodeBuilder.CreateXelsPowNode(this.network).WithWallet().Start();
            this.receivingXelsBitcoinNode = this.nodeBuilder.CreateXelsPowNode(this.network).WithWallet().Start();

            TestHelper.Connect(this.sendingXelsBitcoinNode, this.receivingXelsBitcoinNode);

            this.coinbaseMaturity = (int)this.sendingXelsBitcoinNode.FullNode.Network.Consensus.CoinbaseMaturity;
        }

        protected void a_block_is_mined_creating_spendable_coins()
        {
            TestHelper.MineBlocks(this.sendingXelsBitcoinNode, 1);
        }

        private void more_blocks_mined_to_just_BEFORE_maturity_of_original_block()
        {
            TestHelper.MineBlocks(this.sendingXelsBitcoinNode, this.coinbaseMaturity - 1);
        }

        protected void more_blocks_mined_to_just_AFTER_maturity_of_original_block()
        {
            TestHelper.MineBlocks(this.sendingXelsBitcoinNode, this.coinbaseMaturity);
        }

        private void spending_the_coins_from_original_block()
        {
            HdAddress sendtoAddress = this.receivingXelsBitcoinNode.FullNode.WalletManager().GetUnusedAddress();

            try
            {
                TransactionBuildContext transactionBuildContext = TestHelper.CreateTransactionBuildContext(
                    this.sendingXelsBitcoinNode.FullNode.Network,
                    WalletName,
                    WalletAccountName,
                    WalletPassword,
                    new[] {
                        new Recipient {
                            Amount = Money.COIN * 1,
                            ScriptPubKey = sendtoAddress.ScriptPubKey
                        }
                    },
                    FeeType.Medium, 101);

                this.lastTransaction = this.sendingXelsBitcoinNode.FullNode.WalletTransactionHandler()
                    .BuildTransaction(transactionBuildContext);

                this.sendingXelsBitcoinNode.FullNode.NodeService<WalletController>()
                    .SendTransaction(new SendTransactionRequest(this.lastTransaction.ToHex()));
            }
            catch (Exception exception)
            {
                this.caughtException = exception;
            }
        }

        private void the_transaction_is_rejected_from_the_mempool()
        {
            this.caughtException.Should().BeOfType<WalletException>();

            var walletException = (WalletException)this.caughtException;
            walletException.Message.Should().Be("No spendable transactions found.");

            this.ResetCaughtException();
        }

        private void the_transaction_is_put_in_the_mempool()
        {
            Transaction tx = this.sendingXelsBitcoinNode.FullNode.MempoolManager().GetTransaction(this.lastTransaction.GetHash()).GetAwaiter().GetResult();
            tx.GetHash().Should().Be(this.lastTransaction.GetHash());
            this.caughtException.Should().BeNull();
        }

        private void ResetCaughtException()
        {
            this.caughtException = null;
        }
    }
}