using NBitcoin;
using Xels.SmartContracts.CLR.Local;

namespace Xels.SmartContracts.CLR
{
    public interface ILocalExecutor
    {
        ILocalExecutionResult Execute(ulong blockHeight, uint160 sender, Money txOutValue, ContractTxData txData);
    }
}
