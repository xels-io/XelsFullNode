﻿using Microsoft.Extensions.Logging;
using NBitcoin;
using Xels.Bitcoin.Features.Consensus;
using Xels.Bitcoin.Features.Consensus.CoinViews;
using Xels.Bitcoin.Features.SmartContracts.Caching;
using Xels.Bitcoin.Features.SmartContracts.Rules;
using Xels.SmartContracts.CLR;
using Xels.SmartContracts.Core;
using Xels.SmartContracts.Core.Receipts;
using Xels.SmartContracts.Core.State;
using Xels.SmartContracts.Core.Util;

namespace Xels.Bitcoin.Features.SmartContracts.PoW.Rules
{
    public sealed class SmartContractPowCoinviewRule : SmartContractCoinviewRule
    {
        public SmartContractPowCoinviewRule(Network network, IStateRepositoryRoot stateRepositoryRoot,
            IContractExecutorFactory executorFactory, ICallDataSerializer callDataSerializer,
            ISenderRetriever senderRetriever, IReceiptRepository receiptRepository, ICoinView coinView, IBlockExecutionResultCache executionCache, ILoggerFactory loggerFactory) 
            : base(network, stateRepositoryRoot, executorFactory, callDataSerializer, senderRetriever, receiptRepository, coinView, executionCache, loggerFactory)
        {
        }

        /// <inheritdoc />
        protected override Money GetTransactionFee(UnspentOutputSet view, Transaction tx)
        {
            return view.GetValueIn(tx) - tx.TotalOut;
        }
    }
}