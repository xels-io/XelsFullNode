﻿using Microsoft.Extensions.Logging;
using NBitcoin;
using Xels.Bitcoin.Features.MemoryPool;
using Xels.Bitcoin.Features.MemoryPool.Interfaces;
using Xels.Bitcoin.Features.SmartContracts.PoA.Rules;
using Xels.Bitcoin.Features.SmartContracts.Rules;
using Xels.SmartContracts.CLR;

namespace Xels.Bitcoin.Features.SmartContracts.PoA.MempoolRules
{
    /// <summary>
    /// Validates that the hash of the supplied smart contract code is contained in a list of supplied hashes.
    /// </summary>
    /// <remarks>Should have the same logic as consensus rule <see cref="AllowedCodeHashLogic"/>.</remarks>
    public class AllowedCodeHashLogicMempoolRule : MempoolRule
    {
        private readonly ICallDataSerializer callDataSerializer;
        private readonly IWhitelistedHashChecker whitelistedHashChecker;
        private readonly IContractCodeHashingStrategy hashingStrategy;

        public AllowedCodeHashLogicMempoolRule(Network network,
            ITxMempool mempool,
            MempoolSettings mempoolSettings,
            ChainIndexer chainIndexer,
            ILoggerFactory loggerFactory,
            ICallDataSerializer callDataSerializer,
            IWhitelistedHashChecker whitelistedHashChecker,
            IContractCodeHashingStrategy hashingStrategy) : base(network, mempool, mempoolSettings, chainIndexer, loggerFactory)
        {
            this.callDataSerializer = callDataSerializer;
            this.whitelistedHashChecker = whitelistedHashChecker;
            this.hashingStrategy = hashingStrategy;
        }

        /// <inheritdoc/>
        public override void CheckTransaction(MempoolValidationContext context)
        {
            TxOut scTxOut = context.Transaction.TryGetSmartContractTxOut();

            if (scTxOut == null)
            {
                // No SC output to validate.
                return;
            }

            ContractTxData txData = ContractTransactionChecker.GetContractTxData(this.callDataSerializer, scTxOut);

            if (!txData.IsCreateContract)
                return;

            byte[] hashedCode = this.hashingStrategy.Hash(txData.ContractExecutionCode);

            if (!this.whitelistedHashChecker.CheckHashWhitelisted(hashedCode))
            {
                AllowedCodeHashLogic.ThrowInvalidCode();
            }
        }
    }
}