using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NBitcoin;
using Xels.Bitcoin.Base;
using Xels.Bitcoin.Base.Deployments;
using Xels.Bitcoin.Configuration.Settings;
using Xels.Bitcoin.Consensus;
using Xels.Bitcoin.Consensus.Rules;
using Xels.Bitcoin.Features.Consensus.CoinViews;
using Xels.Bitcoin.Features.Consensus.Interfaces;
using Xels.Bitcoin.Features.Consensus.ProvenBlockHeaders;
using Xels.Bitcoin.Utilities;
using TracerAttributes;

namespace Xels.Bitcoin.Features.Consensus.Rules
{
    /// <summary>
    /// Extension of consensus rules that provide access to a PoS store.
    /// </summary>
    /// <remarks>
    /// A Proof-Of-Stake blockchain as implemented in this code base represents a hybrid POS/POW consensus model.
    /// </remarks>
    public class PosConsensusRuleEngine : PowConsensusRuleEngine
    {
        /// <summary>Database of stake related data for the current blockchain.</summary>
        public IStakeChain StakeChain { get; }

        /// <summary>Provides functionality for checking validity of PoS blocks.</summary>
        public IStakeValidator StakeValidator { get; }

        public IRewindDataIndexCache RewindDataIndexCache { get; }

        /// <summary>
        /// Initializes an instance of the object.
        /// </summary>
        public PosConsensusRuleEngine(Network network, ILoggerFactory loggerFactory, IDateTimeProvider dateTimeProvider, ConcurrentChain chain, NodeDeployments nodeDeployments,
            ConsensusSettings consensusSettings, ICheckpoints checkpoints, ICoinView utxoSet, IStakeChain stakeChain, IStakeValidator stakeValidator, IChainState chainState,
            IInvalidBlockHashStore invalidBlockHashStore, INodeStats nodeStats, IRewindDataIndexCache rewindDataIndexCache)
            : base(network, loggerFactory, dateTimeProvider, chain, nodeDeployments, consensusSettings, checkpoints, utxoSet, chainState, invalidBlockHashStore, nodeStats)
        {
            this.StakeChain = stakeChain;
            this.StakeValidator = stakeValidator;
            this.RewindDataIndexCache = rewindDataIndexCache;
        }

        /// <inheritdoc />
        [NoTrace]
        public override RuleContext CreateRuleContext(ValidationContext validationContext)
        {
            return new PosRuleContext(validationContext, this.DateTimeProvider.GetTimeOffset());
        }

        /// <inheritdoc />
        public override async Task InitializeAsync(ChainedHeader chainTip)
        {
            await base.InitializeAsync(chainTip).ConfigureAwait(false);

            await this.StakeChain.LoadAsync().ConfigureAwait(false);

            // A temporary hack until tip manage will be introduced.
            var breezeCoinView = (DBreezeCoinView)((CachedCoinView)this.UtxoSet).Inner;
            uint256 hash = await breezeCoinView.GetTipHashAsync().ConfigureAwait(false);
            ChainedHeader tip = chainTip.FindAncestorOrSelf(hash);

            await this.RewindDataIndexCache.InitializeAsync(tip.Height, this.UtxoSet).ConfigureAwait(false);
        }
    }
}