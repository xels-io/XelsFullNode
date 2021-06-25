using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;
using System.Runtime.Caching;
using System.IO;

using Nethereum.Hex.HexConvertors.Extensions;
using System.Threading.Tasks;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json;
using System.Linq;

namespace XelsDesktopWalletApp.Common
{

    public class Wallet
    {
        public string Address { get; set; }
        public string PrivateKey { get; set; }
    }

    public class StoredWallet
    {
        public string Address { get; set; }
        public string PrivateKey { get; set; }
        public string Walletname { get; set; }
        public string Coin { get; set; }
        public string Wallethash { get; set; }
    }

    public class CreateWallet
    {
        List<StoredWallet> storedWallets = new List<StoredWallet>();
        //public Wallet WalletCreation(string mnemonic)
        //{
        //    Wallet wallet = new Wallet();

        //    //// creates new mnemonic only
        //    //Mnemonic mnemo = new Mnemonic(Wordlist.English, WordCount.Twelve);
        //    //ExtKey hdRoot = mnemo.DeriveExtKey("my password");
        //    //Console.WriteLine(mnemo);

        //    // try2
        //    Mnemonic mnemo = new Mnemonic("above turn genuine amused sister grocery fiscal draft wedding chat process prosper",
        //        Wordlist.English);
        //    ExtKey hdRoot = mnemo.DeriveExtKey("Shuvo912#");


        //    //try3
        //    var privateKey = "0000000000000000000000000000000000000000000000000000000000000001";
        //    var account = new Account(privateKey);

        //    wallet.PrivateKey = privateKey;
        //    wallet.Address = account.Address;

        //    return wallet;
        //}


        public Wallet WalletCreationFromPk(string pKey)
        {
            Wallet wallet = new Wallet();
            var account = new Account(pKey);

            wallet.PrivateKey = pKey;
            wallet.Address = account.Address;

            return wallet;
        }

        public void StoreLocally(Wallet wallet, string walletname, string symbol, string wallethash)
        {
            if (walletname != null)
            {
                this.storedWallets = RetrieveWallets();

                StoredWallet storedWallet = new StoredWallet();
                storedWallet.Address = wallet.Address;
                storedWallet.PrivateKey = wallet.PrivateKey;
                storedWallet.Walletname = walletname;
                storedWallet.Coin = symbol;
                storedWallet.Wallethash = wallethash;

                this.storedWallets.Add(storedWallet);
                string JSONresult = JsonConvert.SerializeObject(this.storedWallets.ToArray(), Formatting.Indented);

                string path = @"D:\Towsif\Projects\xels-fullnode-wpf-v3\XelsDesktopWalletApp\File\Wallets.json";


                if (File.Exists(path))
                {
                    File.Delete(path);

                    using (var sw = new StreamWriter(path, true))
                    {
                        sw.WriteLine(JSONresult.ToString());
                        sw.Close();
                    }
                }
                else if (!File.Exists(path))
                {
                    using (var sw = new StreamWriter(path, true))
                    {
                        sw.WriteLine(JSONresult.ToString());
                        sw.Close();
                    }
                }


            }

        }




        public List<StoredWallet> RetrieveWallets()
        {
            using (StreamReader r = new StreamReader(@"D:\Towsif\Projects\xels-fullnode-wpf-v3\XelsDesktopWalletApp\File\Wallets.json"))
            {
                string json = r.ReadToEnd();
                List<StoredWallet> wallets = new List<StoredWallet>();

                if (json != null)
                {
                    wallets = JsonConvert.DeserializeObject<List<StoredWallet>>(json);
                }

                return wallets;
            }
        }

        public StoredWallet GetLocalWallet(string walletname, string symbol)
        {
            StoredWallet wallet = new StoredWallet();

            using (StreamReader r = new StreamReader(@"D:\Towsif\Projects\xels-fullnode-wpf-v3\XelsDesktopWalletApp\File\Wallets.json"))
            {
                string json = r.ReadToEnd();
                List<StoredWallet> wallets = new List<StoredWallet>();

                if (json != null)
                {
                    wallets = JsonConvert.DeserializeObject<List<StoredWallet>>(json);
                }
                wallet = wallets.Where(c => c.Walletname == walletname && c.Coin == symbol).FirstOrDefault();

                return wallet;
            }
        }






    }


}
