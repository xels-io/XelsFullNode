using NBitcoin;
using Xels.SmartContracts.Core.State;

namespace Xels.SmartContracts.CLR
{
    public interface IStateFactory
    {
        IState Create(IStateRepository stateRoot, IBlock block, ulong txOutValue, uint256 transactionHash);
    }
}