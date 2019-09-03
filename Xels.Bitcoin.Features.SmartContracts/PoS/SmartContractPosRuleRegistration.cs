using System.Collections.Generic;
using NBitcoin;
using NBitcoin.Rules;
using Xels.Bitcoin.Consensus.Rules;
using Xels.Bitcoin.Features.Consensus;
using Xels.Bitcoin.Features.Consensus.CoinViews;
using Xels.Bitcoin.Features.Consensus.Interfaces;
using Xels.Bitcoin.Features.Consensus.Rules.CommonRules;
using Xels.Bitcoin.Features.SmartContracts.PoS.Rules;
using Xels.SmartContracts.CLR;
using Xels.SmartContracts.Core;
using Xels.SmartContracts.Core.Receipts;
using Xels.SmartContracts.Core.State;
using Xels.SmartContracts.Core.Util;

namespace Xels.Bitcoin.Features.SmartContracts.PoS
{
    public sealed class SmartContractPosRuleRegistration : IRuleRegistration
    {
        private readonly Network network;
        private readonly IStateRepositoryRoot stateRepositoryRoot;
        private readonly IContractExecutorFactory executorFactory;
        private readonly ICallDataSerializer callDataSerializer;
        private readonly ISenderRetriever senderRetriever;
        private readonly IReceiptRepository receiptRepository;
        private readonly ICoinView coinView;
        private readonly IStakeChain stakeChain;
        private readonly IStakeValidator stakeValidator;

        public SmartContractPosRuleRegistration(Network network,
            IStateRepositoryRoot stateRepositoryRoot,
            IContractExecutorFactory executorFactory,
            ICallDataSerializer callDataSerializer,
            ISenderRetriever senderRetriever,
            IReceiptRepository receiptRepository,
            ICoinView coinView,
            IStakeChain stakeChain,
            IStakeValidator stakeValidator)
        {
            this.network = network;
            this.stateRepositoryRoot = stateRepositoryRoot;
            this.executorFactory = executorFactory;
            this.callDataSerializer = callDataSerializer;
            this.senderRetriever = senderRetriever;
            this.receiptRepository = receiptRepository;
            this.coinView = coinView;
            this.stakeChain = stakeChain;
            this.stakeValidator = stakeValidator;
        }

        public void RegisterRules(IConsensus consensus)
        {
            consensus.HeaderValidationRules = new List<IHeaderValidationConsensusRule>()
            {
                new HeaderTimeChecksRule(),
                new HeaderTimeChecksPosRule(),
                new XelsBugFixPosFutureDriftRule(),
                new CheckDifficultyPosRule(),
                new XelsHeaderVersionRule(),
            };

            consensus.IntegrityValidationRules = new List<IIntegrityValidationConsensusRule>()
            {
                new BlockMerkleRootRule(),
                new PosBlockSignatureRepresentationRule(),
                new SmartContractPosBlockSignatureRule(),
            };

            consensus.PartialValidationRules = new List<IPartialValidationConsensusRule>()
            {
                new SetActivationDeploymentsPartialValidationRule(),

                new PosTimeMaskRule(),

                // rules that are inside the method ContextualCheckBlock
                new TransactionLocktimeActivationRule(), // implements BIP113
                new CoinbaseHeightActivationRule(), // implements BIP34
                new WitnessCommitmentsRule(), // BIP141, BIP144
                new BlockSizeRule(),

                // rules that are inside the method CheckBlock
                new EnsureCoinbaseRule(),
                new CheckPowTransactionRule(),
                new CheckPosTransactionRule(),
                new CheckSigOpsRule(),
                new PosCoinstakeRule()
            };

            // TODO: When looking to make PoS work again, will need to add several of the smart contract consensus rules below (see PoA and PoW implementations)
            consensus.FullValidationRules = new List<IFullValidationConsensusRule>()
            {
                new SetActivationDeploymentsFullValidationRule(),

                new CheckDifficultyHybridRule(),
                new LoadCoinviewRule(),
                new TransactionDuplicationActivationRule(), // implements BIP30
                new SmartContractPosCoinviewRule(this.network, this.stateRepositoryRoot, this.executorFactory, this.callDataSerializer, this.senderRetriever, this.receiptRepository, this.coinView, this.stakeChain, this.stakeValidator), // implements BIP68, MaxSigOps and BlockReward 
                new SaveCoinviewRule()
            };
        }
    }
}