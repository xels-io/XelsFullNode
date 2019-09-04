using System.Collections.Generic;
using NBitcoin;
using Xels.Bitcoin.Features.Consensus.CoinViews;
using Xels.Bitcoin.Features.Consensus.Rules.CommonRules;
using Xels.Bitcoin.Features.PoA;
using Xels.Bitcoin.Features.SmartContracts.PoA;
using Xels.Bitcoin.Features.SmartContracts.Rules;
using Xels.Bitcoin.Interfaces;
using Xels.Bitcoin.Utilities;
using Xels.SmartContracts.CLR;
using Xels.SmartContracts.Core;
using Xels.SmartContracts.Core.Receipts;
using Xels.SmartContracts.Core.State;
using Xels.SmartContracts.Core.Util;

namespace Xels.Features.FederatedPeg.Collateral
{
    public class SmartContractCollateralPoARuleRegistration : SmartContractPoARuleRegistration
    {
        private readonly IInitialBlockDownloadState ibdState;

        private readonly ISlotsManager slotsManager;

        private readonly ICollateralChecker collateralChecker;

        private readonly IDateTimeProvider dateTime;

        public SmartContractCollateralPoARuleRegistration(Network network, IStateRepositoryRoot stateRepositoryRoot, IContractExecutorFactory executorFactory,
            ICallDataSerializer callDataSerializer, ISenderRetriever senderRetriever, IReceiptRepository receiptRepository, ICoinView coinView,
            IEnumerable<IContractTransactionPartialValidationRule> partialTxValidationRules, IEnumerable<IContractTransactionFullValidationRule> fullTxValidationRules,
            IInitialBlockDownloadState ibdState, ISlotsManager slotsManager, ICollateralChecker collateralChecker, IDateTimeProvider dateTime)
        : base(network, stateRepositoryRoot, executorFactory, callDataSerializer, senderRetriever, receiptRepository, coinView, partialTxValidationRules, fullTxValidationRules)
        {
            this.ibdState = ibdState;
            this.slotsManager = slotsManager;
            this.collateralChecker = collateralChecker;
            this.dateTime = dateTime;
        }

        public override void RegisterRules(IConsensus consensus)
        {
            base.RegisterRules(consensus);

            // SaveCoinviewRule must be the last rule executed because actually it calls CachedCoinView.SaveChanges that causes internal CachedCoinView to be updated
            // see https://dev.azure.com/Xelsplatformuk/XelsBitcoinFullNode/_workitems/edit/3770
            // TODO: re-design how rules gets called, which order they have and prevent a rule to change internal service statuses (rules should just check)
            int saveCoinviewRulePosition = consensus.FullValidationRules.FindIndex(c => c is SaveCoinviewRule);
            consensus.FullValidationRules.Insert(saveCoinviewRulePosition, new CheckCollateralFullValidationRule(this.ibdState, this.collateralChecker, this.slotsManager, this.dateTime, this.network));
        }
    }
}
