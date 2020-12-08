﻿using System;

namespace Xels.SmartContracts
{
    public interface ISmartContractState
    {
        IBlock Block { get; }
        IMessage Message { get; }
        IPersistentState PersistentState { get; }
        IContractLogger ContractLogger { get; }
        IInternalTransactionExecutor InternalTransactionExecutor { get; }
        IInternalHashHelper InternalHashHelper { get; }
        ISerializer Serializer { get; }
        Func<ulong> GetBalance { get; }
    }
}