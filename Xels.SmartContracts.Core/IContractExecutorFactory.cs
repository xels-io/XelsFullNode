using Xels.SmartContracts.Core.State;

namespace Xels.SmartContracts.Core
{
    public interface IContractExecutorFactory
    {
        IContractExecutor CreateExecutor(
            IStateRepositoryRoot stateRepository,
            IContractTransactionContext transactionContext);
    }
}
