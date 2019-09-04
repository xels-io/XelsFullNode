using System.Linq;
using NBitcoin;
using Xels.Bitcoin.Consensus;
using Xels.Bitcoin.Features.Consensus.Rules.CommonRules;
using Xels.Bitcoin.IntegrationTests.Common.EnvironmentMockUpHelpers;

namespace Xels.Bitcoin.IntegrationTests
{
    public static class CoreNodeExtensions
    {
        public static Money GetProofOfWorkRewardForMinedBlocks(this CoreNode node, int numberOfBlocks)
        {
            var coinviewRule = node.FullNode.NodeService<IConsensusRuleEngine>().GetRule<CoinViewRule>();

            int startBlock = node.FullNode.ChainIndexer.Height - numberOfBlocks + 1;

            return Enumerable.Range(startBlock, numberOfBlocks)
                .Sum(p => coinviewRule.GetProofOfWorkReward(p));
        }       
    }
}