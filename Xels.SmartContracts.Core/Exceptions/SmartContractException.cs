using System;

namespace Xels.SmartContracts.Core.Exceptions
{
    public abstract class SmartContractException : Exception
    {
        protected SmartContractException() { }

        protected SmartContractException(string message)
            : base(message)
        {
        }
    }
}