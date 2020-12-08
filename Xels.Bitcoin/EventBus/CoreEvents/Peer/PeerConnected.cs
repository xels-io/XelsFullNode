using System.Net;

namespace Xels.Bitcoin.EventBus.CoreEvents
{
    /// <summary>
    /// Event that is published whenever a peer connects to the node.
    /// This happens prior to any Payload they have to exchange.
    /// </summary>
    /// <seealso cref="Xels.Bitcoin.EventBus.EventBase" />
    public class PeerConnected : PeerEventBase
    {
        public bool Inbound { get; }

        public PeerConnected(bool inbound, IPEndPoint peerEndPoint) : base(peerEndPoint)
        {
            this.Inbound = inbound;
        }
    }
}