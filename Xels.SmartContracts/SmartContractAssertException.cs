using System;

namespace Xels.SmartContracts
{
    public class SmartContractAssertException : Exception
    {
        public SmartContractAssertException(string message) : base(message) { }
    }
}
