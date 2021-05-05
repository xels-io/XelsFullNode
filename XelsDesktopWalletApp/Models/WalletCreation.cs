using System;
using System.Collections.Generic;
using System.Text;

namespace XelsDesktopWalletApp.Models
{
    public class WalletCreation
    {
        public string name { get; set; }
        public string password { get; set; }
        public string passphrase { get; set; }
        public string mnemonic { get; set; }
    }
}
