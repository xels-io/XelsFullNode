using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NBitcoin;
using Xels.Bitcoin.Features.MemoryPool;
using Xels.Bitcoin.Features.Wallet;
using Xels.Bitcoin.Features.Wallet.Controllers;
using Xels.Bitcoin.Features.Wallet.Models;
using Xels.Bitcoin.IntegrationTests.Common;
using Xels.Bitcoin.IntegrationTests.Common.EnvironmentMockUpHelpers;
using Xels.Bitcoin.Tests.Common;
using Xels.Bitcoin.Tests.Common.TestFramework;
using Xunit.Abstractions;

namespace Xels.Bitcoin.IntegrationTests.Wallet
{
    public partial class SendingTransactionWithDoubleSpend : BddSpecification
    {
        private const string Password = "password";
        private const string Name = "mywallet";
        private const string Passphrase = "passphrase";
        private const string AccountName = "account 0";

        private NodeBuilder builder;
        private CoreNode xelsSender;
        private CoreNode xelsReceiver;
        private Transaction transaction;
        private MempoolValidationState mempoolValidationState;
        private HdAddress receivingAddress;

        public SendingTransactionWithDoubleSpend(ITestOutputHelper outputHelper) : base(outputHelper) { }

        protected override void BeforeTest()
        {
            this.builder = NodeBuilder.Create(this);
            this.xelsSender = this.builder.CreateXelsPowNode(KnownNetworks.RegTest).WithWallet().Start();
            this.xelsReceiver = this.builder.CreateXelsPowNode(KnownNetworks.RegTest).WithWallet().Start();
            this.mempoolValidationState = new MempoolValidationState(true);
        }

        protected override void AfterTest()
        {
            this.builder.Dispose();
        }

        private void wallets_with_coins()
        {
            var maturity = (int)this.xelsSender.FullNode.Network.Consensus.CoinbaseMaturity;
            TestHelper.MineBlocks(this.xelsSender, maturity + 5);

            var total = this.xelsSender.FullNode.WalletManager().GetSpendableTransactionsInWallet(Name).Sum(s => s.Transaction.Amount);
            total.Should().Equals(Money.COIN * 6 * 50);

            TestHelper.ConnectAndSync(this.xelsSender, this.xelsReceiver);
        }

        private void coins_first_sent_to_receiving_wallet()
        {
            this.receivingAddress = this.xelsReceiver.FullNode.WalletManager().GetUnusedAddress(new WalletAccountReference(Name, AccountName));

            this.transaction = this.xelsSender.FullNode.WalletTransactionHandler().BuildTransaction(WalletTests.CreateContext(this.xelsSender.FullNode.Network,
                new WalletAccountReference(Name, AccountName), Password, this.receivingAddress.ScriptPubKey, Money.COIN * 100, FeeType.Medium, 101));

            this.xelsSender.FullNode.NodeService<WalletController>().SendTransaction(new SendTransactionRequest(this.transaction.ToHex()));

            TestHelper.WaitLoop(() => this.xelsReceiver.CreateRPCClient().GetRawMempool().Length > 0);
            TestHelper.WaitLoop(() => this.xelsReceiver.FullNode.WalletManager().GetSpendableTransactionsInWallet(Name).Any());

            var receivetotal = this.xelsReceiver.FullNode.WalletManager().GetSpendableTransactionsInWallet(Name).Sum(s => s.Transaction.Amount);
            receivetotal.Should().Equals(Money.COIN * 100);
            this.xelsReceiver.FullNode.WalletManager().GetSpendableTransactionsInWallet(Name).First().Transaction.BlockHeight.Should().BeNull();
        }

        private void txn_mempool_conflict_error_occurs()
        {
            this.mempoolValidationState.Error.Code.Should().BeEquivalentTo("txn-mempool-conflict");
        }

        private void receiving_node_attempts_to_double_spend_mempool_doesnotaccept()
        {
            var unusedAddress = this.xelsReceiver.FullNode.WalletManager().GetUnusedAddress(new WalletAccountReference(Name, AccountName));
            var transactionCloned = this.xelsReceiver.FullNode.Network.CreateTransaction(this.transaction.ToBytes());
            transactionCloned.Outputs[1].ScriptPubKey = unusedAddress.ScriptPubKey;
            this.xelsReceiver.FullNode.MempoolManager().Validator.AcceptToMemoryPool(this.mempoolValidationState, transactionCloned).Result.Should().BeFalse();
        }

        private void trx_is_mined_into_a_block_and_removed_from_mempools()
        {
            TestHelper.MineBlocks(this.xelsSender, 1);
            TestHelper.WaitForNodeToSync(this.xelsSender, this.xelsReceiver);

            this.xelsSender.FullNode.MempoolManager().GetMempoolAsync().Result.Should().NotContain(this.transaction.GetHash());
            this.xelsReceiver.FullNode.MempoolManager().GetMempoolAsync().Result.Should().NotContain(this.transaction.GetHash());
        }

        private void trx_is_propagated_across_sending_and_receiving_mempools()
        {
            List<uint256> senderMempoolTransactions = this.xelsSender.FullNode.MempoolManager().GetMempoolAsync().Result;
            senderMempoolTransactions.Should().Contain(this.transaction.GetHash());

            List<uint256> receiverMempoolTransactions = this.xelsSender.FullNode.MempoolManager().GetMempoolAsync().Result;
            receiverMempoolTransactions.Should().Contain(this.transaction.GetHash());
        }
    }
}