using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NBitcoin;
using Xels.Bitcoin.Builder;
using Xels.Bitcoin.Builder.Feature;
using Xels.Bitcoin.Configuration.Logging;
using Xels.Bitcoin.Features.BlockStore;

namespace Xels.Bitcoin.Features.BlockExplorer
{
    public class BlockExplorerFeature : FullNodeFeature
    {
        private ConcurrentChain chain;

        public BlockExplorerFeature(ConcurrentChain chain)
        {
            this.chain = chain;
        }

        public override async Task InitializeAsync()
        {
            //throw new NotImplementedException();
            return;
        }
    }

    /// <summary>
    /// A class providing extension methods for <see cref="IFullNodeBuilder"/>.
    /// </summary>
    public static class FullNodeBuilderBlockExplorerExtension
    {
        public static IFullNodeBuilder UseBlockExplorer(this IFullNodeBuilder fullNodeBuilder)
        {
            LoggingConfiguration.RegisterFeatureNamespace<BlockExplorerFeature>("BlockExplorer");

            fullNodeBuilder.ConfigureFeature(features =>
            {
                features
                .AddFeature<BlockExplorerFeature>()
                .FeatureServices(services =>
                {
                    services.AddSingleton<BlockExplorerController>();
                });
            });

            return fullNodeBuilder;
        }
    }
}
