using System.Collections.Generic;
using Xels.SmartContracts.Core;
using Xels.SmartContracts.Core.Receipts;
using Xels.SmartContracts.Core.State.AccountAbstractionLayer;

namespace Xels.SmartContracts.CLR.Local
{
    public interface ILocalExecutionResult
    {
        IReadOnlyList<TransferInfo> InternalTransfers { get; }
        RuntimeObserver.Gas GasConsumed { get; }
        bool Revert { get; }
        ContractErrorMessage ErrorMessage { get; }
        object Return { get; }
        IList<Log> Logs { get; }
    }
}