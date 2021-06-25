using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;

namespace XelsDesktopWalletApp.Models.SmartContractModels
{
    public class SmartContractTransactionItem
    {
        public int? blockHeight { get; set; }
        public SmartContractTransactionItemType type { get; set; }
        //public uint256 hash { get; set; }
        public string hash { get; set; }
        public string to { get; set; }
        public decimal amount { get; set; }
        public decimal transactionFee { get; set; }
        public decimal gasFee { get; set; }
    }

    public enum SmartContractTransactionItemType
    {
        Received,
        Send,
        Staked,
        ContractCall,
        ContractCreate,
        GasRefund
    }
}
