using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NBitcoin;
using Xels.Bitcoin.AsyncWork;
using Xels.Bitcoin.Connection;
using Xels.Bitcoin.Features.BlockStore;
using Xels.Bitcoin.IntegrationTests.Common;
using Xels.Bitcoin.IntegrationTests.Common.EnvironmentMockUpHelpers;
using Xels.Bitcoin.IntegrationTests.Common.ReadyData;
using Xels.Bitcoin.Networks;
using Xels.Bitcoin.P2P.Peer;
using Xels.Bitcoin.P2P.Protocol;
using Xels.Bitcoin.P2P.Protocol.Behaviors;
using Xels.Bitcoin.P2P.Protocol.Payloads;
using Xels.Bitcoin.Tests.Common;
using Xels.Bitcoin.Utilities;
using Xunit;

namespace Xels.Bitcoin.IntegrationTests.BlockStore
{
    /// <summary>
    /// Used for recording messages coming into a test node. Does not respond to them in any way.
    /// </summary>
    internal class TestBehavior : NetworkPeerBehavior
    {
        public Dictionary<string, List<IncomingMessage>> receivedMessageTracker = new Dictionary<string, List<IncomingMessage>>();

        protected override void AttachCore()
        {
            this.AttachedPeer.MessageReceived.Register(this.OnMessageReceivedAsync);
        }

        protected override void DetachCore()
        {
            this.AttachedPeer.MessageReceived.Unregister(this.OnMessageReceivedAsync);
        }

