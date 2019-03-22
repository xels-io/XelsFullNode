using System.Threading.Tasks;
using NBitcoin;
using Xels.Bitcoin.Base.Deployments;
using Xels.Bitcoin.Consensus.Rules;
using Xels.Bitcoin.Features.PoA.ConsensusRules;
using Xels.Bitcoin.Features.SmartContracts.Rules;

namespace Xels.Bitcoin.Features.SmartContracts.PoA.Rules
{
    public class SmartContractPoACoinviewRule : PoACoinviewRule
    {
        private SmartContractCoinViewRuleLogic logic;

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            this.logic = new SmartContractCoinViewRuleLogic(this.Parent);
        }

        /// <inheritdoc />
        public override async Task RunAsync(RuleContext context)
        {
            await this.logic.RunAsync(base.RunAsync, context);
        }

        /// <inheritdoc/>
        protected override bool CheckInput(Transaction tx, int inputIndexCopy, TxOut txout, PrecomputedTransactionData txData, TxIn input, DeploymentFlags flags)
        {
            return this.logic.CheckInput(base.CheckInput, tx, inputIndexCopy, txout, txData, input, flags);
        }

        /// <summary>
        /// Executes contracts as necessary and updates the coinview / UTXOset after execution.
        /// </summary>
        /// <inheritdoc/>
        public override void UpdateCoinView(RuleContext context, Transaction transaction)
        {
            this.logic.UpdateCoinView(base.UpdateUTXOSet, context, transaction);
        }
    }
}
