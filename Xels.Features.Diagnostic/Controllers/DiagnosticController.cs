﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Xels.Bitcoin.Connection;
using Xels.Bitcoin.Consensus;
using Xels.Bitcoin.Controllers;
using Xels.Bitcoin.P2P.Peer;
using Xels.Bitcoin.Utilities.JsonErrors;
using Xels.Features.Diagnostic.Controllers.Models;
using Xels.Features.Diagnostic.PeerDiagnostic;
using Xels.Features.Diagnostic.Utils;

namespace Xels.Features.Diagnostic.Controllers
{
    /// <summary>
    /// Controller providing diagnostic operations on fullnode.
    /// </summary>
    [ApiVersion("1")]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DiagnosticController : FeatureController
    {
        private readonly PeerStatisticsCollector peerStatisticsCollector;

        public DiagnosticController(PeerStatisticsCollector peerStatisticsCollector, IConnectionManager connectionManager, IConsensusManager consensusManager)
            : base(connectionManager: connectionManager, consensusManager: consensusManager)
        {
            this.peerStatisticsCollector = peerStatisticsCollector;
        }

        /// <summary>
        /// Returns the connected peers with some information
        /// </summary>
        [HttpGet]
        public IActionResult GetConnectedPeersInfo()
        {
            try
            {
                var peersByPeerId = this.ConsensusManager.GetPrivateFieldValue<Dictionary<int, INetworkPeer>>("peersByPeerId").Values.ToList();
                var connectedPeers = this.ConnectionManager.ConnectedPeers.ToList();

                object DumpPeer(INetworkPeer peer)
                {
                    return new { peer.IsConnected, peer.DisconnectReason, peer.State, EndPoint = peer.PeerEndPoint.ToString() };
                }

                return this.Ok(new
                {
                    peersByPeerId = peersByPeerId.Select(DumpPeer),
                    connectedPeers = connectedPeers.Select(DumpPeer),
                    connectedPeersNotInPeersByPeerId = connectedPeers.Except(peersByPeerId).Select(DumpPeer)
                });
            }
            catch (Exception e)
            {
                return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, e.Message, e.ToString());
            }
        }

        /// <summary>
        /// Gets the Diagnostic Feature status.
        /// </summary>
        /// <returns>The Diagnostic Feature status</returns>
        [HttpGet]
        public IActionResult GetStatus()
        {
            try
            {
                return this.Ok(new
                {
                    PeerStatistics = this.peerStatisticsCollector.Enabled ? "Enabled" : "Disabled"
                });
            }
            catch (Exception e)
            {
                return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, e.Message, e.ToString());
            }
        }

        /// <summary>
        /// Returns the connected peers with some statistical information.
        /// </summary>
        /// <param name="connectedOnly">if set to <c>true</c> returns statistics related to connected peers only.</param>
        /// <returns>List of peer statistics</returns>
        [HttpGet]
        public ActionResult<List<PeerStatisticsModel>> GetPeerStatistics(bool connectedOnly)
        {
            try
            {
                IEnumerable<PeerStatistics> peerStatistics = this.peerStatisticsCollector.GetStatistics();
                IEnumerable<IPEndPoint> connectedPeersEndpoints = this.ConnectionManager.ConnectedPeers.ToList().Select(peer => peer.PeerEndPoint);

                if (connectedOnly)
                {
                    peerStatistics = peerStatistics.Where(peer => connectedPeersEndpoints.Contains(peer.PeerEndPoint));
                }

                return this.Ok(peerStatistics.Select(peer => new PeerStatisticsModel(peer, connectedPeersEndpoints.Contains(peer.PeerEndPoint))));
            }
            catch (Exception e)
            {
                return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, e.Message, e.ToString());
            }
        }

        /// <summary>
        /// Starts collecting peers statistics.
        /// </summary>
        /// <returns>Operation result.</returns>
        [HttpGet]
        public IActionResult StartCollectingPeerStatistics()
        {
            try
            {
                if (this.peerStatisticsCollector.Enabled)
                {
                    return Ok("Diagnostic Peer Statistic Collector already enabled.");
                }
                else
                {
                    this.peerStatisticsCollector.StartCollecting();
                    return Ok("Diagnostic Peer Statistic Collector enabled.");
                }
            }
            catch (Exception e)
            {
                return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, e.Message, e.ToString());
            }
        }

        /// <summary>
        /// Stops collecting peers statistics.
        /// Stopping a running peer statistic collecotr doesn't clear obtained results.
        /// </summary>
        /// <returns>Operation result.</returns>
        [HttpGet]
        public IActionResult StopCollectingPeerStatistics()
        {
            try
            {
                if (!this.peerStatisticsCollector.Enabled)
                {
                    return Ok("Diagnostic Peer Statistic Collector already disabled.");
                }
                else
                {
                    this.peerStatisticsCollector.StopCollecting();
                    return Ok("Diagnostic Peer Statistic Collector disabled.");
                }
            }
            catch (Exception e)
            {
                return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, e.Message, e.ToString());
            }
        }
    }
}
