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


            //// creates new mnemonic only
            //Mnemonic mnemo = new Mnemonic(Wordlist.English, WordCount.Twelve);
            //ExtKey hdRoot = mnemo.DeriveExtKey("my password");
            //Console.WriteLine(mnemo);

            Mnemonic mnemo = new Mnemonic("above turn genuine amused sister grocery fiscal draft wedding chat process prosper", 
                Wordlist.English);
            ExtKey hdRoot = mnemo.DeriveExtKey("Shuvo912#");

            return wallet;
        }

        
        public Wallet WalletCreationFromPk(string pKey)
        {
            Wallet wallet = new Wallet();

            return wallet;
        }
    }


}
