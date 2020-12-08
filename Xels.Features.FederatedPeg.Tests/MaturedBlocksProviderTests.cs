﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NBitcoin;
using NSubstitute;
using NSubstitute.Core;
using Xels.Bitcoin;
using Xels.Bitcoin.Consensus;
using Xels.Bitcoin.Primitives;
using Xels.Bitcoin.Tests.Common;
using Xels.Features.FederatedPeg.Interfaces;
using Xels.Features.FederatedPeg.Models;
using Xels.Features.FederatedPeg.SourceChain;
using Xunit;

namespace Xels.Features.FederatedPeg.Tests
{
    public class MaturedBlocksProviderTests
    {
        private readonly IDepositExtractor depositExtractor;

        private readonly ILoggerFactory loggerFactory;

        private readonly ILogger logger;

        private readonly IConsensusManager consensusManager;

        public MaturedBlocksProviderTests()
        {
            this.loggerFactory = Substitute.For<ILoggerFactory>();
            this.logger = Substitute.For<ILogger>();
            this.loggerFactory.CreateLogger(null).ReturnsForAnyArgs(this.logger);
            this.depositExtractor = Substitute.For<IDepositExtractor>();
            this.consensusManager = Substitute.For<IConsensusManager>();
        }

        [Fact]
        public void GetMaturedBlocksReturnsDeposits()
        {
            List<ChainedHeader> headers = ChainedHeadersHelper.CreateConsecutiveHeaders(10, null, true);

            foreach (ChainedHeader chainedHeader in headers)
            {
                chainedHeader.Block = new Block(chainedHeader.Header);
            }

            var blocks = new List<ChainedHeaderBlock>(headers.Count);

            foreach (ChainedHeader chainedHeader in headers)
            {
                blocks.Add(new ChainedHeaderBlock(chainedHeader.Block, chainedHeader));
            }

            ChainedHeader tip = headers.Last();

            this.consensusManager.GetBlockData(Arg.Any<List<uint256>>()).Returns(delegate (CallInfo info)
            {
                List<uint256> hashes = (List<uint256>)info[0];
                return hashes.Select((hash) => blocks.Single(x => x.ChainedHeader.HashBlock == hash)).ToArray();
            });

            uint zero = 0;
            this.depositExtractor.MinimumDepositConfirmations.Returns(info => zero);
            this.depositExtractor.ExtractBlockDeposits(null).ReturnsForAnyArgs(new MaturedBlockDepositsModel(new MaturedBlockInfoModel(), new List<IDeposit>()));
            this.consensusManager.Tip.Returns(tip);

            // Makes every block a matured block.
            var maturedBlocksProvider = new MaturedBlocksProvider(this.consensusManager, this.depositExtractor, this.loggerFactory);

            SerializableResult<List<MaturedBlockDepositsModel>> depositsResult = maturedBlocksProvider.GetMaturedDeposits(0, 10);

            // Expect the number of matured deposits to equal the number of blocks.
            Assert.Equal(10, depositsResult.Value.Count);
        }
    }
}
