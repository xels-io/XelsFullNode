using System;
using Xels.SmartContracts;

[Deploy]
public sealed class ThrowExceptionContract : SmartContract
{
    public ThrowExceptionContract(ISmartContractState state)
        : base(state)
    {
    }

    public void ThrowException()
    {
        throw new Exception();
    }
}