using System.Collections.Generic;
using NBitcoin;
using Xels.SmartContracts.CLR.ContractLogging;
using Xels.SmartContracts.CLR.Serialization;
using Xels.SmartContracts.Core.Receipts;
using Xels.SmartContracts.Core.State;
using Xels.SmartContracts.Core.State.AccountAbstractionLayer;

namespace Xels.SmartContracts.CLR
{
    public interface IState
    {
        IBlock Block { get; }
        BalanceState BalanceState { get; }
        IStateRepository ContractState { get; }
        IList<Log> GetLogs(IContractPrimitiveSerializer serializer);
        IReadOnlyList<TransferInfo> InternalTransfers { get; }
        IContractLogHolder LogHolder { get; }
        IState Snapshot();
        NonceGenerator NonceGenerator { get; }
        void TransitionTo(IState state);
        void AddInternalTransfer(TransferInfo transferInfo);
        ulong GetBalance(uint160 address);
        uint160 GenerateAddress(IAddressGenerator addressGenerator);
        ISmartContractState CreateSmartContractState(IState state, RuntimeObserver.IGasMeter gasMeter, uint160 address, BaseMessage message, IStateRepository repository);
        void AddInitialTransfer(TransferInfo initialTransfer);
    }
}