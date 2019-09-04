using Xels.Bitcoin.Connection;
using Xels.Bitcoin.P2P;

namespace Xels.Bitcoin.IntegrationTests.Common.EnvironmentMockUpHelpers
{
    /// <summary>
    /// To be used with all the test runners to ensure that peer discovery does not run.
    /// </summary>
    public sealed class PeerDiscoveryDisabled : IPeerDiscovery
    {
        public void DiscoverPeers(IConnectionManager connectionManager)
        {
        }

        public void Dispose()
        {
        }
    }
}
