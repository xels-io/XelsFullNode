﻿using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NBitcoin;
using Xels.Bitcoin.Features.Wallet;
using Xels.Bitcoin.Features.Wallet.Controllers;
using Xels.Bitcoin.Features.Wallet.Models;
using Xels.Bitcoin.IntegrationTests.Common;
using Xels.Bitcoin.IntegrationTests.Common.EnvironmentMockUpHelpers;
using Xels.Bitcoin.IntegrationTests.Common.TestNetworks;
using Xels.Bitcoin.Tests.Common;
using Xels.Bitcoin.Tests.Common.TestFramework;
using Xunit.Abstractions;

namespace Xels.Bitcoin.IntegrationTests.Wallet
{
    public partial class WalletAddressBuffer : BddSpecification
    {
        private const string WalletName = "mywallet";
        private const string WalletPassword = "password";
        private const string WalletPassphrase = "passphrase";

        public const string AccountZero = "account 0";
        private const string ReceivingNodeName = "receiving";
        private const string SendingNodeName = "sending";

        private CoreNode sendingXelsBitcoinNode;
        private CoreNode receivingXelsBitcoinNode;
        private long walletBalance;
        private NodeBuilder nodeBuilder;
        private Network network;

        protected override void BeforeTest()
        {
            this.nodeBuilder = NodeBuilder.Create(Path.Combine(this.GetType().Name, this.CurrentTest.DisplayName));
            this.network = new BitcoinRegTestOverrideCoinbaseMaturity(1);
        }

        protected override void AfterTest()
        {
            this.nodeBuilder.Dispose();
        }

        public WalletAddressBuffer(ITestOutputHelper output) : base(output)
        {
        }

        private void MineSpendableCoins()
        {
            this.sendingXelsBitcoinNode.FullNode.Network.Consensus.CoinbaseMaturity.Should().Be(this.receivingXelsBitcoinNode.FullNode.Network.Consensus.CoinbaseMaturity);

            TestHelper.MineBlocks(this.sendingXelsBitcoinNode, 2);
        }

        private void a_default_gap_limit_of_20()
        {
            this.sendingXelsBitcoinNode = this.nodeBuilder.CreateXelsPowNode(this.network).WithWallet().Start();
            this.receivingXelsBitcoinNode = this.nodeBuilder.CreateXelsPowNode(this.network).WithWallet().Start();

            TestHelper.ConnectAndSync(this.sendingXelsBitcoinNode, this.receivingXelsBitcoinNode);

            this.MineSpendableCoins();
        }

        private void a_gap_limit_of_21()
        {
            int customUnusedAddressBuffer = 21;
            var configParameters = new NodeConfigParameters { { "walletaddressbuffer", customUnusedAddressBuffer.ToString() } };

            this.sendingXelsBitcoinNode = this.nodeBuilder.CreateXelsPowNode(this.network).WithWallet().Start();
            this.receivingXelsBitcoinNode = this.nodeBuilder.CreateXelsCustomPowNode(this.network, configParameters).WithWallet().Start();

            TestHelper.ConnectAndSync(this.sendingXelsBitcoinNode, this.receivingXelsBitcoinNode);
            this.MineSpendableCoins();
        }

        private async Task a_wallet_with_funds_at_index_20_which_is_beyond_default_gap_limit()
        {
            ExtPubKey xPublicKey = this.GetExtendedPublicKey(this.receivingXelsBitcoinNode);
            var recipientAddressBeyondGapLimit = xPublicKey.Derive(new KeyPath("0/20")).PubKey.GetAddress(KnownNetworks.RegTest);

            TransactionBuildContext transactionBuildContext = TestHelper.CreateTransactionBuildContext(
                this.sendingXelsBitcoinNode.FullNode.Network,
                WalletName,
                AccountZero,
                WalletPassword,
                new[] {
                        new Recipient {
                            Amount = Money.COIN * 1,
                            ScriptPubKey = recipientAddressBeyondGapLimit.ScriptPubKey
                        }
                },
                FeeType.Medium, 0);

            var transaction = this.sendingXelsBitcoinNode.FullNode.WalletTransactionHandler()
                 .BuildTransaction(transactionBuildContext);

            await this.sendingXelsBitcoinNode.FullNode.NodeController<WalletController>()
                .SendTransaction(new SendTransactionRequest(transaction.ToHex()));
                

            TestHelper.MineBlocks(this.sendingXelsBitcoinNode, 1);
        }

        private ExtPubKey GetExtendedPublicKey(CoreNode node)
        {
            ExtKey xPrivKey = node.Mnemonic.DeriveExtKey(WalletPassphrase);
            Key privateKey = xPrivKey.PrivateKey;
            ExtPubKey xPublicKey = HdOperations.GetExtendedPublicKey(privateKey, xPrivKey.ChainCode, (int)CoinType.Bitcoin, 0);
            return xPublicKey;
        }

        private void getting_wallet_balance()
        {
            TestHelper.WaitForNodeToSync(this.sendingXelsBitcoinNode, this.receivingXelsBitcoinNode);

            this.walletBalance = this.receivingXelsBitcoinNode.FullNode.WalletManager()
               .GetSpendableTransactionsInWallet(WalletName)
               .Sum(s => s.Transaction.Amount);
        }

        private void the_balance_is_zero()
        {
            this.walletBalance.Should().Be(0);
        }

        private void _21_new_addresses_are_requested()
        {
            this.receivingXelsBitcoinNode.FullNode.WalletManager()
                .GetUnusedAddresses(new WalletAccountReference(WalletName, AccountZero), 21);
        }

        private void the_balance_is_NOT_zero()
        {
            this.walletBalance.Should().Be(1 * Money.COIN);
        }
    }
}
