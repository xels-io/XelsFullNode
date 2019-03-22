using Microsoft.Extensions.Logging;
using NBitcoin;
using Xels.Bitcoin.Base;
using Xels.Bitcoin.Base.Deployments;
using Xels.Bitcoin.Configuration.Settings;
using Xels.Bitcoin.Consensus;
using Xels.Bitcoin.Features.Consensus.CoinViews;
using Xels.Bitcoin.Features.Consensus.Rules;
using Xels.Bitcoin.Utilities;

namespace Xels.Bitcoin.Features.PoA
{
    /// <inheritdoc />
    public class PoAConsensusRuleEngine : PowConsensusRuleEngine
    {
        public SlotsManager SlotsManager { get; private set; }

        public PoABlockHeaderValidator poaHeaderValidator { get; private set; }

        public PoAConsensusRuleEngine(Network network, ILoggerFactory loggerFactory, IDateTimeProvider dateTimeProvider, ConcurrentChain chain,
            NodeDeployments nodeDeployments, ConsensusSettings consensusSettings, ICheckpoints checkpoints, ICoinView utxoSet, IChainState chainState,
            IInvalidBlockHashStore invalidBlockHashStore, INodeStats nodeStats, SlotsManager slotsManager, PoABlockHeaderValidator poaHeaderValidator)
            : base(network, loggerFactory, dateTimeProvider, chain, nodeDeployments, consensusSettings, checkpoints, utxoSet, chainState, invalidBlockHashStore, nodeStats)
        {
            this.SlotsManager = slotsManager;
            this.poaHeaderValidator = poaHeaderValidator;
        }
    }
}
