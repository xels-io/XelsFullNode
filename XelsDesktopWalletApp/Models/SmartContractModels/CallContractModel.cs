using System;
using System.Collections.Generic;
using System.Text;

namespace XelsDesktopWalletApp.Models.SmartContractModels
{
    public class CallContractModel
    {
        public string walletName { get; set; }
        public string accountName { get; set; }
        public string contractAddress { get; set; }
        public string methodName { get; set; }
        public string amount { get; set; }
        public string feeAmount { get; set; }
        public string password { get; set; }
        public ulong gasPrice { get; set; }
        public ulong gasLimit { get; set; }
        public string sender { get; set; }
        public string[] parameters { get; set; }
    }
}
