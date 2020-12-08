using Microsoft.Extensions.Logging;
using NBitcoin;
using Xels.Bitcoin.Features.MemoryPool;
using Xels.Bitcoin.Features.MemoryPool.Interfaces;
using Xels.Bitcoin.Features.SmartContracts.Rules;

namespace Xels.Bitcoin.Features.SmartContracts.MempoolRules
{
    /// <summary>
    /// Each transaction should have only 1 'SmartContractExec' output.
    /// </summary>
    public class TxOutSmartContractExecMempoolRule : MempoolRule
    {
        public TxOutSmartContractExecMempoolRule(Network network,
            ITxMempool mempool,
            MempoolSettings mempoolSettings,
            ChainIndexer chainIndexer,
            ILoggerFactory loggerFactory) : base(network, mempool, mempoolSettings, chainIndexer, loggerFactory)
        {
        }

        /// <inheritdoc/>
        public override void CheckTransaction(MempoolValidationContext context)
        {
            TxOutSmartContractExecRule.CheckTransaction(context.Transaction);
        }
    }
}