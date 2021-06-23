using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;

namespace XelsDesktopWalletApp.Common
{

    public class Wallet
    {
        public string Address { get; set; }
        public string PrivateKey { get; set; }
    }

    public class CreateWallet
    {
        public Wallet WalletCreation(string mnemonic)
        {
            Wallet wallet = new Wallet();

            Mnemonic mnemo = new Mnemonic(Wordlist.English, WordCount.Twelve);
            ExtKey hdRoot = mnemo.DeriveExtKey("my password");
            Console.WriteLine(mnemo);

            return wallet;
        }

        
        public Wallet WalletCreationFromPk(string pKey)
        {
            Wallet wallet = new Wallet();

            return wallet;
        }
    }


}
