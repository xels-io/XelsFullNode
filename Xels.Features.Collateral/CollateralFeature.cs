using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xels.Bitcoin.Builder;
using Xels.Bitcoin.Builder.Feature;
using Xels.Bitcoin.Features.Miner;
using Xels.Bitcoin.Features.PoA;
using Xels.Bitcoin.Features.SmartContracts;
using Xels.Bitcoin.Features.SmartContracts.PoA;
using Xels.Features.Collateral.CounterChain;

namespace Xels.Features.Collateral
{
    /// <summary>
    /// Sets up the necessary components to check the collateral requirement is met on the counter chain.
    /// </summary>
    public class CollateralFeature : FullNodeFeature
    {
        private readonly ICollateralChecker collateralChecker;

        public CollateralFeature(ICollateralChecker collateralChecker)
        {
            this.collateralChecker = collateralChecker;
        }

        public override async Task InitializeAsync()
        {
            await this.collateralChecker.InitializeAsync().ConfigureAwait(false);
        }

        public override void Dispose()
        {
            this.collateralChecker?.Dispose();
        }
    }

    /// <summary>
    /// A class providing extension methods for <see cref="IFullNodeBuilder"/>.
    /// </summary>
    public static class FullNodeBuilderCollateralFeatureExtension
    {
        // Both Xoy Peg and Xoy Miner calls this.
        public static IFullNodeBuilder CheckForPoAMembersCollateral(this IFullNodeBuilder fullNodeBuilder, bool isMiner)
        {
            // This rule always executes between all Xoy nodes.
            fullNodeBuilder.Network.Consensus.ConsensusRules.FullValidationRules.Insert(0, typeof(CheckCollateralCommitmentHeightRule));

            // Only configure this if the Xoy node is a miner (XoyPegD and XoyMinerD)
            if (isMiner)
            {
                // Inject the CheckCollateralFullValidationRule as the first Full Validation Rule.
                // This is still a bit hacky and we need to properly review the dependencies again between the different side chain nodes.
                fullNodeBuilder.Network.Consensus.ConsensusRules.FullValidationRules.Insert(0, typeof(CheckCollateralFullValidationRule));

                fullNodeBuilder.ConfigureFeature(features =>
                {
                    features.AddFeature<CollateralFeature>()
                        .DependOn<CounterChainFeature>()
                        .DependOn<PoAFeature>()
                        .FeatureServices(services =>
                        {
                            services.AddSingleton<IFederationManager, CollateralFederationManager>();
                            services.AddSingleton<ICollateralChecker, CollateralChecker>();
                        });
                });
            }

            return fullNodeBuilder;
        }

        /// <summary>
        /// Adds mining to the smart contract node when on a proof-of-authority network with collateral enabled.
        /// </summary>
        public static IFullNodeBuilder UseSmartContractCollateralPoAMining(this IFullNodeBuilder fullNodeBuilder)
        {
            fullNodeBuilder.ConfigureFeature(features =>
            {
                features
                    .AddFeature<PoAFeature>()
                    .FeatureServices(services =>
                    {
                        services.AddSingleton<IFederationManager, FederationManager>();
                        services.AddSingleton<PoABlockHeaderValidator>();
                        services.AddSingleton<IPoAMiner, CollateralPoAMiner>();
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
