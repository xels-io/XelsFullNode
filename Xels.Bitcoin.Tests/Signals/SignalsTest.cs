using Microsoft.Extensions.Logging;
using NBitcoin;
using Xels.Bitcoin.EventBus;
using Xels.Bitcoin.EventBus.CoreEvents;
using Xels.Bitcoin.Primitives;
using Xels.Bitcoin.Tests.Common;
using Xunit;

namespace Xels.Bitcoin.Tests.Signals
{
    public class SignalsTest
    {
        private readonly Bitcoin.Signals.ISignals signals;

        public SignalsTest()
        {
            this.signals = new Bitcoin.Signals.Signals(new LoggerFactory(), null);
        }

        [Fact]
        public void SignalBlockBroadcastsToBlockSignaler()
        {
            Block block = KnownNetworks.XelsMain.CreateBlock();
            ChainedHeader header = ChainedHeadersHelper.CreateGenesisChainedHeader();
            var chainedHeaderBlock = new ChainedHeaderBlock(block, header);

            bool signaled = false;
            using (SubscriptionToken sub = this.signals.Subscribe<BlockConnected>(headerBlock => signaled = true))
            {
                this.signals.Publish(new BlockConnected(chainedHeaderBlock));
            }

            Assert.True(signaled);
        }

        [Fact]
        public void SignalBlockDisconnectedToBlockSignaler()
        {
            Block block = KnownNetworks.XelsMain.CreateBlock();
            ChainedHeader header = ChainedHeadersHelper.CreateGenesisChainedHeader();
            var chainedHeaderBlock = new ChainedHeaderBlock(block, header);

            bool signaled = false;
            using (SubscriptionToken sub = this.signals.Subscribe<BlockDisconnected>(headerBlock => signaled = true))
            {
                this.signals.Publish(new BlockDisconnected(chainedHeaderBlock));
            }

            Assert.True(signaled);
        }

        [Fact]
        public void SignalTransactionBroadcastsToTransactionSignaler()
        {
            Transaction transaction = KnownNetworks.XelsMain.CreateTransaction();

            bool signaled = false;
            using (SubscriptionToken sub = this.signals.Subscribe<TransactionReceived>(transaction1 => signaled = true))
            {
                this.signals.Publish(new TransactionReceived(transaction));
            }

            Assert.True(signaled);
        }

        [Fact]
        public void SignalUnsubscribingPreventTriggeringPreviouslySubscribedAction()
        {
            Transaction transaction = KnownNetworks.XelsMain.CreateTransaction();

            bool signaled = false;
            using (SubscriptionToken sub = this.signals.Subscribe<TransactionReceived>(transaction1 => signaled = true))
            {
                this.signals.Publish(new TransactionReceived(transaction));
            }

            Assert.True(signaled);

            signaled = false; // reset the flag
            this.signals.Publish(new TransactionReceived(transaction));
            //expect signaled be false
            Assert.True(!signaled);

        }

        [Fact]
        public void SignalSubscrerThrowExceptionDefaultSubscriptionErrorHandlerRethrow()
        {
            try
            {
                using (SubscriptionToken sub = this.signals.Subscribe<TestEvent>(transaction1 => throw new System.Exception("TestingException")))
                {
                    this.signals.Publish(new TestEvent());
                }
            }
            catch (System.Exception ex)
            {
                Assert.True(ex.Message == "TestingException");
            }
        }

        class TestEvent : EventBase
        {
            public TestEvent() { }
        }
    }
}