        private async Task OnMessageReceivedAsync(INetworkPeer peer, IncomingMessage message)
        {
            try
            {
                await this.ProcessMessageAsync(peer, message).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private async Task ProcessMessageAsync(INetworkPeer peer, IncomingMessage message)
        {
            if (!this.receivedMessageTracker.ContainsKey(message.Message.Payload.Command))
                this.receivedMessageTracker[message.Message.Payload.Command] = new List<IncomingMessage>();

            this.receivedMessageTracker[message.Message.Payload.Command].Add(message);
        }

        public override object Clone()
        {
            var res = new TestBehavior();

            return res;
        }
    }

    public class BlockStoreSignaledTests
    {
        protected readonly ILoggerFactory loggerFactory;
        private readonly Network network;

        public BlockStoreSignaledTests()
        {
            this.loggerFactory = new LoggerFactory();

            this.network = new BitcoinRegTest();
        }

        [Fact]
        public void CheckBlocksAnnounced_AndQueueEmptiesOverTime()
        {
            using (NodeBuilder builder = NodeBuilder.Create(this))
            {
                CoreNode xelsNodeSync = builder.CreateXelsPowNode(this.network, "bss-1-xelsNodeSync").WithReadyBlockchainData(ReadyBlockchain.BitcoinRegTest10Miner).Start();
                CoreNode xelsNode1 = builder.CreateXelsPowNode(this.network, "bss-1-xelsNode1").Start();

                // Change the second node's list of default behaviours include the test behaviour in it.
                // We leave the other behaviors alone for this test because we want to see what messages the node gets under normal operation.
                IConnectionManager node1ConnectionManager = xelsNode1.FullNode.NodeService<IConnectionManager>();
                node1ConnectionManager.Parameters.TemplateBehaviors.Add(new TestBehavior());

                // Connect node1 to initial node.
                TestHelper.Connect(xelsNode1, xelsNodeSync);

                INetworkPeer connectedPeer = node1ConnectionManager.ConnectedPeers.FindByEndpoint(xelsNodeSync.Endpoint);
                TestBehavior testBehavior = connectedPeer.Behavior<TestBehavior>();

                TestBase.WaitLoop(() => TestHelper.AreNodesSynced(xelsNode1, xelsNodeSync));

                HashSet<uint256> advertised = new HashSet<uint256>();

                // Check to see that all blocks got advertised to node1 via the "headers" payload.
                foreach (IncomingMessage message in testBehavior.receivedMessageTracker["headers"])
                {
                    if (message.Message.Payload is HeadersPayload)
                        foreach (BlockHeader header in ((HeadersPayload)message.Message.Payload).Headers)
                            advertised.Add(header.GetHash());
                }

                foreach (ChainedHeader chainedHeader in xelsNodeSync.FullNode.ChainIndexer.EnumerateToTip(this.network.GenesisHash))
                    if ((!advertised.Contains(chainedHeader.HashBlock)) && (!(chainedHeader.HashBlock == this.network.GenesisHash)))
                        throw new Exception($"An expected block was not advertised to peer: {chainedHeader.HashBlock}");

                // Check current state of announce queue
                BlockStoreSignaled blockStoreSignaled = xelsNodeSync.FullNode.NodeService<BlockStoreSignaled>();

                IAsyncQueue<ChainedHeader> blocksToAnnounce = (IAsyncQueue<ChainedHeader>)blockStoreSignaled.GetMemberValue("blocksToAnnounce");
                Queue<ChainedHeader> queueItems = (Queue<ChainedHeader>)blocksToAnnounce.GetMemberValue("items");

                TestBase.WaitLoop(() => queueItems.Count == 0);
            }
        }

        [Fact]
        public void CheckBlocksAnnounced_AndQueueEmptiesOverTime_ForMultiplePeers_WhenOneIsDisconnected()
        {
            using (NodeBuilder builder = NodeBuilder.Create(this))
            {
                CoreNode xelsNodeSync = builder.CreateXelsPowNode(this.network, "bss-2-xelsNodeSync").WithReadyBlockchainData(ReadyBlockchain.BitcoinRegTest10Miner).Start();

                CoreNode xelsNode1 = builder.CreateXelsPowNode(this.network, "bss-2-xelsNode1").Start();
                CoreNode xelsNode2 = builder.CreateXelsPowNode(this.network, "bss-2-xelsNode2").Start();
                CoreNode xelsNode3 = builder.CreateXelsPowNode(this.network, "bss-2-xelsNode3").Start();

                // Change the other nodes' lists of default behaviours include the test behaviour in it.
                // We leave the other behaviors alone for this test because we want to see what messages the node gets under normal operation.
                IConnectionManager node1ConnectionManager = xelsNode1.FullNode.NodeService<IConnectionManager>();
                node1ConnectionManager.Parameters.TemplateBehaviors.Add(new TestBehavior());

                IConnectionManager node2ConnectionManager = xelsNode2.FullNode.NodeService<IConnectionManager>();
                node2ConnectionManager.Parameters.TemplateBehaviors.Add(new TestBehavior());

                // Connect other nodes to initial node.
                TestHelper.Connect(xelsNode1, xelsNodeSync);
                TestHelper.Connect(xelsNode2, xelsNodeSync);
                TestHelper.Connect(xelsNode3, xelsNodeSync);

                // Make node3 unable to respond to anything, effectively disconnecting it.
                IConnectionManager node3ConnectionManager = xelsNode3.FullNode.NodeService<IConnectionManager>();
                node3ConnectionManager.Parameters.TemplateBehaviors.Clear();
                node3ConnectionManager.Parameters.TemplateBehaviors.Add(new TestBehavior());

                INetworkPeer connectedPeer1 = node1ConnectionManager.ConnectedPeers.FindByEndpoint(xelsNodeSync.Endpoint);
                TestBehavior testBehavior1 = connectedPeer1.Behavior<TestBehavior>();

                INetworkPeer connectedPeer2 = node2ConnectionManager.ConnectedPeers.FindByEndpoint(xelsNodeSync.Endpoint);
                TestBehavior testBehavior2 = connectedPeer2.Behavior<TestBehavior>();

                INetworkPeer connectedPeer3 = node3ConnectionManager.ConnectedPeers.FindByEndpoint(xelsNodeSync.Endpoint);
                TestBehavior testBehavior3 = connectedPeer3.Behavior<TestBehavior>();

                // If the announce queue is not getting stalled, the other 2 nodes should sync properly.
                TestBase.WaitLoop(() => TestHelper.AreNodesSynced(xelsNode1, xelsNodeSync));
                TestBase.WaitLoop(() => TestHelper.AreNodesSynced(xelsNode2, xelsNodeSync));

                HashSet<uint256> advertised = new HashSet<uint256>();

                // Check to see that all blocks got advertised to node1 via the "headers" payload.
                foreach (IncomingMessage message in testBehavior1.receivedMessageTracker["headers"])
                {
                    if (message.Message.Payload is HeadersPayload)
                        foreach (BlockHeader header in ((HeadersPayload)message.Message.Payload).Headers)
                            advertised.Add(header.GetHash());
                }

                foreach (ChainedHeader chainedHeader in xelsNodeSync.FullNode.ChainIndexer.EnumerateToTip(this.network.GenesisHash))
                    if ((!advertised.Contains(chainedHeader.HashBlock)) && (!(chainedHeader.HashBlock == this.network.GenesisHash)))
                        throw new Exception($"An expected block was not advertised to peer 1: {chainedHeader.HashBlock}");

                advertised.Clear();

                // Check to see that all blocks got advertised to node1 via the "headers" payload.
                foreach (IncomingMessage message in testBehavior2.receivedMessageTracker["headers"])
                {
                    if (message.Message.Payload is HeadersPayload)
                        foreach (BlockHeader header in ((HeadersPayload)message.Message.Payload).Headers)
                            advertised.Add(header.GetHash());
                }

                foreach (ChainedHeader chainedHeader in xelsNodeSync.FullNode.ChainIndexer.EnumerateToTip(this.network.GenesisHash))
                    if ((!advertised.Contains(chainedHeader.HashBlock)) && (!(chainedHeader.HashBlock == this.network.GenesisHash)))
                        throw new Exception($"An expected block was not advertised to peer 2: {chainedHeader.HashBlock}");

                // Check current state of announce queue.
                BlockStoreSignaled blockStoreSignaled = xelsNodeSync.FullNode.NodeService<BlockStoreSignaled>();

                IAsyncQueue<ChainedHeader> blocksToAnnounce = (IAsyncQueue<ChainedHeader>)blockStoreSignaled.GetMemberValue("blocksToAnnounce");
                Queue<ChainedHeader> queueItems = (Queue<ChainedHeader>)blocksToAnnounce.GetMemberValue("items");

                // It should still eventually empty despite not being able to communicate with node3.
                TestBase.WaitLoop(() => queueItems.Count == 0);
            }
        }

        [Fact]
        public void QueueEmpties_WithNoPeersConnected()
        {
            using (NodeBuilder builder = NodeBuilder.Create(this))
            {
                CoreNode xelsNodeSync = builder.CreateXelsPowNode(this.network, "bss-3-xelsNodeSync").WithReadyBlockchainData(ReadyBlockchain.BitcoinRegTest10Miner).Start();
                BlockStoreSignaled blockStoreSignaled = xelsNodeSync.FullNode.NodeService<BlockStoreSignaled>();

                AsyncQueue<ChainedHeader> blocksToAnnounce = (AsyncQueue<ChainedHeader>)blockStoreSignaled.GetMemberValue("blocksToAnnounce");

                Queue<ChainedHeader> queueItems = (Queue<ChainedHeader>)blocksToAnnounce.GetMemberValue("items");

                // Announce queue length should drop to zero once the announce batch timer elapses at the latest.
                // Most likely it will be cleared almost instantly as the blocks getting mined are all tips.
                TestBase.WaitLoop(() => queueItems.Count == 0);
            }
        }

        [Fact]
        public void MustNotAnnounceABlock_WhenNotInBestChain()
        {
            using (NodeBuilder builder = NodeBuilder.Create(this))
            {
                CoreNode xelsNodeSync = builder.CreateXelsPowNode(this.network, "bss-2-xelsNodeSync").WithReadyBlockchainData(ReadyBlockchain.BitcoinRegTest10Miner).Start();
                CoreNode xelsNode1 = builder.CreateXelsPowNode(this.network, "bss-2-xelsNode1").WithReadyBlockchainData(ReadyBlockchain.BitcoinRegTest10Listener).Start();
                CoreNode xelsNode2 = builder.CreateXelsPowNode(this.network, "bss-2-xelsNode2").WithReadyBlockchainData(ReadyBlockchain.BitcoinRegTest10NoWallet).Start();

                // Store block 1 of chain0 for later usage
                ChainedHeader firstBlock = null;
                foreach (ChainedHeader chainedHeader in xelsNodeSync.FullNode.ChainIndexer.EnumerateToTip(this.network.GenesisHash))
                {
                    if (chainedHeader.Height == 1)
                    {
                        firstBlock = chainedHeader;
                    }
                }

                Assert.NotNull(firstBlock);

                // Mine longer chain1 using node1
                TestHelper.MineBlocks(xelsNode1, 15);

                IConnectionManager node1ConnectionManager = xelsNode1.FullNode.NodeService<IConnectionManager>();
                node1ConnectionManager.Parameters.TemplateBehaviors.Add(new TestBehavior());

                IConnectionManager node2ConnectionManager = xelsNode2.FullNode.NodeService<IConnectionManager>();
                node2ConnectionManager.Parameters.TemplateBehaviors.Add(new TestBehavior());

                // Connect node0 and node1
                TestHelper.Connect(xelsNode1, xelsNodeSync);

                INetworkPeer connectedPeer = node1ConnectionManager.ConnectedPeers.FindByEndpoint(xelsNodeSync.Endpoint);
                TestBehavior testBehavior = connectedPeer.Behavior<TestBehavior>();

                // We expect that node0 will abandon the 10 block chain and use the 15 block chain from node1
                TestBase.WaitLoop(() => TestHelper.AreNodesSynced(xelsNode1, xelsNodeSync));

                // Connect all nodes together
                TestHelper.Connect(xelsNode2, xelsNodeSync);
                TestHelper.Connect(xelsNode1, xelsNode2);

                INetworkPeer connectedPeer2 = node2ConnectionManager.ConnectedPeers.FindByEndpoint(xelsNodeSync.Endpoint);
                TestBehavior testBehavior2 = connectedPeer2.Behavior<TestBehavior>();

                // Wait for node2 to sync; it should have the 15 block chain
                TestBase.WaitLoop(() => TestHelper.AreNodesSynced(xelsNode2, xelsNodeSync));

                // Insert block 1 from chain0 into node1's announce queue
                BlockStoreSignaled node1BlockStoreSignaled = xelsNode1.FullNode.NodeService<BlockStoreSignaled>();

                IAsyncQueue<ChainedHeader> node1BlocksToAnnounce = (IAsyncQueue<ChainedHeader>)node1BlockStoreSignaled.GetMemberValue("blocksToAnnounce");

                Queue<ChainedHeader> node1QueueItems = (Queue<ChainedHeader>)node1BlocksToAnnounce.GetMemberValue("items");

                TestBase.WaitLoop(() => node1QueueItems.Count == 0);

                // Check that node2 does not have block 1 in test behaviour advertised list
                foreach (IncomingMessage message in testBehavior2.receivedMessageTracker["headers"])
                {
                    if (message.Message.Payload is HeadersPayload)
                    {
                        foreach (BlockHeader header in ((HeadersPayload)message.Message.Payload).Headers)
                        {
                            if (header.GetHash() == firstBlock.Header.GetHash())
                            {
                                throw new Exception("Should not have received payload announcing block from wrong chain");
                            }
                        }
                    }
                }
            }
        }
    }
}
