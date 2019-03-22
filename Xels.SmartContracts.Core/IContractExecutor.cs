namespace Xels.SmartContracts.Core
{
    public interface IContractExecutor
    {
        IContractExecutionResult Execute(IContractTransactionContext transactionContext);
    }
}
