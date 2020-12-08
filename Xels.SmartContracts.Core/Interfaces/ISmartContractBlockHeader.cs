﻿using NBitcoin;

namespace Xels.SmartContracts.Core.Interfaces
{
    public interface ISmartContractBlockHeader
    {
        uint256 HashStateRoot { get; set; }

        uint256 ReceiptRoot { get; set; }

        Bloom LogsBloom { get; set; }
    }
}
