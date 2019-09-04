using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xels.Bitcoin.P2P.Protocol.Payloads;
using Xels.Features.FederatedPeg.Payloads;

namespace Xels.Features.FederatedPeg.Interfaces
{
    /// <summary>
    /// Broadcasts a payload to all federated peg nodes, from -federationips.
    /// </summary>
    public interface IFederatedPegBroadcaster
    {
        /// <summary>
        /// Broadcast the given payload to the known federated peg nodes.
        /// </summary>
        Task BroadcastAsync(Payload payload);
    }
}
