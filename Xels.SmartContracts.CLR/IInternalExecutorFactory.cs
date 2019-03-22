namespace Xels.SmartContracts.CLR
{
    public interface IInternalExecutorFactory
    {
        IInternalTransactionExecutor Create(IState state);
    }
}