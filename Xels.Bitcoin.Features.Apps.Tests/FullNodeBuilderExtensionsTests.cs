using System.Linq;
using Xels.Bitcoin.Builder;
using Xels.Bitcoin.Configuration;
using Xels.Bitcoin.Features.Apps.Interfaces;
using Xels.Bitcoin.Features.Consensus;
using Xels.Bitcoin.Tests.Common;
using Xunit;

namespace Xels.Bitcoin.Features.Apps.Tests
{
    public class FullNodeBuilderExtensionsTests
    {
        private readonly IFullNodeBuilder fullNodeBuilder;

        public FullNodeBuilderExtensionsTests()
        {
            this.fullNodeBuilder = new FullNodeBuilder()
                .UseNodeSettings(new NodeSettings(KnownNetworks.TestNet))
                .UsePosConsensus();
        }

        [Fact]
        public void Test_UseApps_adds_the_AppsFeature()
        {
            this.fullNodeBuilder.UseApps().Build();
            var count = this.fullNodeBuilder.Features.FeatureRegistrations.Count(x => x.FeatureType == typeof(AppsFeature));
            Assert.Equal(1, count);
        }

        [Fact]
        public void Test_UseApps_adds_the_AppStore()
        {
            this.fullNodeBuilder.UseApps().Build();
            var count = this.fullNodeBuilder.Services.Count(x => x.ServiceType == typeof(IAppsStore));
            Assert.Equal(1, count);
        }        

        [Fact]
        public void Test_UseApps_adds_the_AppsHost()
        {
            this.fullNodeBuilder.UseApps().Build();
            var count = this.fullNodeBuilder.Services.Count(x => x.ServiceType == typeof(IAppsHost));
            Assert.Equal(1, count);
        }        

        [Fact]
        public void Test_UseApps_adds_the_AppsController()
        {
            this.fullNodeBuilder.UseApps().Build();
            var count = this.fullNodeBuilder.Services.Count(x => x.ServiceType == typeof(AppsController));
            Assert.Equal(1, count);
        }
    }
}
