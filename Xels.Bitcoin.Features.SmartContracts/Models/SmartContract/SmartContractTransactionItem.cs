using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;

namespace Xels.Bitcoin.Features.SmartContracts.Models.SmartContract
{
    public class SmartContractTransactionItem
    {
        public int? BlockHeight { get; set; }
        public string Type { get; set; }
        public uint256 Hash { get; set; }
        public string To { get; set; }
        public decimal Amount { get; set; }
        public decimal TransactionFee { get; set; }
        public decimal GasFee { get; set; }
    }
}
