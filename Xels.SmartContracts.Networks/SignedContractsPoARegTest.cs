using NBitcoin;
using Xels.Bitcoin.Features.SmartContracts;

namespace Xels.SmartContracts.Networks
{
    public class SignedContractsPoARegTest : SmartContractsPoARegTest, ISignedCodePubKeyHolder
    {
        public Key SigningContractPrivKey { get;}

        public PubKey SigningContractPubKey { get;}

        public SignedContractsPoARegTest()
        {
            this.Name = "SignedContractsPoARegTest";
            this.NetworkType = NetworkType.Regtest;
            this.SigningContractPrivKey = new Mnemonic("lava frown leave wedding virtual ghost sibling able mammal liar wide wisdom").DeriveExtKey().PrivateKey;
            this.SigningContractPubKey = this.SigningContractPrivKey.PubKey;
        }
    }
}
