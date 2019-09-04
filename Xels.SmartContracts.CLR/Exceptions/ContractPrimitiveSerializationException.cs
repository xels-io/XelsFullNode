using Xels.SmartContracts.Core.Exceptions;

namespace Xels.SmartContracts.CLR.Exceptions
{
    public sealed class ContractPrimitiveSerializationException : SmartContractException
    {
        public ContractPrimitiveSerializationException(string message) : base(message) {}
    }
}