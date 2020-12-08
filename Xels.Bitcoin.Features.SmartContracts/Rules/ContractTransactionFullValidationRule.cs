using System.Collections.Generic;
using System.Threading.Tasks;
using Xels.Bitcoin.Consensus.Rules;
using Xels.Bitcoin.Features.MemoryPool;
using Xels.Bitcoin.Features.MemoryPool.Interfaces;
using Xels.SmartContracts.CLR;

namespace Xels.Bitcoin.Features.SmartContracts.Rules
{
    /// <summary>
    /// Checks that smart contract transactions are in a valid format and the data is serialized correctly.
    /// </summary>
    public class ContractTransactionFullValidationRule : FullValidationConsensusRule
    {
        private readonly ContractTransactionChecker transactionChecker;

        /// <summary>The rules are kept in a covariant interface.</summary>
        private readonly IEnumerable<IContractTransactionFullValidationRule> internalRules;

        public ContractTransactionFullValidationRule(ICallDataSerializer serializer, IEnumerable<IContractTransactionFullValidationRule> internalRules)
        {
            this.transactionChecker = new ContractTransactionChecker(serializer);

            this.internalRules = internalRules;
        }

        /// <inheritdoc/>
        public override Task RunAsync(RuleContext context)
        {
            return this.transactionChecker.RunAsync(context, this.internalRules);
        }
    }
}