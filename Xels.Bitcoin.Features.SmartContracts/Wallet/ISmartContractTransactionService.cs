using Xels.Bitcoin.Features.SmartContracts.Models;
using Xels.SmartContracts.CLR;

namespace Xels.Bitcoin.Features.SmartContracts.Wallet
{
    public interface ISmartContractTransactionService
    {
        EstimateFeeResult EstimateFee(ScTxFeeEstimateRequest request);
        BuildContractTransactionResult BuildTx(BuildContractTransactionRequest request);
        BuildCallContractTransactionResponse BuildCallTx(BuildCallContractTransactionRequest request);
        BuildCreateContractTransactionResponse BuildCreateTx(BuildCreateContractTransactionRequest request);
        ContractTxData BuildLocalCallTxData(LocalCallContractRequest request);
    }
}