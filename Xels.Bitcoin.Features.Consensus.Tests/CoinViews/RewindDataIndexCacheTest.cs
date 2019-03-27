﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NBitcoin;
using NBitcoin.BitcoinCore;
using Xels.Bitcoin.Features.Consensus.CoinViews;
using Xels.Bitcoin.Features.Consensus.ProvenBlockHeaders;
using Xels.Bitcoin.Interfaces;
using Xels.Bitcoin.Networks;
using Xels.Bitcoin.Tests.Common;
using Xels.Bitcoin.Tests.Common.Logging;
using Xels.Bitcoin.Utilities;
using Xunit;

namespace Xels.Bitcoin.Features.Consensus.Tests.CoinViews
{
    public class RewindDataIndexCacheTest : LogsTestBase
    {
        private readonly ProvenBlockHeaderStore provenBlockHeaderStore;
        private readonly IProvenBlockHeaderRepository provenBlockHeaderRepository;

        public RewindDataIndexCacheTest() : base(new XelsTest())
        {
            // override max reorg to 10
            Type consensusType = typeof(NBitcoin.Consensus);
            consensusType.GetProperty("MaxReorgLength").SetValue(this.Network.Consensus, (uint)10);

        }

        [Fact]
        public async Task RewindDataIndex_InitialiseCache_BelowMaxREprg()
        {
            Mock<IDateTimeProvider> dateTimeProviderMock = new Mock<IDateTimeProvider>();
            Mock<ICoinView> coinViewMock = new Mock<ICoinView>();
            this.SetupMockCoinView(coinViewMock);

            RewindDataIndexCache rewindDataIndexCache = new RewindDataIndexCache(dateTimeProviderMock.Object, this.Network);

            await rewindDataIndexCache.InitializeAsync(5, coinViewMock.Object);

            var items = rewindDataIndexCache.GetMemberValue("items") as ConcurrentDictionary<string, int>;

            items.Should().HaveCount(10);
            this.CheckCache(items, 5, 1);
        }

        [Fact]
        public async Task RewindDataIndex_InitialiseCache()
        {
            Mock<IDateTimeProvider> dateTimeProviderMock = new Mock<IDateTimeProvider>();
            Mock<ICoinView> coinViewMock = new Mock<ICoinView>();
            this.SetupMockCoinView(coinViewMock);

            RewindDataIndexCache rewindDataIndexCache = new RewindDataIndexCache(dateTimeProviderMock.Object, this.Network);

            await rewindDataIndexCache.InitializeAsync(20, coinViewMock.Object);

            var items = rewindDataIndexCache.GetMemberValue("items") as ConcurrentDictionary<string, int>;

            items.Should().HaveCount(22);
            this.CheckCache(items, 20, 10);
        }

        [Fact]
        public async Task RewindDataIndex_Save()
        {
            Mock<IDateTimeProvider> dateTimeProviderMock = new Mock<IDateTimeProvider>();
            Mock<ICoinView> coinViewMock = new Mock<ICoinView>();
            this.SetupMockCoinView(coinViewMock);

            RewindDataIndexCache rewindDataIndexCache = new RewindDataIndexCache(dateTimeProviderMock.Object, this.Network);

            await rewindDataIndexCache.InitializeAsync(20, coinViewMock.Object);

            rewindDataIndexCache.Save(new Dictionary<string, int>() { { $"{ new uint256(21) }-{ 0 }", 21}});
            var items = rewindDataIndexCache.GetMemberValue("items") as ConcurrentDictionary<string, int>;

            items.Should().HaveCount(23);
            this.CheckCache(items, 21, 10);
        }

        [Fact]
        public async Task RewindDataIndex_Flush()
        {
            Mock<IDateTimeProvider> dateTimeProviderMock = new Mock<IDateTimeProvider>();
            Mock<ICoinView> coinViewMock = new Mock<ICoinView>();
            this.SetupMockCoinView(coinViewMock);

            RewindDataIndexCache rewindDataIndexCache = new RewindDataIndexCache(dateTimeProviderMock.Object, this.Network);

            await rewindDataIndexCache.InitializeAsync(20, coinViewMock.Object);

            rewindDataIndexCache.Flush(15);
            var items = rewindDataIndexCache.GetMemberValue("items") as ConcurrentDictionary<string, int>;

            items.Should().HaveCount(12);
            this.CheckCache(items, 15, 9);
        }

        [Fact]
        public async Task RewindDataIndex_Remove()
        {
            Mock<IDateTimeProvider> dateTimeProviderMock = new Mock<IDateTimeProvider>();
            Mock<ICoinView> coinViewMock = new Mock<ICoinView>();
            this.SetupMockCoinView(coinViewMock);

            RewindDataIndexCache rewindDataIndexCache = new RewindDataIndexCache(dateTimeProviderMock.Object, this.Network);

            await rewindDataIndexCache.InitializeAsync(20, coinViewMock.Object);

            await rewindDataIndexCache.Remove(19, coinViewMock.Object);
            var items = rewindDataIndexCache.GetMemberValue("items") as ConcurrentDictionary<string, int>;

            items.Should().HaveCount(22);
            this.CheckCache(items, 19, 9);
        }


        private void CheckCache(ConcurrentDictionary<string, int> items, int tip, int bottom)
        {
            foreach (KeyValuePair<string, int> keyValuePair in items)
            {
                Assert.True(keyValuePair.Value <= tip && keyValuePair.Value >= bottom);
            }
        }

        private void SetupMockCoinView(Mock<ICoinView> coinViewMock)
        {
            // set up coinview with 2 blocks and 2 utxo per block.
            ulong index = 1;
            coinViewMock.Setup(c => c.GetRewindData(It.IsAny<int>())).Returns(() => Task.FromResult(new RewindData()
            {
                OutputsToRestore = new List<UnspentOutputs>() { new UnspentOutputs(new uint256(index++), new Coins()) { Outputs = new TxOut[] { new TxOut(), new TxOut() } } }
            }));
        }
    }
}