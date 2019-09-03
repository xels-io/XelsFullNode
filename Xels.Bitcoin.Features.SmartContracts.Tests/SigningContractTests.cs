using NBitcoin.Crypto;
using Xels.SmartContracts.Networks;
using Xunit;

namespace Xels.Bitcoin.Features.SmartContracts.Tests
{
    public class SigningContractTests
    {
        private readonly SignedContractsPoARegTest network;

        public SigningContractTests()
        {
            this.network = new SignedContractsPoARegTest();
        }

        [Fact]
        public void SignContract()
        {
            byte[] contractCode = new byte[12];
            ECDSASignature signature = this.network.SigningContractPrivKey.SignMessageBytes(contractCode);

            Assert.True(this.network.SigningContractPubKey.VerifyMessage(contractCode, signature));
        }
    }
}
