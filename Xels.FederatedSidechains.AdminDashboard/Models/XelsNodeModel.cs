using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Xels.FederatedSidechains.AdminDashboard.Entities;
using Xels.FederatedSidechains.AdminDashboard.Services;

namespace Xels.FederatedSidechains.AdminDashboard.Models
{
    public class XelsNodeModel
    {
        public int SyncingStatus { get; set; }
        public string WebAPIUrl { get; set; } = "http://localhost:38221/api";
        public string SwaggerUrl { get; set; } = "http://localhost:38221/swagger";
        public int BlockHeight { get; set; }
        public int MempoolSize { get; set; }
        public string BlockHash { get; set; }
        public double ConfirmedBalance { get; set; }
        public double UnconfirmedBalance { get; set; }
        public List<Peer> Peers { get; set; }
        public List<Peer> FederationMembers { get; set; }
        public object History { get; set; }
        public string CoinTicker { get; set; }
        public List<LogRule> LogRules { get; set; }
    }
}