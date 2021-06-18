using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;

namespace XelsDesktopWalletApp.Models
{
    public class WalletGeneralInfoModel
    {
        public string walletName { get; set; }
        public Network network { get; set; }
        public DateTimeOffset creationTime { get; set; }
        public bool isDecrypted { get; set; }
        public int? lastBlockSyncedHeight { get; set; }
        public int? chainTip { get; set; }
        public bool isChainSynced { get; set; }
        public int connectedNodes { get; set; }
    }
}
