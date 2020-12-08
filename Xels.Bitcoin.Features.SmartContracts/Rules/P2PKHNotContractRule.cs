using System.Threading.Tasks;
using NBitcoin;
using Xels.Bitcoin.Consensus;
using Xels.Bitcoin.Consensus.Rules;
using Xels.Bitcoin.Features.MemoryPool;
using Xels.SmartContracts.Core.State;

namespace Xels.Bitcoin.Features.SmartContracts.Rules
{
    /// <summary>
    /// Used to check that people don't try and send funds to contracts via P2PKH.
    /// </summary>
    public class P2PKHNotContractRule : FullValidationConsensusRule
    {
        private readonly IStateRepositoryRoot stateRepositoryRoot;

        public P2PKHNotContractRule(IStateRepositoryRoot stateRepositoryRoot)
        {
            this.stateRepositoryRoot = stateRepositoryRoot;
        }

        /// <inheritdoc/>
        public override Task RunAsync(RuleContext context)
        {
            Block block = context.ValidationContext.BlockToValidate;

            foreach (Transaction transaction in block.Transactions)
            {
                CheckTransaction(this.stateRepositoryRoot, transaction);
            }

            return Task.CompletedTask;
        }

        public static void CheckTransaction(IStateRepositoryRoot stateRepositoryRoot, Transaction transaction)
        {
            foreach(TxOut output in transaction.Outputs)
            {
                if (PayToPubkeyHashTemplate.Instance.CheckScriptPubKey(output.ScriptPubKey))
                {
                    KeyId p2pkhParams = PayToPubkeyHashTemplate.Instance.ExtractScriptPubKeyParameters(output.ScriptPubKey);
                    var to = new uint160(p2pkhParams.ToBytes());
                    if (stateRepositoryRoot.GetAccountState(to) != null)
                        new ConsensusError("p2pkh-to-contract", "attempted send directly to contract address. use OP_CALL instead.").Throw();
                }
            }
        }
    }
}
