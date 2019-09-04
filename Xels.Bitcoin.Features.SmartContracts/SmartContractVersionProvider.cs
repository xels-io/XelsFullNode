using Xels.Bitcoin.Interfaces;

namespace Xels.Bitcoin.Features.SmartContracts
{
    public sealed class SmartContractVersionProvider : IVersionProvider
    {
        /// <inheritdoc />
        public string GetVersion()
        {
            return "1.0.0";
        }
    }
}
