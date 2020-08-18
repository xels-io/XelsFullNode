using System.Linq;
using NBitcoin;
using Xels.Bitcoin.Features.PoA;
using Xels.Bitcoin.Features.PoA.IntegrationTests.Common;
using Xels.Bitcoin.Features.Wallet.Interfaces;
using Xels.Bitcoin.IntegrationTests.Common.EnvironmentMockUpHelpers;
using Xels.Bitcoin.Tests.Common;
using Xels.Features.FederatedPeg.IntegrationTests.Utils;
using Xels.Sidechains.Networks;
using Xunit;

namespace Xels.Features.FederatedPeg.IntegrationTests
{
    public class MiningTests
    {
        [Fact(Skip = FederatedPegTestHelper.SkipTests)]
        public void NodeCanLoadFederationKey()
        {
            var network = (XoyRegTest)XoyNetwork.NetworksSelector.Regtest();

            using (PoANodeBuilder builder = PoANodeBuilder.CreatePoANodeBuilder(this))
            {
                // Create first node as fed member.
                Key key = network.FederationKeys[0];
                CoreNode node = builder.CreatePoANode(network, key).Start();

                Assert.True(node.FullNode.NodeService<IFederationManager>().IsFederationMember);
                Assert.Equal(node.FullNode.NodeService<IFederationManager>().CurrentFederationKey, key);
                // Assert.True(node.FullNode.NodeService<IPoAMiner>().IsMining()); Old method

                // Create second node as normal node.
                CoreNode node2 = builder.CreatePoANode(network).Start();

                Assert.False(node2.FullNode.NodeService<IFederationManager>().IsFederationMember);
                Assert.Equal(node2.FullNode.NodeService<IFederationManager>().CurrentFederationKey, null);
                // Assert.False(node2.FullNode.NodeService<IPoAMiner>().IsMining()); Old method
            }
        }

        [Fact(Skip = FederatedPegTestHelper.SkipTests)]
        public void NodeCanMine()
        {
            var network = (XoyRegTest)XoyNetwork.NetworksSelector.Regtest();

            using (PoANodeBuilder builder = PoANodeBuilder.CreatePoANodeBuilder(this))
            {
                CoreNode node0 = builder.CreatePoANode(network, network.FederationKeys[0]).Start();
                CoreNode node1 = builder.CreatePoANode(network, network.FederationKeys[1]).Start();
                // node0.EnableFastMining(); Old method
                // node1.EnableFastMining(); Old method

                int tipBefore = node0.GetTip().Height;
                TestBase.WaitLoop(
                    () =>
                        {
                            return node0.GetTip().Height >= tipBefore + 5;
                        }
                    );
            }
        }

        [Fact(Skip = FederatedPegTestHelper.SkipTests)]
        public void PremineIsReceived()
        {
            var network = (XoyRegTest)XoyNetwork.NetworksSelector.Regtest();

            using (PoANodeBuilder builder = PoANodeBuilder.CreatePoANodeBuilder(this))
            {
                string walletName = "mywallet";
                CoreNode node = builder.CreatePoANode(network, network.FederationKeys[0]).WithWallet("pass", walletName).Start();
                // node.EnableFastMining(); Old method

                var walletManager = node.FullNode.NodeService<IWalletManager>();
                long balanceOnStart = walletManager.GetBalances(walletName, "account 0").Sum(x => x.AmountConfirmed);
                Assert.Equal(0, balanceOnStart);

                TestBase.WaitLoop(() => node.GetTip().Height >= network.Consensus.PremineHeight + network.Consensus.CoinbaseMaturity + 1);

                long balanceAfterPremine = walletManager.GetBalances(walletName, "account 0").Sum(x => x.AmountConfirmed);

                Assert.Equal(network.Consensus.PremineReward.Satoshi, balanceAfterPremine);
            }
        }
    }
}
