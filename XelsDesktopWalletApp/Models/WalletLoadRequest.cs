using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XelsDesktopWalletApp.Models
{
    public class WalletLoadRequest
    {
        public WalletLoadRequest()
        {
            this.name = "Empty Name";
            this.password = "";
        }

        [Required(ErrorMessage = "A password is required.")]
        public string password { get; set; }

        [Required(ErrorMessage = "The name of the wallet is missing.")]
        public string name { get; set; }
    }
}
