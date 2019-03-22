using NBitcoin;
using Xels.SmartContracts.Core.State;

namespace Xels.SmartContracts.CLR
{
    public interface ISmartContractStateFactory
    {
        /// <summary>
        /// Sets up a new <see cref="ISmartContractState"/> based on the current state.
        /// </summary>        
        ISmartContractState Create(IState state, IGasMeter gasMeter, uint160 address, BaseMessage message,
            IStateRepository repository);
    }
}