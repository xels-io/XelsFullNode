using NBitcoin;
using Xels.SmartContracts.Core;

namespace Xels.Bitcoin.Features.SmartContracts
{
    public interface ISmartContractBlockHeader
    {
        uint256 HashStateRoot { get; set; }

        uint256 ReceiptRoot { get; set; }

        Bloom LogsBloom { get; set; }
    }
}
