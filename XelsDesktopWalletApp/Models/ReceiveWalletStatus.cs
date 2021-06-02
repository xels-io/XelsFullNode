using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;
using Newtonsoft.Json;

namespace XelsDesktopWalletApp.Models
{
    public class ReceiveWalletStatus
    {
        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }

        [JsonProperty(PropertyName = "isUsed")]
        public bool IsUsed { get; set; }

        [JsonProperty(PropertyName = "isChange")]
        public bool IsChange { get; set; }

        public Money AmountConfirmed { get; set; }
        public Money AmountUnconfirmed { get; set; }
    }

    // After getting the full list of received wallet. Then put them into seperate arraylist 
    public class ReceiveWalletArray
    {
        public ReceiveWalletStatus[] addresses { get; set; }
    }

    public class SelectedAddress
    {
        public string address { get; set; }
    }

    public class ReceiveWalletList
    {
        public Dictionary<string, ReceiveWalletStatus> Wallets { get; set; }

    }

}
