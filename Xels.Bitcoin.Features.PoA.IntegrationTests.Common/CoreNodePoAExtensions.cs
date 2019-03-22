using Xels.Bitcoin.IntegrationTests.Common.EnvironmentMockUpHelpers;

namespace Xels.Bitcoin.Features.PoA.IntegrationTests.Common
{
    public static class CoreNodePoAExtensions
    {
        public static void EnableFastMining(this CoreNode node)
        {
            (node.FullNode.NodeService<IPoAMiner>() as TestPoAMiner).EnableFastMining();
        }
    }
}
