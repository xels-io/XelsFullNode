using NBitcoin;
using Xels.Bitcoin.Consensus;
using Xels.Bitcoin.Features.SmartContracts.Rules;
using Xels.SmartContracts.CLR;
using Xels.SmartContracts.Core.ContractSigning;

namespace Xels.Bitcoin.Features.SmartContracts.PoA.Rules
{
    /// <summary>
    /// Validates that the supplied smart contract code is signed with a valid signature.
    /// Depends on <see cref="SignedCodeCallDataSerializer"/> being injected into the node.
    /// </summary>
    public class ContractSignedCodeLogic : IContractTransactionPartialValidationRule
    {
        private readonly IContractSigner contractSigner;
        private readonly PubKey signingContractPubKey;

        public ContractSignedCodeLogic(
            IContractSigner contractSigner,
            PubKey signingContractPubKey)
        {
            this.contractSigner = contractSigner;
            this.signingContractPubKey = signingContractPubKey;
        }

        public void CheckContractTransaction(ContractTxData txData, Money suppliedBudget)
        {
            if (!txData.IsCreateContract)
            {
                // We do not need to validate calls.
                return;
            }

            // If this rule is being used as part of consensus, then we need to have the SignedCodeCallDataSerializer being used.
            // If the below line is throwing, it must not be being used.
            var signedTxData = (SignedCodeContractTxData)txData;

            if (!this.contractSigner.Verify(this.signingContractPubKey, signedTxData.ContractExecutionCode, signedTxData.CodeSignature))
            {
                this.ThrowInvalidCode();
            }
        }
        
        private void ThrowInvalidCode()
        {
            new ConsensusError("contract-code-invalid-signature", "Contract code does not have a valid signature").Throw();
        }
    }
}