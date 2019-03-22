using NBitcoin;
using Xels.SmartContracts.Core;
using Xels.SmartContracts.Core.Receipts;
using Xels.SmartContracts.Core.State;
using Xels.SmartContracts.Core.Util;
using Xels.SmartContracts.CLR;

namespace Xels.Bitcoin.Features.SmartContracts
{
    public interface ISmartContractCoinviewRule
    {
        ICallDataSerializer CallDataSerializer { get; }
        IContractExecutorFactory ExecutorFactory { get; }
        Network Network { get; }
        IStateRepositoryRoot OriginalStateRoot { get; }
        IReceiptRepository ReceiptRepository { get; }
        ISenderRetriever SenderRetriever { get; }
    }
}