﻿using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using NBitcoin;
using NBitcoin.Rules;
using Xels.Bitcoin.Base;
using Xels.Bitcoin.Builder;
using Xels.Bitcoin.Configuration.Logging;
using Xels.Bitcoin.Consensus;
using Xels.Bitcoin.Consensus.Rules;
using Xels.Bitcoin.Features.Consensus.CoinViews;
using Xels.Bitcoin.Features.Consensus.Interfaces;
using Xels.Bitcoin.Features.Consensus.ProvenBlockHeaders;
using Xels.Bitcoin.Features.Consensus.Rules;
using Xels.Bitcoin.Features.Consensus.Rules.CommonRules;
using Xels.Bitcoin.Features.Consensus.Rules.ProvenHeaderRules;
using Xels.Bitcoin.Interfaces;

namespace Xels.Bitcoin.Features.Consensus
{
    /// <summary>
    /// A class providing extension methods for <see cref="IFullNodeBuilder"/>.
    /// </summary>
    public static class FullNodeBuilderConsensusExtension
    {
        public static IFullNodeBuilder UsePowConsensus(this IFullNodeBuilder fullNodeBuilder)
        {
            LoggingConfiguration.RegisterFeatureNamespace<PowConsensusFeature>("powconsensus");

            fullNodeBuilder.ConfigureFeature(features =>
            {
                features
                    .AddFeature<PowConsensusFeature>()
                    .FeatureServices(services =>
                    {
                        services.AddSingleton<ConsensusOptions, ConsensusOptions>();
                        services.AddSingleton<DBreezeCoinView>();
                        services.AddSingleton<ICoinView, CachedCoinView>();
                        services.AddSingleton<ConsensusController>();
                        services.AddSingleton<IConsensusRuleEngine, PowConsensusRuleEngine>();
                        services.AddSingleton<IChainState, ChainState>();
                        services.AddSingleton<ConsensusQuery>()
                            .AddSingleton<INetworkDifficulty, ConsensusQuery>(provider => provider.GetService<ConsensusQuery>())
                            .AddSingleton<IGetUnspentTransaction, ConsensusQuery>(provider => provider.GetService<ConsensusQuery>());
                        new PowConsensusRulesRegistration().RegisterRules(fullNodeBuilder.Network.Consensus);
                    });
            });

            return fullNodeBuilder;
        }

        public static IFullNodeBuilder UsePosConsensus(this IFullNodeBuilder fullNodeBuilder)
        {
            LoggingConfiguration.RegisterFeatureNamespace<PosConsensusFeature>("posconsensus");

            fullNodeBuilder.ConfigureFeature(features =>
            {
                features
                    .AddFeature<PosConsensusFeature>()
                    .FeatureServices(services =>
                    {
                        services.AddSingleton<DBreezeCoinView>();
                        services.AddSingleton<ICoinView, CachedCoinView>();
                        services.AddSingleton<StakeChainStore>().AddSingleton<IStakeChain, StakeChainStore>(provider => provider.GetService<StakeChainStore>());
                        services.AddSingleton<IStakeValidator, StakeValidator>();
                        services.AddSingleton<ConsensusController>();
                        services.AddSingleton<IRewindDataIndexCache, RewindDataIndexCache>();
                        services.AddSingleton<IConsensusRuleEngine, PosConsensusRuleEngine>();
                        services.AddSingleton<IChainState, ChainState>();
                        services.AddSingleton<ConsensusQuery>()
                            .AddSingleton<INetworkDifficulty, ConsensusQuery>(provider => provider.GetService<ConsensusQuery>())
                            .AddSingleton<IGetUnspentTransaction, ConsensusQuery>(provider => provider.GetService<ConsensusQuery>());
                        services.AddSingleton<IProvenBlockHeaderStore, ProvenBlockHeaderStore>();
                        services.AddSingleton<IProvenBlockHeaderRepository, ProvenBlockHeaderRepository>();
                        new PosConsensusRulesRegistration().RegisterRules(fullNodeBuilder.Network.Consensus);
                    });
            });

            return fullNodeBuilder;
        }

        public class PowConsensusRulesRegistration : IRuleRegistration
        {
            public void RegisterRules(IConsensus consensus)
            {
                consensus.HeaderValidationRules = new List<IHeaderValidationConsensusRule>()
                {
                    new HeaderTimeChecksRule(),
                    new CheckDifficultyPowRule(),
                    new BitcoinActivationRule(),
                    new BitcoinHeaderVersionRule()
                };

                consensus.IntegrityValidationRules = new List<IIntegrityValidationConsensusRule>()
                {
                    new BlockMerkleRootRule()
                };

                consensus.PartialValidationRules = new List<IPartialValidationConsensusRule>()
                {
                    new SetActivationDeploymentsPartialValidationRule(),

                    new TransactionLocktimeActivationRule(), // implements BIP113
                    new CoinbaseHeightActivationRule(), // implements BIP34
                    new WitnessCommitmentsRule(), // BIP141, BIP144
                    new BlockSizeRule(),

                    // rules that are inside the method CheckBlock
                    new EnsureCoinbaseRule(),
                    new CheckPowTransactionRule(),
                    new CheckSigOpsRule(),
                };

                consensus.FullValidationRules = new List<IFullValidationConsensusRule>()
                {
                    new SetActivationDeploymentsFullValidationRule(),

                    // rules that require the store to be loaded (coinview)
                    new LoadCoinviewRule(),
                    new TransactionDuplicationActivationRule(), // implements BIP30
                    new PowCoinviewRule(), // implements BIP68, MaxSigOps and BlockReward calculation
                    new SaveCoinviewRule()
                };
            }
        }

        public class PosConsensusRulesRegistration : IRuleRegistration
        {
            public void RegisterRules(IConsensus consensus)
            {
                consensus.HeaderValidationRules = new List<IHeaderValidationConsensusRule>()
                {
                    new HeaderTimeChecksRule(),
                    new HeaderTimeChecksPosRule(),
                    new XelsBugFixPosFutureDriftRule(),
                    new CheckDifficultyPosRule(),
                    new XelsHeaderVersionRule(),
                    new ProvenHeaderSizeRule(),
                    new ProvenHeaderCoinstakeRule()
                };

                consensus.IntegrityValidationRules = new List<IIntegrityValidationConsensusRule>()
                {
                    new BlockMerkleRootRule(),
                    new PosBlockSignatureRepresentationRule(),
                    new PosBlockSignatureRule(),
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
                    new PosCoinstakeRule(),
                };

                consensus.FullValidationRules = new List<IFullValidationConsensusRule>()
                {
                    new SetActivationDeploymentsFullValidationRule(),

                    new CheckDifficultyHybridRule(),

                    // rules that require the store to be loaded (coinview)
                    new LoadCoinviewRule(),
                    new TransactionDuplicationActivationRule(), // implements BIP30
                    new PosCoinviewRule(), // implements BIP68, MaxSigOps and BlockReward calculation
                    // Place the PosColdStakingRule after the PosCoinviewRule to ensure that all input scripts have been evaluated
                    // and that the "IsColdCoinStake" flag would have been set by the OP_CHECKCOLDSTAKEVERIFY opcode if applicable.
                    new PosColdStakingRule(),
                    new SaveCoinviewRule()
                };
            }
        }
    }
}