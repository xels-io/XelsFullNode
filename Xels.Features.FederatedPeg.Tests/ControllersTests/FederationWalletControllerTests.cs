using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NBitcoin;
using NSubstitute;
using Xels.Bitcoin.Connection;
using Xels.Bitcoin.Features.Wallet;
using Xels.Bitcoin.Features.Wallet.Models;
using Xels.Bitcoin.Networks;
using Xels.Bitcoin.P2P.Peer;
using Xels.Bitcoin.Tests.Common;
using Xels.Bitcoin.Utilities;
using Xels.Features.FederatedPeg.Controllers;
using Xels.Features.FederatedPeg.Interfaces;
using Xels.Features.FederatedPeg.Models;
using Xels.Features.FederatedPeg.TargetChain;
using Xels.Features.FederatedPeg.Wallet;
using Xunit;

namespace Xels.Features.FederatedPeg.Tests.ControllersTests
{
    public class FederationWalletControllerTests
    {
        private readonly ILoggerFactory loggerFactory;
        private readonly IFederationWalletManager walletManager;
        private readonly IFederationWalletSyncManager walletSyncManager;
        private readonly IConnectionManager connectionManager;
        private readonly Network network;
        private readonly ChainIndexer chainIndexer;
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly IWithdrawalHistoryProvider withdrawalHistoryProvider;

        private readonly FederationWalletController controller;
        private readonly FederationWallet fedWallet;

        public FederationWalletControllerTests()
        {
            this.loggerFactory = Substitute.For<ILoggerFactory>();
            this.walletManager = Substitute.For<IFederationWalletManager>();
            this.walletSyncManager = Substitute.For<IFederationWalletSyncManager>();
            this.connectionManager = Substitute.For<IConnectionManager>();
            this.network = new XelsTest();

            this.chainIndexer = new ChainIndexer(this.network);

            ChainedHeader tip = ChainedHeadersHelper.CreateConsecutiveHeaders(100, ChainedHeadersHelper.CreateGenesisChainedHeader(this.network), true, null, this.network).Last();
            this.chainIndexer.SetTip(tip);


            this.dateTimeProvider = Substitute.For<IDateTimeProvider>();
            this.withdrawalHistoryProvider = Substitute.For<IWithdrawalHistoryProvider>();

            this.controller = new FederationWalletController(this.loggerFactory, this.walletManager, this.walletSyncManager,
                this.connectionManager, this.network, this.chainIndexer, this.dateTimeProvider, this.withdrawalHistoryProvider);

            this.fedWallet = new FederationWallet();
            this.fedWallet.Network = this.network;
            this.fedWallet.LastBlockSyncedHeight = 999;
            this.fedWallet.CreationTime = DateTimeOffset.Now;

            this.walletManager.GetWallet().Returns(this.fedWallet);

            var federationWalletManager = (FederationWalletManager)FormatterServices.GetUninitializedObject(typeof(FederationWalletManager));
            PropertyInfo lockProp = typeof(LockProtected).GetProperty("lockObject", BindingFlags.NonPublic | BindingFlags.Instance);
            lockProp.SetValue(federationWalletManager, new object());
            federationWalletManager.Wallet = this.fedWallet;
            this.walletManager.GetSpendableAmount().Returns((x) => {
                return federationWalletManager.GetSpendableAmount();
            });
        }

        [Fact]
        public void GetGeneralInfo()
        {
            this.connectionManager.ConnectedPeers.Returns(info => new NetworkPeerCollection());

            IActionResult result = this.controller.GetGeneralInfo();
            WalletGeneralInfoModel model = this.ActionResultToModel<WalletGeneralInfoModel>(result);

            Assert.Equal(this.fedWallet.CreationTime, model.CreationTime);
            Assert.Equal(this.fedWallet.LastBlockSyncedHeight, model.LastBlockSyncedHeight);
            Assert.Equal(this.fedWallet.Network, model.Network);
        }

        [Fact]
        public void GetBalance()
        {
            this.fedWallet.MultiSigAddress = new MultiSigAddress();

            IActionResult result = this.controller.GetBalance();
            WalletBalanceModel model = this.ActionResultToModel<WalletBalanceModel>(result);

            Assert.Single(model.AccountsBalances);
            Assert.Equal(CoinType.Xels, model.AccountsBalances.First().CoinType);
            Assert.Equal(0, model.AccountsBalances.First().AmountConfirmed.Satoshi);
        }

        [Fact]
        public void GetHistory()
        {
            var withdrawals = new List<WithdrawalModel>() {new WithdrawalModel(), new WithdrawalModel()};

            this.withdrawalHistoryProvider.GetHistory(0).ReturnsForAnyArgs(withdrawals);

            IActionResult result = this.controller.GetHistory(5);
            List<WithdrawalModel> model = this.ActionResultToModel<List<WithdrawalModel>>(result);

            Assert.Equal(withdrawals.Count, model.Count);
        }

        [Fact]
        public void Sync()
        {
            ChainedHeader header = this.chainIndexer.Tip;

            bool called = false;
            this.walletSyncManager.When(x => x.SyncFromHeight(header.Height)).Do(info => called = true);

            this.controller.Sync(new HashModel() { Hash = header.HashBlock.ToString() });

            Assert.True(called);
        }

        [Fact]
        public void EnableFederation()
        {
            bool called = false;
            this.walletManager.When(x => x.EnableFederationWallet(null)).Do(info => called = true);

            this.controller.EnableFederation(new EnableFederationRequest());

            Assert.True(called);
        }

        [Fact]
        public void RemoveTransactions()
        {
            var hashSet = new HashSet<(uint256, DateTimeOffset)>();
            hashSet.Add((uint256.One, DateTimeOffset.MinValue));

            this.walletManager.RemoveAllTransactions().Returns(info => hashSet);

            IActionResult result = this.controller.RemoveTransactions(new RemoveFederationTransactionsModel());

            IEnumerable<RemovedTransactionModel> model = this.ActionResultToModel<IEnumerable<RemovedTransactionModel>>(result);

            Assert.Single(model);
        }

        private T ActionResultToModel<T>(IActionResult result) where T : class
        {
            T model = (result as JsonResult).Value as T;
            return model;
        }
    }
}
