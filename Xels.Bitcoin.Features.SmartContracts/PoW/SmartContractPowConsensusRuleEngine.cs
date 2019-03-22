using Microsoft.Extensions.Logging;
using NBitcoin;
using Xels.Bitcoin.Base;
using Xels.Bitcoin.Base.Deployments;
using Xels.Bitcoin.Configuration.Settings;
using Xels.Bitcoin.Consensus;
using Xels.Bitcoin.Consensus.Rules;
using Xels.Bitcoin.Features.Consensus;
using Xels.Bitcoin.Features.Consensus.CoinViews;
using Xels.Bitcoin.Features.Consensus.Rules;
using Xels.Bitcoin.Utilities;
using Xels.SmartContracts.Core;
using Xels.SmartContracts.Core.Receipts;
using Xels.SmartContracts.Core.State;
using Xels.SmartContracts.Core.Util;
using Xels.SmartContracts.CLR;

namespace Xels.Bitcoin.Features.SmartContracts.PoW
{
    /// <summary>
    /// Extension of consensus rules that provide access to a store based on UTXO (Unspent transaction outputs).
    /// </summary>
    public sealed class SmartContractPowConsensusRuleEngine : PowConsensusRuleEngine, ISmartContractCoinviewRule
    {
        public ICallDataSerializer CallDataSerializer { get; private set; }
        public IContractExecutorFactory ExecutorFactory { get; private set; }
        public IStateRepositoryRoot OriginalStateRoot { get; private set; }
        public IReceiptRepository ReceiptRepository { get; private set; }
        public ISenderRetriever SenderRetriever { get; private set; }

        public SmartContractPowConsensusRuleEngine(
            ICallDataSerializer callDataSerializer,
            ConcurrentChain chain,
            ICheckpoints checkpoints,
            ConsensusSettings consensusSettings,
            IDateTimeProvider dateTimeProvider,
            IContractExecutorFactory executorFactory,
            ILoggerFactory loggerFactory,
            Network network,
            NodeDeployments nodeDeployments,
            IStateRepositoryRoot originalStateRoot,
            IReceiptRepository receiptRepository,
            ISenderRetriever senderRetriever,
            ICoinView utxoSet,
            IChainState chainState,
            IInvalidBlockHashStore invalidBlockHashStore,
            INodeStats nodeStats)
            : base(network, loggerFactory, dateTimeProvider, chain, nodeDeployments, consensusSettings, checkpoints, utxoSet, chainState, invalidBlockHashStore, nodeStats)
        {
            this.CallDataSerializer = callDataSerializer;
            this.ExecutorFactory = executorFactory;
            this.OriginalStateRoot = originalStateRoot;
            this.ReceiptRepository = receiptRepository;
            this.SenderRetriever = senderRetriever;
        }

        /// <inheritdoc />
        public override RuleContext CreateRuleContext(ValidationContext validationContext)
        {
            return new PowRuleContext(validationContext, this.DateTimeProvider.GetTimeOffset());
        }
    }
}