using System.Collections.Generic;
using NBitcoin;
using NBitcoin.Rules;
using Xels.Bitcoin.Consensus.Rules;
using Xels.Bitcoin.Features.Consensus.Rules.CommonRules;
using Xels.Bitcoin.Features.PoA.ConsensusRules;
using Xels.Bitcoin.Features.SmartContracts.PoA.Rules;
using Xels.Bitcoin.Features.SmartContracts.ReflectionExecutor.Consensus.Rules;
using Xels.Bitcoin.Features.SmartContracts.Rules;
using Xels.SmartContracts.Core.Util;
using Xels.SmartContracts.CLR;
using Xels.SmartContracts.CLR.Serialization;

namespace Xels.Bitcoin.Features.SmartContracts.PoA
{
    public class SmartContractPoARuleRegistration : IRuleRegistration
    {
        private readonly Network network;

        public SmartContractPoARuleRegistration(Network network)
        {
            this.network = network;
        }

        public void RegisterRules(IConsensus consensus)
        {
            consensus.HeaderValidationRules = new List<IHeaderValidationConsensusRule>()
            {
                new HeaderTimeChecksPoARule(),
                new XelsHeaderVersionRule(),
                new PoAHeaderDifficultyRule(),
                new PoAHeaderSignatureRule()
            };

            consensus.IntegrityValidationRules = new List<IIntegrityValidationConsensusRule>()
            {
                new BlockMerkleRootRule(),
                new PoAIntegritySignatureRule()
            };

            consensus.PartialValidationRules = new List<IPartialValidationConsensusRule>()
            {
                new SetActivationDeploymentsPartialValidationRule(),

                // rules that are inside the method ContextualCheckBlock
                new TransactionLocktimeActivationRule(), // implements BIP113
                new CoinbaseHeightActivationRule(), // implements BIP34
                new BlockSizeRule(),

                // rules that are inside the method CheckBlock
                new EnsureCoinbaseRule(),
                new CheckPowTransactionRule(),
                new CheckSigOpsRule(),
                new AllowedScriptTypeRule()
            };

            consensus.FullValidationRules = new List<IFullValidationConsensusRule>()
            {
                new SetActivationDeploymentsFullValidationRule(),

                // rules that require the store to be loaded (coinview)
                new LoadCoinviewRule(),
                new TransactionDuplicationActivationRule(), // implements BIP30
                new TxOutSmartContractExecRule(),
                new OpSpendRule(),
                new CanGetSenderRule(new SenderRetriever()),
                new SmartContractFormatRule(new CallDataSerializer(new ContractPrimitiveSerializer(this.network))), // Can we inject these serializers?
                new P2PKHNotContractRule(),
                new SmartContractPoACoinviewRule(),
                new SaveCoinviewRule()
            };
        }
    }
}
