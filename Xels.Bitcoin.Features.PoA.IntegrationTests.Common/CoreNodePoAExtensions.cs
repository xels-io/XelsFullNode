using System.Threading.Tasks;
using Xels.Bitcoin.IntegrationTests.Common;
using Xels.Bitcoin.IntegrationTests.Common.EnvironmentMockUpHelpers;
using Xels.Bitcoin.Tests.Common;

namespace Xels.Bitcoin.Features.PoA.IntegrationTests.Common
{
    public static class CoreNodePoAExtensions
    {
        public static async Task MineBlocksAsync(this CoreNode node, int count)
        {
            await (node.FullNode.NodeService<IPoAMiner>() as TestPoAMiner).MineBlocksAsync(count);
        }

        public static void WaitTillSynced(params CoreNode[] nodes)
        {
            for (int i = 0; i < nodes.Length - 1; i++)
            {
                TestBase.WaitLoop(() => TestHelper.AreNodesSynced(nodes[i], nodes[i + 1]));
            }
        }
    }
}
