using Xels.Bitcoin.Connection;
using Xels.Bitcoin.P2P.Peer;

namespace Xels.Bitcoin.Utilities.Extensions
{
    public static class PeerExtensions
    {
        public static bool IsWhitelisted(this INetworkPeer peer)
        {
            return peer.Behavior<IConnectionManagerBehavior>()?.Whitelisted == true;
        }
    }
}
