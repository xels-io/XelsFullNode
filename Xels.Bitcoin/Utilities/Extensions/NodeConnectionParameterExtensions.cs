using System.Linq;
using Xels.Bitcoin.P2P;
using Xels.Bitcoin.P2P.Peer;

namespace Xels.Bitcoin.Utilities.Extensions
{
    public static class NodeConnectionParameterExtensions
    {
        public static PeerAddressManagerBehaviour PeerAddressManagerBehaviour(this NetworkPeerConnectionParameters parameters)
        {
            return parameters.TemplateBehaviors.OfType<PeerAddressManagerBehaviour>().FirstOrDefault();
        }
    }
}