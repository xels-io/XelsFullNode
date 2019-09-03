using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using NBitcoin;
using Xels.Bitcoin.EventBus.CoreEvents;
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

            Signals.Signals signals = new Signals.Signals(loggerFactoryMock.Object, null);
            var subject = new BlocksDisconnectedSignaled(mempoolValidatorMock.Object, new MempoolSchedulerLock(), loggerFactoryMock.Object, signals);
            subject.Initialize();

            var block = new Block();
            var genesisChainedHeaderBlock = new ChainedHeaderBlock(block, ChainedHeadersHelper.CreateGenesisChainedHeader());
            var transaction1 = new Transaction();
            var transaction2 = new Transaction();
            block.Transactions = new List<Transaction> { transaction1, transaction2 };

            signals.Publish(new BlockDisconnected(genesisChainedHeaderBlock));

            mempoolValidatorMock.Verify(x => x.AcceptToMemoryPool(It.IsAny<MempoolValidationState>(), transaction1));
            mempoolValidatorMock.Verify(x => x.AcceptToMemoryPool(It.IsAny<MempoolValidationState>(), transaction2));
        }
    }
}
