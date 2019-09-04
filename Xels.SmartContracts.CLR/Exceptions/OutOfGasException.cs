using Xels.SmartContracts.Core.Exceptions;

namespace Xels.SmartContracts.CLR.Exceptions
{
    public class OutOfGasException : SmartContractException
    {
        public OutOfGasException(string message) : base(message) { }
    }
}