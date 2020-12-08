using System.Collections.Generic;
using Xels.SmartContracts.Core;
using Xels.SmartContracts.Core.Receipts;
using Xels.SmartContracts.Core.State.AccountAbstractionLayer;

namespace Xels.SmartContracts.CLR.Local
{
    public class LocalExecutionResult : ILocalExecutionResult
    {
        public IReadOnlyList<TransferInfo> InternalTransfers { get; set; }
        public RuntimeObserver.Gas GasConsumed { get; set; }
        public bool Revert { get; set; }
        public ContractErrorMessage ErrorMessage { get; set; }
        public object Return { get; set; }
        public IList<Log> Logs { get; set; }
    }
}