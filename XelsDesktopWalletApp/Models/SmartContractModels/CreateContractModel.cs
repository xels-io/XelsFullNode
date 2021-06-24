using System;
using System.Collections.Generic;
using System.Text;

namespace XelsDesktopWalletApp.Models.SmartContractModels
{
    public class CreateContractModel
    {
        public string walletName { get; set; }
        public string accountName { get; set; }

        //public List<OutpointRequest> Outpoints { get; set; }

        public string amount { get; set; }
        public string feeAmount { get; set; }
        public string password { get; set; }
        public string contractCode { get; set; }
        public ulong gasPrice { get; set; }

        public ulong gasLimit { get; set; }

        public string sender { get; set; }
        public string[] parameters { get; set; }

    }
}
