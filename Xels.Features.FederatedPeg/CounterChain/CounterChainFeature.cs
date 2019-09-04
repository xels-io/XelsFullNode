using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NBitcoin;
using Xels.Bitcoin.Builder;
using Xels.Bitcoin.Builder.Feature;
using Xels.Features.FederatedPeg.Interfaces;

namespace Xels.Features.FederatedPeg.CounterChain
{
    /// <summary>
    /// A pre-requisite for other sidechains-related features.
    /// </summary>
    public class CounterChainFeature : FullNodeFeature
    {
        public override async Task InitializeAsync()
        {
            // Don't need to initialise anything, it's just settings injected at the moment.
        }
    }

    /// <summary>
    /// A class providing extension methods for <see cref="IFullNodeBuilder"/>.
    /// </summary>
    public static class FullNodeBuilderCounterChainFeatureExtension
    {
        public static IFullNodeBuilder SetCounterChainNetwork(this IFullNodeBuilder fullNodeBuilder,
            Network counterChainNetwork)
        {
            fullNodeBuilder.Features.AddFeature<CounterChainFeature>().FeatureServices(services =>
            {
                // Inject the counter chain network with a wrapper.
                services.AddSingleton(new CounterChainNetworkWrapper(counterChainNetwork));

                // Inject the actual counter chain settings which consume the above wrapper.
                services.AddSingleton<ICounterChainSettings, CounterChainSettings>();

                // We're also going to need a http client if we're calling another node.
                services.AddSingleton<IHttpClientFactory, Bitcoin.Controllers.HttpClientFactory>();
            });

            return fullNodeBuilder;
        }
    }
}
