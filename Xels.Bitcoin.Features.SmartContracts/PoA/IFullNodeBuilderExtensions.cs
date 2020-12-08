using Microsoft.Extensions.DependencyInjection;
using Xels.Bitcoin.Builder;
using Xels.Bitcoin.Configuration.Logging;
using Xels.Bitcoin.Consensus;
using Xels.Bitcoin.Features.Consensus;
using Xels.Bitcoin.Features.Consensus.CoinViews;
using Xels.Bitcoin.Features.Miner;
using Xels.Bitcoin.Features.PoA;
using Xels.Bitcoin.Features.PoA.Voting;
using Xels.Bitcoin.Features.SmartContracts.ReflectionExecutor.Consensus.Rules;
using Xels.Bitcoin.Features.SmartContracts.Rules;

namespace Xels.Bitcoin.Features.SmartContracts.PoA
{
    public static partial class IFullNodeBuilderExtensions
    {
        /// <summary>
        /// Configures the node with the smart contract proof of authority consensus model.
        /// </summary>
        public static IFullNodeBuilder UseSmartContractPoAConsensus(this IFullNodeBuilder fullNodeBuilder)
        {
            LoggingConfiguration.RegisterFeatureNamespace<ConsensusFeature>("consensus");

            fullNodeBuilder.ConfigureFeature(features =>
            {
                features
                    .AddFeature<ConsensusFeature>()
                    .DependOn<SmartContractFeature>()
                    .FeatureServices(services =>
                    {
                        services.AddSingleton<DBreezeCoinView>();
                        services.AddSingleton<ICoinView, CachedCoinView>();
                        services.AddSingleton<VotingManager>();
                        services.AddSingleton<IWhitelistedHashesRepository, WhitelistedHashesRepository>();
                        services.AddSingleton<IPollResultExecutor, PollResultExecutor>();

                        services.AddSingleton(typeof(IContractTransactionPartialValidationRule), typeof(SmartContractFormatLogic));
                        services.AddSingleton<IConsensusRuleEngine, PoAConsensusRuleEngine>();

                        // Voting.
                        services.AddSingleton<VotingManager>();
                        services.AddSingleton<IPollResultExecutor, PollResultExecutor>();
                        services.AddSingleton<IWhitelistedHashesRepository, WhitelistedHashesRepository>();
                        services.AddSingleton<IdleFederationMembersKicker>();
                    });
            });

            return fullNodeBuilder;
        }

        /// <summary>
        /// Adds mining to the smart contract node when on a proof-of-authority network.
        /// </summary>
        public static IFullNodeBuilder UseSmartContractPoAMining(this IFullNodeBuilder fullNodeBuilder)
        {
            fullNodeBuilder.ConfigureFeature(features =>
            {
                features
                    .AddFeature<PoAFeature>()
                    .FeatureServices(services =>
                    {
                        services.AddSingleton<IFederationManager, FederationManager>();
                        services.AddSingleton<PoABlockHeaderValidator>();
                        services.AddSingleton<IPoAMiner, PoAMiner>();
                        services.AddSingleton<PoAMinerSettings>();
                        services.AddSingleton<MinerSettings>();
                        services.AddSingleton<ISlotsManager, SlotsManager>();
                        services.AddSingleton<BlockDefinition, SmartContractPoABlockDefinition>();
                        services.AddSingleton<IBlockBufferGenerator, BlockBufferGenerator>();
                    });
            });

            return fullNodeBuilder;
        }
    }
}
