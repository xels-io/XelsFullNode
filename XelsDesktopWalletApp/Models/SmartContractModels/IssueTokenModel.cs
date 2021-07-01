using System;
using System.Collections.Generic;
using System.Text;

namespace XelsDesktopWalletApp.Models.SmartContractModels
{
    public class IssueTokenModel
    {
        public string amount { get; set; }
        public string feeAmount { get; set; }
        public ulong gasPrice { get; set; }
        public ulong gasLimit { get; set; }

        public  string[] parameters { get; set; }

        public string ContractCode { get; set; }

        public string password { get; set; }

        public string walletName { get; set; }
        public string sender { get; set; }

 

        //public string totalSupply { get; set; }

        //public string tokenName { get; set; }

        //public string tokenSymbol { get; set; }

        //public string decimals { get; set; }

    }
        public class Parameter
        {
            public int type { get; set; }
            public string value { get; set; }
        }

    }
