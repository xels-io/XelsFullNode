using Xels.SmartContracts;

public class SingleConstructor : SmartContract
    {
        public SingleConstructor(ISmartContractState smartContractState)
            : base(smartContractState)
        {
        }
    }