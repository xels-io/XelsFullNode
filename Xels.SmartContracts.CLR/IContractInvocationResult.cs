using Xels.SmartContracts.Core;

namespace Xels.SmartContracts.CLR
{
    public interface IContractInvocationResult
    {
        bool IsSuccess { get; }

        ContractInvocationErrorType InvocationErrorType { get; }

        ContractErrorMessage ErrorMessage { get; }

        object Return { get; }
    }
}