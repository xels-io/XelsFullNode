using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xels.FederatedSidechains.AdminDashboard.Models;

namespace Xels.FederatedSidechains.AdminDashboard.Helpers
{
    public static class LogLevelHelper
    {
        public static string[] DefaultLogRules
        {
            get
            {
                return new string[]
                {
                    "*",
                    "Xels.Bitcoin.Features.Api.*",
                    "Xels.Bitcoin.Features.BlockStore.*",
                    "Xels.Bitcoin.Features.Consensus.*",
                    "Xels.Bitcoin.Consensus.*",
                    "Xels.Bitcoin.Consensus.ChainedHeaderTree",
                    "Xels.Bitcoin.Consensus.ConsensusManager",
                    "Xels.Bitcoin.Features.Consensus.CoinViews.*",
                    "Xels.Bitcoin.Features.Dns.*",
                    "Xels.Bitcoin.Features.LightWallet.*",
                    "Xels.Bitcoin.Features.MemoryPool.*",
                    "Xels.Bitcoin.Features.Miner.*",
                    "Xels.Bitcoin.Features.Notifications.*",
                    "Xels.Bitcoin.Features.RPC.*",
                    "Xels.Bitcoin.Features.Wallet.*",
                    "Xels.Bitcoin.Features.WatchOnlyWallet.*",
                    "Xels.Bitcoin.Consensus.ConsensusManagerBehavior",
                    "Xels.Bitcoin.Base.*",
                    "Xels.Bitcoin.Base.TimeSyncBehaviorState",
                    "Xels.Bitcoin.BlockPulling.*",
                    "Xels.Bitcoin.Connection.*",
                    "Xels.Bitcoin.P2P.*"
                };
            }
        }
    }
}
