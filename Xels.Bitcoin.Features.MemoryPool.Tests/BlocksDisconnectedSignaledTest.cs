﻿using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using NBitcoin;
using Xels.Bitcoin.Primitives;
using Xels.Bitcoin.Tests.Common;
using Xunit;

namespace Xels.Bitcoin.Features.MemoryPool.Tests
{
    public class BlocksDisconnectedSignaledTest
    {
        [Fact]
        public void OnNextCore_WhenTransactionsMissingInLongestChain_ReturnsThemToTheMempool()
        {
            var mempoolValidatorMock = new Mock<IMempoolValidator>();
            var loggerFactoryMock = new Mock<ILoggerFactory>();
            loggerFactoryMock.Setup(i => i.CreateLogger(It.IsAny<string>())).Returns(new Mock<ILogger>().Object);

            var subject = new BlocksDisconnectedSignaled(mempoolValidatorMock.Object, new MempoolSchedulerLock(), loggerFactoryMock.Object);

            var block = new Block();
            var genesisChainedHeaderBlock = new ChainedHeaderBlock(block, ChainedHeadersHelper.CreateGenesisChainedHeader());
            var transaction1 = new Transaction();
            var transaction2 = new Transaction();
            block.Transactions = new List<Transaction> { transaction1, transaction2 };

            subject.OnNext(genesisChainedHeaderBlock);

            mempoolValidatorMock.Verify(x => x.AcceptToMemoryPool(It.IsAny<MempoolValidationState>(), transaction1));
            mempoolValidatorMock.Verify(x => x.AcceptToMemoryPool(It.IsAny<MempoolValidationState>(), transaction2));
        }
    }
}
