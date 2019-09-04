using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xels.Bitcoin.Builder;
using Xels.Bitcoin.Builder.Feature;
using Xels.Bitcoin.Consensus;
using Xels.Bitcoin.Consensus.Rules;
using Xels.Bitcoin.Features.PoA;
using Xels.Features.FederatedPeg.Controllers;
using Xels.Features.FederatedPeg.CounterChain;

namespace Xels.Features.FederatedPeg.Collateral
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
        public static IFullNodeBuilder CheckForPoAMembersCollateral(this IFullNodeBuilder fullNodeBuilder)
        {
            fullNodeBuilder.ConfigureFeature(features =>
            {
                features.AddFeature<CollateralFeature>()
                    .DependOn<CounterChainFeature>()
                    .DependOn<PoAFeature>()
                    .FeatureServices(services =>
                    {
                        services.AddSingleton<IFederationManager, CollateralFederationManager>();
                        services.AddSingleton<ICollateralChecker, CollateralChecker>();
                        services.AddSingleton<CollateralVotingController>();

                        services.AddSingleton<IRuleRegistration, SmartContractCollateralPoARuleRegistration>();
                        services.AddSingleton<IConsensusRuleEngine>(f =>
                        {
                            PoAConsensusRuleEngine concreteRuleEngine = f.GetService<PoAConsensusRuleEngine>();
                            IRuleRegistration ruleRegistration = f.GetService<IRuleRegistration>();

                            return new DiConsensusRuleEngine(concreteRuleEngine, ruleRegistration);
                        });
                    });
            });

            return fullNodeBuilder;
        }
    }
}
