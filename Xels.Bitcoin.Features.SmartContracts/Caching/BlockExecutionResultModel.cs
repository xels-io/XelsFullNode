using System.Collections.Generic;
using Xels.SmartContracts.Core.Receipts;
using Xels.SmartContracts.Core.State;

namespace Xels.Bitcoin.Features.SmartContracts.Caching
{
    /// <summary>
    /// Contains information that was obtained through execution of a single block's smart contracts.
    /// </summary>
    public class BlockExecutionResultModel
    {
        public IStateRepositoryRoot MutatedStateRepository { get; }
        public List<Receipt> Receipts { get; }

        public BlockExecutionResultModel(IStateRepositoryRoot stateRepo, List<Receipt> receipts)
        {
            this.MutatedStateRepository = stateRepo;
            this.Receipts = receipts;
        }
    }
}
