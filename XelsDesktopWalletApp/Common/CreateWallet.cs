using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;

using Nethereum.Hex.HexConvertors.Extensions;
using System.Threading.Tasks;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

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

            // try2
            Mnemonic mnemo = new Mnemonic("above turn genuine amused sister grocery fiscal draft wedding chat process prosper",
                Wordlist.English);
            ExtKey hdRoot = mnemo.DeriveExtKey("Shuvo912#");


            //try3
            var privateKey = "0000000000000000000000000000000000000000000000000000000000000001";
            var account = new Account(privateKey);

            wallet.PrivateKey = privateKey;
            wallet.Address = account.Address;

            return wallet;
        }

        
        public Wallet WalletCreationFromPk(string pKey)
        {
            Wallet wallet = new Wallet();
            var account = new Account(pKey);

            wallet.PrivateKey = pKey;
            wallet.Address = account.Address;

            return wallet;
        }

    }


}
