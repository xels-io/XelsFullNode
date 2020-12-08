using Xels.Bitcoin.Consensus.Rules;
using Xels.Bitcoin.Features.Consensus.CoinViews;
using Xels.Bitcoin.Utilities;

namespace Xels.Bitcoin.Features.Consensus.Rules
{
    /// <summary>
    /// Rules that provide easy access to the <see cref="CoinView"/> which is the store for a PoW system.
    /// </summary>
    public abstract class UtxoStoreConsensusRule : FullValidationConsensusRule
    {
        /// <summary>Allow access to the POS parent.</summary>
        protected PowConsensusRuleEngine PowParent;

        protected CoinviewHelper coinviewHelper;

        /// <inheritdoc />
        public override void Initialize()
        {
            this.PowParent = this.Parent as PowConsensusRuleEngine;
            Guard.NotNull(this.PowParent, nameof(this.PowParent));

            this.coinviewHelper = new CoinviewHelper();
        }
    }
}