using System;
using System.Threading;
using NBitcoin;
using Xels.Bitcoin.Base.Deployments;
using Xels.Bitcoin.Consensus;
using Xels.Bitcoin.Features.RPC;
using Xels.Bitcoin.IntegrationTests.Common.EnvironmentMockUpHelpers;
using Xels.Bitcoin.Networks.Deployments;
using Xels.Bitcoin.Tests.Common;
using Xels.Bitcoin.Utilities.Extensions;
using Xunit;

namespace Xels.Bitcoin.IntegrationTests
{
    public class SegWitTests
    {
        [Fact]
        public void TestSegwit_MinedOnCore_ActivatedOn_XelsNode()
        {
            using (NodeBuilder builder = NodeBuilder.Create(this))
            {
                CoreNode coreNode = builder.CreateBitcoinCoreNode(version: "0.15.1");

                coreNode.ConfigParameters.AddOrReplace("debug", "1");
                coreNode.ConfigParameters.AddOrReplace("printtoconsole", "0");
                coreNode.Start();

                CoreNode xelsNode = builder.CreateXelsPowNode(KnownNetworks.RegTest).Start();

                RPCClient xelsNodeRpc = xelsNode.CreateRPCClient();
                RPCClient coreRpc = coreNode.CreateRPCClient();

                coreRpc.AddNode(xelsNode.Endpoint, false);
                xelsNodeRpc.AddNode(coreNode.Endpoint, false);

                // core (in version 0.15.1) only mines segwit blocks above a certain height on regtest
                // future versions of core will change that behaviour so this test may need to be changed in the future
                // see issue for more details https://github.com/xelsproject/XelsBitcoinFullNode/issues/1028
                BIP9DeploymentsParameters prevSegwitDeployment = KnownNetworks.RegTest.Consensus.BIP9Deployments[BitcoinBIP9Deployments.Segwit];
                KnownNetworks.RegTest.Consensus.BIP9Deployments[BitcoinBIP9Deployments.Segwit] = new BIP9DeploymentsParameters(1, 0, DateTime.Now.AddDays(50).ToUnixTimestamp());

                try
                {
                    // generate 450 blocks, block 431 will be segwit activated.
                    coreRpc.Generate(450);
                    var cancellationToken = new CancellationTokenSource(TimeSpan.FromMinutes(2)).Token;
                    TestBase.WaitLoop(() => xelsNode.CreateRPCClient().GetBestBlockHash() == coreNode.CreateRPCClient().GetBestBlockHash(), cancellationToken: cancellationToken);

                    // segwit activation on Bitcoin regtest.
                    // - On regtest deployment state changes every 144 block, the threshold for activating a rule is 108 blocks.
                    // segwit deployment status should be:
                    // - Defined up to block 142.
                    // - Started at block 143 to block 286 .
                    // - LockedIn 287 (as segwit should already be signaled in blocks).
                    // - Active at block 431.

                    var consensusLoop = xelsNode.FullNode.NodeService<IConsensusRuleEngine>() as ConsensusRuleEngine;
                    ThresholdState[] segwitDefinedState = consensusLoop.NodeDeployments.BIP9.GetStates(xelsNode.FullNode.ChainIndexer.GetHeader(142));
                    ThresholdState[] segwitStartedState = consensusLoop.NodeDeployments.BIP9.GetStates(xelsNode.FullNode.ChainIndexer.GetHeader(143));
                    ThresholdState[] segwitLockedInState = consensusLoop.NodeDeployments.BIP9.GetStates(xelsNode.FullNode.ChainIndexer.GetHeader(287));
                    ThresholdState[] segwitActiveState = consensusLoop.NodeDeployments.BIP9.GetStates(xelsNode.FullNode.ChainIndexer.GetHeader(431));

                    // check that segwit is got activated at block 431
                    Assert.Equal(ThresholdState.Defined, segwitDefinedState.GetValue((int)BitcoinBIP9Deployments.Segwit));
                    Assert.Equal(ThresholdState.Started, segwitStartedState.GetValue((int)BitcoinBIP9Deployments.Segwit));
                    Assert.Equal(ThresholdState.LockedIn, segwitLockedInState.GetValue((int)BitcoinBIP9Deployments.Segwit));
                    Assert.Equal(ThresholdState.Active, segwitActiveState.GetValue((int)BitcoinBIP9Deployments.Segwit));
                }
                finally
                {
                    KnownNetworks.RegTest.Consensus.BIP9Deployments[BitcoinBIP9Deployments.Segwit] = prevSegwitDeployment;
                }
            }
        }

        private void TestSegwit_MinedOnXelsNode_ActivatedOn_CoreNode()
        {
            // TODO: mine segwit onh a xels node on the bitcoin network
            // write a tests that mines segwit blocks on the xels node
            // and signals them to a core not, then segwit will get activated on core
        }
    }
}
