using FluentAssertions;
using NBitcoin;
using Xels.Bitcoin.Consensus.Rules;
using Xels.Bitcoin.Features.Consensus.Rules.CommonRules;
using Xunit;

namespace Xels.Bitcoin.Features.Consensus.Tests.Rules.CommonRules
{
    public class BlockMerkleRootRuleTests
    {
        [Fact]
        public void BlockMerkleRootRule_Cannot_Be_Skipped()
        {
            // TODO: Create fake blocks.
        }
    }
}
