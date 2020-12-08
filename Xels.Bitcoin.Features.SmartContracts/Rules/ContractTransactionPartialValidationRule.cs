using System.Collections.Generic;
using System.Threading.Tasks;
using Xels.Bitcoin.Consensus.Rules;
using Xels.SmartContracts.CLR;

namespace Xels.Bitcoin.Features.SmartContracts.Rules
{
    /// <summary>
    /// Checks that smart contract transactions are in a valid format and the data is serialized correctly.
    /// </summary>
    public class ContractTransactionPartialValidationRule : PartialValidationConsensusRule
    {
        private readonly ContractTransactionChecker transactionChecker;

        // Keep the rules in a covariant interface.
        private readonly IEnumerable<IContractTransactionValidationRule> internalRules;

        public ContractTransactionPartialValidationRule(ICallDataSerializer serializer, IEnumerable<IContractTransactionPartialValidationRule> internalRules)
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
