using Newtonsoft.Json.Linq;

namespace Xels.FederatedSidechains.AdminDashboard.Models
{
    public class DashboardModel
    {
        public bool IsCacheBuilt { get; set; }
        public bool Status { get; set; }
        public string MainchainWalletAddress { get; set; }
        public string SidechainWalletAddress { get; set; }
        public JArray MiningPublicKeys { get; set; }
        public XelsNodeModel XelsNode { get; set; }
        public XelsNodeModel SidechainNode { get; set; }
    }
}