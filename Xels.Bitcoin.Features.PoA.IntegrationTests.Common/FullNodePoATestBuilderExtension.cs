using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xels.Bitcoin.Builder;
using Xels.Bitcoin.Builder.Feature;

namespace Xels.Bitcoin.Features.PoA.IntegrationTests.Common
{
    public static class FullNodePoATestBuilderExtension
    {
        public static IFullNodeBuilder AddFastMiningCapability(this IFullNodeBuilder fullNodeBuilder)
        {
            fullNodeBuilder.ConfigureFeature(features =>
            {
                foreach (IFeatureRegistration feature in features.FeatureRegistrations)
                {
                    feature.FeatureServices(services =>
                    {
                        services.Replace(new ServiceDescriptor(typeof(IPoAMiner), typeof(TestPoAMiner), ServiceLifetime.Singleton));
                    });
                }
            });

            return fullNodeBuilder;
        }
    }
}
