using System;
using NBitcoin;
using Newtonsoft.Json;
using Xels.Bitcoin.Utilities.JsonConverters;

namespace XelsDesktopWalletApp.Models
{
    public class WalletGeneralInfoModel
    {
        public string walletName { get; set; }
        public string network { get; set; }
        public string creationTime { get; set; }
        public bool isDecrypted { get; set; }
        public int? lastBlockSyncedHeight { get; set; }
        public int? chainTip { get; set; }
        public bool isChainSynced { get; set; }
        public int connectedNodes { get; set; }
    }
}
