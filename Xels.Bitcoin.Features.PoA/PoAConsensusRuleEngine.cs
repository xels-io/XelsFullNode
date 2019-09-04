using Microsoft.Extensions.Logging;
using NBitcoin;
using Xels.Bitcoin.AsyncWork;
using Xels.Bitcoin.Base;
using Xels.Bitcoin.Base.Deployments;
using Xels.Bitcoin.Configuration.Settings;
using Xels.Bitcoin.Consensus;
using Xels.Bitcoin.Features.Consensus.CoinViews;
using Xels.Bitcoin.Features.Consensus.Rules;
using Xels.Bitcoin.Features.PoA.Voting;
using Xels.Bitcoin.Utilities;

namespace Xels.Bitcoin.Features.PoA
{
    /// <inheritdoc />
    public class PoAConsensusRuleEngine : PowConsensusRuleEngine
    {
        public ISlotsManager SlotsManager { get; private set; }

        public PoABlockHeaderValidator PoaHeaderValidator { get; private set; }

        public VotingManager VotingManager { get; private set; }

        public IFederationManager FederationManager { get; private set; }

        public PoAConsensusRuleEngine(Network network, ILoggerFactory loggerFactory, IDateTimeProvider dateTimeProvider, ChainIndexer chainIndexer,
            NodeDeployments nodeDeployments, ConsensusSettings consensusSettings, ICheckpoints checkpoints, ICoinView utxoSet, IChainState chainState,
            IInvalidBlockHashStore invalidBlockHashStore, INodeStats nodeStats, ISlotsManager slotsManager, PoABlockHeaderValidator poaHeaderValidator,
            VotingManager votingManager, IFederationManager federationManager, IAsyncProvider asyncProvider)
            : base(network, loggerFactory, dateTimeProvider, chainIndexer, nodeDeployments, consensusSettings, checkpoints, utxoSet, chainState, invalidBlockHashStore, nodeStats, asyncProvider)
        {
            this.SlotsManager = slotsManager;
            this.PoaHeaderValidator = poaHeaderValidator;
            this.VotingManager = votingManager;
            this.FederationManager = federationManager;
        }
    }
}
