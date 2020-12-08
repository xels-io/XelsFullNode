using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NBitcoin;
using Xels.Bitcoin.Base.Deployments;
using Xels.Bitcoin.Consensus.Rules;
using Xels.Bitcoin.Features.Consensus.CoinViews;
using Xels.Bitcoin.Features.PoA.BasePoAFeatureConsensusRules;
using Xels.Bitcoin.Features.SmartContracts.Caching;
using Xels.Bitcoin.Features.SmartContracts.Rules;
using Xels.SmartContracts.CLR;
using Xels.SmartContracts.Core;
using Xels.SmartContracts.Core.Receipts;
using Xels.SmartContracts.Core.State;
using Xels.SmartContracts.Core.Util;

namespace Xels.Bitcoin.Features.SmartContracts.PoA.Rules
{
    public class SmartContractPoACoinviewRule : PoACoinviewRule
    {
        private SmartContractCoinViewRuleLogic logic;
        private readonly IStateRepositoryRoot stateRepositoryRoot;
        private readonly IContractExecutorFactory executorFactory;
        private readonly ICallDataSerializer callDataSerializer;
        private readonly ISenderRetriever senderRetriever;
        private readonly IReceiptRepository receiptRepository;
        private readonly ICoinView coinView;
        private readonly IBlockExecutionResultCache executionCache;
        private readonly ILoggerFactory loggerFactory;

        public SmartContractPoACoinviewRule(
            IStateRepositoryRoot stateRepositoryRoot,
            IContractExecutorFactory executorFactory,
            ICallDataSerializer callDataSerializer,
            ISenderRetriever senderRetriever,
            IReceiptRepository receiptRepository,
            ICoinView coinView,
            IBlockExecutionResultCache executionCache,
            ILoggerFactory loggerFactory)
        {
            this.stateRepositoryRoot = stateRepositoryRoot;
            this.executorFactory = executorFactory;
            this.callDataSerializer = callDataSerializer;
            this.senderRetriever = senderRetriever;
            this.receiptRepository = receiptRepository;
            this.coinView = coinView;
            this.executionCache = executionCache;
            this.loggerFactory = loggerFactory;
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            this.logic = new SmartContractCoinViewRuleLogic(this.stateRepositoryRoot, this.executorFactory, this.callDataSerializer, this.senderRetriever, this.receiptRepository, this.coinView, this.executionCache, this.loggerFactory);
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
