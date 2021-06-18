using System;
using System.Collections.Generic;
using System.Text;

namespace XelsDesktopWalletApp.Models
{
    public class StakingInfoModel 
    {
        public bool enabled { get; set; }
        public bool staking { get; set; }
        public string errors { get; set; }
        public long currentBlockSize { get; set; }
        public long currentBlockTx { get; set; }
        public long pooledTx { get; set; }
        public double difficulty { get; set; }
        public int searchInterval { get; set; }
        public long weight { get; set; }
        public long netStakeWeight { get; set; }
        public long immature { get; set; }
        public long expectedTime { get; set; }
    }
}
