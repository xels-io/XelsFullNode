using System.Collections.Generic;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using NBitcoin;
using Xels.Bitcoin.Configuration;
using Xels.Bitcoin.Consensus;
using Xels.Bitcoin.Features.Consensus.CoinViews;
using Xels.Bitcoin.Features.MemoryPool;
using Xels.Bitcoin.Features.MemoryPool.Interfaces;
using Xels.Bitcoin.Features.SmartContracts.ReflectionExecutor.Consensus.Rules;
using Xels.Bitcoin.Features.SmartContracts.Rules;
using Xels.Bitcoin.Utilities;
using Xels.SmartContracts.CLR;
using Xels.SmartContracts.Core.State;

namespace Xels.Bitcoin.Features.SmartContracts
{
    /// <summary>
    /// Provides the same functionality as the original mempool validator with some extra validation.
    /// </summary>
    public class SmartContractMempoolValidator : MempoolValidator
    {
        /// <summary>The "functional" minimum gas limit. Not enforced by consensus but miners are only going to pick transactions up if their gas price is higher than this.</summary>
        public const ulong MinGasPrice = 100;

        /// <summary>
        /// These rules can be checked instantly. They don't rely on other parts of the context to be loaded.
        /// </summary>
        private readonly List<ISmartContractMempoolRule> preTxRules;

        /// <summary>
        /// These rules rely on the fee part of the context to be loaded in parent class. See 'AcceptToMemoryPoolWorkerAsync'.
        /// </summary>
        private readonly List<ISmartContractMempoolRule> feeTxRules;
        private readonly ICallDataSerializer callDataSerializer;
        private readonly IStateRepositoryRoot stateRepositoryRoot;

        public SmartContractMempoolValidator(ITxMempool memPool, MempoolSchedulerLock mempoolLock,
            IDateTimeProvider dateTimeProvider, MempoolSettings mempoolSettings, ChainIndexer chainIndexer,
            ICoinView coinView, ILoggerFactory loggerFactory, NodeSettings nodeSettings,
            IConsensusRuleEngine consensusRules, ICallDataSerializer callDataSerializer, Network network,
            IStateRepositoryRoot stateRepositoryRoot,
            IEnumerable<IContractTransactionFullValidationRule> txFullValidationRules)
            : base(memPool, mempoolLock, dateTimeProvider, mempoolSettings, chainIndexer, coinView, loggerFactory, nodeSettings, consensusRules)
        {
            // Dirty hack, but due to AllowedScriptTypeRule we don't need to check for standard scripts on any network, even live.
            // TODO: Remove ASAP. Ensure RequireStandard isn't used on SC mainnets, or the StandardScripts check is modular.
            mempoolSettings.RequireStandard = false;

            this.callDataSerializer = callDataSerializer;
            this.stateRepositoryRoot = stateRepositoryRoot;

            var p2pkhRule = new P2PKHNotContractRule(stateRepositoryRoot);

            var scriptTypeRule = new AllowedScriptTypeRule(network);
            scriptTypeRule.Initialize();

            this.preTxRules = new List<ISmartContractMempoolRule>
            {
                new MempoolOpSpendRule(),
                new TxOutSmartContractExecRule(),
                scriptTypeRule,
                p2pkhRule
            };

            var txChecks = new List<IContractTransactionPartialValidationRule>()
            {
                new SmartContractFormatLogic()
            };
            
            this.feeTxRules = new List<ISmartContractMempoolRule>()
            {
                new ContractTransactionPartialValidationRule(this.callDataSerializer, txChecks),
                new ContractTransactionFullValidationRule(this.callDataSerializer, txFullValidationRules)
            };
        }

        /// <inheritdoc />
        protected override void PreMempoolChecks(MempoolValidationContext context)
        {
            base.PreMempoolChecks(context);

            foreach (ISmartContractMempoolRule rule in this.preTxRules)
            {
                rule.CheckTransaction(context);
            }
        }

        /// <inheritdoc />
        public override void CheckFee(MempoolValidationContext context)
        {
            base.CheckFee(context);

            foreach (ISmartContractMempoolRule rule in this.feeTxRules)
            {
                rule.CheckTransaction(context);
            }

            this.CheckMinGasLimit(context);
        }

        private void CheckMinGasLimit(MempoolValidationContext context)
        {
            Transaction transaction = context.Transaction;

            if (!transaction.IsSmartContractExecTransaction())
                return;

            // We know it has passed SmartContractFormatRule so we can deserialize it easily.
            TxOut scTxOut = transaction.TryGetSmartContractTxOut();
            Result<ContractTxData> callDataDeserializationResult = this.callDataSerializer.Deserialize(scTxOut.ScriptPubKey.ToBytes());
            ContractTxData callData = callDataDeserializationResult.Value;
            if (callData.GasPrice < MinGasPrice)
                context.State.Fail(MempoolErrors.InsufficientFee, $"Gas price {callData.GasPrice} is below required price: {MinGasPrice}").Throw();
        }
    }
}