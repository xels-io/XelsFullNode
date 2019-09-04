using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NBitcoin;
using NBitcoin.Networks;

namespace Xels.Bitcoin.Features.BlockExplorer
{
    public class TransactionInfo
    {
        public static bool TimeStamp = false;
      
        public uint256 TxId
        {
            get; set;
        }

        uint nVersion = 1;

        public uint Version
        {
            get
            {
                return nVersion;
            }
            set
            {
                this.nVersion = value;
            }
        }
       
        private uint nTime = Utils.DateTimeToUnixTime(DateTime.UtcNow);

        public uint Time
        {
            get
            {
                return this.nTime;
            }
            set
            {
                this.nTime = value;
            }
        }

        string inputs;
        string outputs;
        bool isCoinStake;
        List<TxInInfo> vin;
        List<TxOutInfo> vout;
        LockTime nLockTime;
        string networkType;

        public TransactionInfo(uint256 hash, uint nVersion, bool isCoinStake, uint nTime, TxInList vin, TxOutList vout, LockTime nLockTime,  string networkType)
        {
            this.isCoinStake = isCoinStake;
            this.TxId = hash;
            this.nVersion = nVersion;
            this.nTime = nTime;
            this.networkType = networkType;
            this.InitVIn(vin, this.networkType);
            this.InitVOut(vout,this.isCoinStake, this.networkType);
            InitInputOutput(vin, vout, this.networkType);
          //  InitAddress(vin, vout);
            this.nLockTime = nLockTime;
        }

        private void InitVIn(TxInList inList, string networkType)
        {
            this.vin = new List<TxInInfo>();
            foreach (TxIn txIn in inList)
            {
                this.vin.Add(new TxInInfo(txIn.PrevOut, txIn.ScriptSig, txIn.WitScript, networkType));
            }
        }

        private void InitVOut(TxOutList outList, bool isCoinStake , string networkType)
        {
            this.vout = new List<TxOutInfo>();
            foreach (TxOut txOut in outList)
            {
                //if (txOut.ScriptPubKey.GetDestinationAddress(Network.XelsMain) != null)
                //{
                    this.vout.Add(new TxOutInfo(txOut.ScriptPubKey, isCoinStake, txOut.Value, networkType));
                //}
            }
        }
        

        private void InitInputOutput(TxInList vin, TxOutList vout , string networkType)
        {
            foreach(TxIn txIn in vin)
            {
                this.inputs += txIn.ScriptSig.GetScriptAddress(NetworkRegistration.GetNetwork(networkType)) + " ";
            }
            foreach(TxOut txOut in vout)
            {
                //this.outputs += "dest:  " + txOut.ScriptPubKey.GetDestinationAddress(Network.XelsMain) + "  ";
                this.outputs += txOut.ScriptPubKey.GetScriptAddress(NetworkRegistration.GetNetwork(networkType)) + "  ";
                this.outputs += txOut.Value.ToString() + "  ";
            }
        }

        public Money TotalOut
        {
            get
            {
                return this.vout.Sum(v => v.Value);
            }
        }

        public LockTime LockTime
        {
            get
            {
                return nLockTime;
            }
            set
            {
                nLockTime = value;
            }
        }

        public string Inputs
        {
            get
            {
                return inputs;
            }
        }
        public string Outputs
        {
            get
            {
                return outputs;
            }
        }
        public List<TxInInfo> VIn
        {
            get
            {
                return vin;
            }
        }
        public List<TxOutInfo> VOut
        {
            get
            {
                return vout;
            }
        }
    }

    public class TxInInfo
    {
        OutPoint prevout = new OutPoint();
        Script scriptSig = Script.Empty;
        BitcoinAddress address;
        public BitcoinAddress Address
        {
            get
            {
                return address;
            }
            set
            {
                address = value;
            }
        }
        public OutPoint PrevOut
        {
            get
            {
                return prevout;
            }
            set
            {
                prevout = value;
            }
        }


        public Script ScriptSig
        {
            get
            {
                return scriptSig;
            }
            set
            {
                scriptSig = value;
            }
        }
        
        WitScript witScript = WitScript.Empty;

        public TxInInfo(OutPoint prevout, Script scriptSig, WitScript witScript , string networkType)
        {
            this.prevout = prevout;
            this.scriptSig = scriptSig;
            this.witScript = witScript;
            this.address = scriptSig.PaymentScript.GetDestinationAddress(NetworkRegistration.GetNetwork(networkType));  //scriptSig.GetScriptAddress(Network.XelsMain);witScript.ToScript().GetDestinationAddress(Network.XelsMain);//
        }

        /// <summary>
        /// The witness script (Witness script is not serialized and deserialized at the TxIn level, but at the Transaction level)
        /// </summary>
        public WitScript WitScript
        {
            get
            {
                return witScript;
            }
            set
            {
                witScript = value;
            }
        }
    }

    public class TxOutInfo : IDestination
    {
        BitcoinAddress address;
        public BitcoinAddress Address
        {
            get
            {
                return address;
            }
            set
            {
                address = value;
            }
        }

        Script publicKey = Script.Empty;
        public Script ScriptPubKey
        {
            get
            {
                return this.publicKey;
            }
            set
            {
                this.publicKey = value;
            }
        }
        
        readonly static Money NullMoney = new Money(-1);
        Money _Value = NullMoney;
        public bool cStake;

        public TxOutInfo(Script scriptPubKey, bool coinStake,  Money Value, string networkType)
        {
            this.cStake = coinStake;
            this.publicKey = scriptPubKey;
            this.ScriptPubKey = scriptPubKey.PaymentScript.Hash.ScriptPubKey ;
            this._Value = Value;
            this.address = scriptPubKey.PaymentScript.GetDestinationAddress(NetworkRegistration.GetNetwork(networkType));
            //this.address = scriptPubKey.GetDestinationAddress(Network.XelsMain);
        }

        public Money Value
        {
            get
            {
                return _Value;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _Value = value;
            }
        }
    }
}
