using Xels.Bitcoin.Interfaces;

namespace Xels.Bitcoin.Features.SmartContracts
{
    public class SmartContractVersionProvider : IVersionProvider
    {
        public string GetVersion()
        {
            return "0.13.0";
        }
    }
}
