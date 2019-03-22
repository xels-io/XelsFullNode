using Xels.Bitcoin.Features.SmartContracts.Models;
using Xels.SmartContracts.CLR;

namespace Xels.Bitcoin.Features.SmartContracts.Wallet
{
    public interface ISmartContractTransactionService
    {
        BuildCallContractTransactionResponse BuildCallTx(BuildCallContractTransactionRequest request);
        BuildCreateContractTransactionResponse BuildCreateTx(BuildCreateContractTransactionRequest request);
        ContractTxData BuildLocalCallTxData(LocalCallContractRequest request);

    }
}