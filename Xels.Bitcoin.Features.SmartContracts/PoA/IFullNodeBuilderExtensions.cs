using Microsoft.Extensions.DependencyInjection;
using Xels.Bitcoin.Builder;
using Xels.Bitcoin.Configuration.Logging;
using Xels.Bitcoin.Consensus;
using Xels.Bitcoin.Features.Consensus;
using Xels.Bitcoin.Features.Consensus.CoinViews;
using Xels.Bitcoin.Features.Miner;
using Xels.Bitcoin.Features.PoA;

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
                        services.AddSingleton<ConsensusController>();
                        services.AddSingleton<IConsensusRuleEngine, SmartContractPoARuleEngine>();

                        new SmartContractPoARuleRegistration(fullNodeBuilder.Network).RegisterRules(fullNodeBuilder.Network.Consensus);
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
                        services.AddSingleton<FederationManager>();
                        services.AddSingleton<PoABlockHeaderValidator>();
                        services.AddSingleton<IPoAMiner, PoAMiner>();
                        services.AddSingleton<SlotsManager>();
                        services.AddSingleton<BlockDefinition, SmartContractPoABlockDefinition>();
                        services.AddSingleton<IBlockBufferGenerator, BlockBufferGenerator>();
                    });
            });

            return fullNodeBuilder;
        }
    }
}
