using Xels.Bitcoin.Features.MemoryPool;

namespace Xels.Bitcoin.Features.SmartContracts
{
    /// <summary>
    /// Smart-contract related rule to be checked before transactions are added to the mempool.
    /// </summary>
    public interface ISmartContractMempoolRule
    {
        /// <summary>
        /// Check that the transaction meets certain criteria before being added to mempool. If it doesn't, a ConsensusErrorException
        /// will be thrown.
        /// </summary>
        void CheckTransaction(MempoolValidationContext context);
    }
}