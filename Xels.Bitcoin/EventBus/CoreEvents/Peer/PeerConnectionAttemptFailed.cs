using System.Net;

namespace Xels.Bitcoin.EventBus.CoreEvents
{
    /// <summary>
    /// Event that is published whenever a peer connection attempt failed.
    /// </summary>
    /// <seealso cref="Xels.Bitcoin.EventBus.EventBase" />
    public class PeerConnectionAttemptFailed : PeerEventBase
    {
        public bool Inbound { get; }

        public string Reason { get; }

        public PeerConnectionAttemptFailed(bool inbound, IPEndPoint peerEndPoint, string reason) : base(peerEndPoint)
        {
            this.Inbound = inbound;
            this.Reason = reason;
        }
    }
}