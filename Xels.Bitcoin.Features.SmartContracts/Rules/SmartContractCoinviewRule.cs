﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NBitcoin;
using Xels.Bitcoin.Base.Deployments;
using Xels.Bitcoin.Consensus;
using Xels.Bitcoin.Consensus.Rules;
using Xels.Bitcoin.Features.Consensus.Rules.CommonRules;
using Xels.Bitcoin.Utilities;

namespace Xels.Bitcoin.Features.SmartContracts.Rules
{
    /// <inheritdoc />
    public abstract class SmartContractCoinviewRule : CoinViewRule
    {
        protected List<Transaction> blockTxsProcessed;
        protected Transaction generatedTransaction;
        protected uint refundCounter;
        private SmartContractCoinViewRuleLogic logic;

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();
            this.logic = new SmartContractCoinViewRuleLogic(this.Parent);
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

        /// <inheritdoc/>
        public override void CheckBlockReward(RuleContext context, Money fees, int height, NBitcoin.Block block)
        {
            Money blockReward = fees + this.GetProofOfWorkReward(height);
            if (block.Transactions[0].TotalOut > blockReward)
            {
                this.Logger.LogTrace("(-)[BAD_COINBASE_AMOUNT]");
                ConsensusErrors.BadCoinbaseAmount.Throw();
            }
        }

        /// <inheritdoc/>
        public override void CheckMaturity(UnspentOutputs coins, int spendHeight)
        {
            base.CheckCoinbaseMaturity(coins, spendHeight);
        }

        /// <inheritdoc />
        /// <remarks>Should someone wish to use POW only we'll need to implement subsidy halving.</remarks>
        public override Money GetProofOfWorkReward(int height)
        {
            if (height == this.Parent.Network.Consensus.PremineHeight)
                return this.Parent.Network.Consensus.PremineReward;

            return this.Parent.Network.Consensus.ProofOfWorkReward;
        }

        /// <summary>
        /// Executes contracts as necessary and updates the coinview / UTXOset after execution.
        /// </summary>
        /// <inheritdoc/>
        public override void UpdateCoinView(RuleContext context, Transaction transaction)
        {
            this.logic.UpdateCoinView(base.UpdateUTXOSet, context, transaction);
        }

        /// <summary>
        /// Validates that any condensing transaction matches the transaction generated during execution
        /// </summary>
        /// <param name="transaction"></param>
        protected void ValidateGeneratedTransaction(Transaction transaction)
        {
            this.logic.ValidateGeneratedTransaction(transaction);
        }

        /// <summary>
        /// Validates that a submitted transacction doesn't contain illegal operations
        /// </summary>
        /// <param name="transaction"></param>
        protected void ValidateSubmittedTransaction(Transaction transaction)
        {
            this.logic.ValidateSubmittedTransaction(transaction);
        }

        /// <summary>
        /// Executes the smart contract part of a transaction
        /// </summary>
        protected void ExecuteContractTransaction(RuleContext context, Transaction transaction)
        {
            this.logic.ExecuteContractTransaction(context, transaction);
        }

        /// <inheritdoc/>
        protected override bool IsProtocolTransaction(Transaction transaction)
        {
            return this.logic.IsProtocolTransaction(transaction);
        }
    }
}