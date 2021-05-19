using System;
using System.Collections.Generic;
using System.Text;

namespace XelsDesktopWalletApp.Models
{
    public class WalletRecovery
    {
        public string name { get; set; }
        public string mnemonic { get; set; }

        // public DateTime creationDate { get; set; }

        public string creationDate { get; set; }
        public string password { get; set; }
        public string passphrase { get; set; }
    }
}
