﻿using System.Linq;
using Xels.SmartContracts;

public class InvalidImplicitAssembly : SmartContract
{
    public InvalidImplicitAssembly(ISmartContractState state) : base(state)
    {
    }

    public void Test()
    {
        new string[] { }.ToList().Sort();
    }
}