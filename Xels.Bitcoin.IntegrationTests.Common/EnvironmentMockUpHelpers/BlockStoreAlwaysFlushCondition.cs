using Xels.Bitcoin.Interfaces;

namespace Xels.Bitcoin.IntegrationTests.Common.EnvironmentMockUpHelpers
{
    public class BlockStoreAlwaysFlushCondition : IBlockStoreQueueFlushCondition
    {
        public bool ShouldFlush => true;
    }
}
