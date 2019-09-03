﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xels.Bitcoin.Connection;
using Xels.Bitcoin.P2P.Peer;
using Xels.Bitcoin.P2P.Protocol.Payloads;
using Xels.Features.FederatedPeg.Interfaces;
using Xels.Features.FederatedPeg.NetworkHelpers;

namespace Xels.Features.FederatedPeg.TargetChain
{
    public class FederatedPegBroadcaster : IFederatedPegBroadcaster
    {
        private readonly IConnectionManager connectionManager;
        private readonly IFederatedPegSettings federatedPegSettings;

        public FederatedPegBroadcaster(
            IConnectionManager connectionManager,
            IFederatedPegSettings federatedPegSettings)
        {
            this.connectionManager = connectionManager;
            this.federatedPegSettings = federatedPegSettings;
        }

        /// <inheritdoc />
        public async Task BroadcastAsync(Payload payload)
        {
            // TODO: Optimize how federation peers are identified.

            List<INetworkPeer> peers = this.connectionManager.ConnectedPeers.ToList();

            var ipAddressComparer = new IPAddressComparer();

            // TODO: Can do the send to each peer in parallel.

            foreach (INetworkPeer peer in peers)
            {
                // Broadcast to peers.
                if (!peer.IsConnected)
                    continue;

                if (this.federatedPegSettings.FederationNodeIpEndPoints.Any(e => ipAddressComparer.Equals(e.Address, peer.PeerEndPoint.Address)))
                {
                    try
                    {
                        await peer.SendMessageAsync(payload).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                }
            }
        }
    }
}
