using System;
using System.Collections.Generic;
using System.Text;

namespace XelsDesktopWalletApp.Models.SmartContractModels
{
    public class SmtGetHistoryRequest
    {
        public string WalletName { get; set; }
        public string Address { get; set; }
        public int? Skip { get; set; }
        public int? Take { get; set; }
    }
}
