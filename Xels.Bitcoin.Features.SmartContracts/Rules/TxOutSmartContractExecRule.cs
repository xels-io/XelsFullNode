﻿using System.Linq;
using System.Threading.Tasks;
using NBitcoin;
using Xels.Bitcoin.Consensus;
using Xels.Bitcoin.Consensus.Rules;
using Xels.Bitcoin.Features.MemoryPool;
using Xels.SmartContracts.Core;

namespace Xels.Bitcoin.Features.SmartContracts.Rules
{
    /// <summary>
    /// Each transaction should have only 1 'SmartContractExec' output.
    /// </summary>
    public class TxOutSmartContractExecRule : FullValidationConsensusRule, ISmartContractMempoolRule
    {
        public override Task RunAsync(RuleContext context)
        {
            Block block = context.ValidationContext.BlockToValidate;

            foreach (Transaction transaction in block.Transactions)
            {
                this.CheckTransaction(transaction);
            }

            return Task.CompletedTask;
        }

        public void CheckTransaction(MempoolValidationContext context)
        {
            this.CheckTransaction(context.Transaction);
        }

        private void CheckTransaction(Transaction transaction)
        {
            int smartContractExecCount = transaction.Outputs.Count(o => o.ScriptPubKey.IsSmartContractExec());

            if ((transaction.IsCoinBase)  && smartContractExecCount > 0)
                new ConsensusError("smartcontractexec-in-coinbase", "coinbase contains smartcontractexec output").Throw();

            if (smartContractExecCount > 1)
                new ConsensusError("multiple-smartcontractexec-outputs", "transaction contains multiple smartcontractexec outputs").Throw();
        }
    }
}
