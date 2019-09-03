using Xels.SmartContracts.Core.State;

namespace Xels.SmartContracts.CLR
{
    public interface IVirtualMachine
    {
        VmExecutionResult Create(IStateRepository repository, 
            ISmartContractState contractState,
            RuntimeObserver.IGasMeter gasMeter,
            byte[] contractCode,
            object[] parameters,
            string typeName = null);

        VmExecutionResult ExecuteMethod(ISmartContractState contractState, 
            RuntimeObserver.IGasMeter gasMeter,
            MethodCall methodCall,
            byte[] contractCode, string typeName);
    }
}
