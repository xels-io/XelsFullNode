using System;
using System.IO;
using FluentAssertions;
using NBitcoin;
using NBitcoin.Protocol;
using Xels.Bitcoin.Builder;
using Xels.Bitcoin.Features.Api;
using Xels.Bitcoin.Features.BlockStore;
using Xels.Bitcoin.Features.Consensus;
using Xels.Bitcoin.Features.MemoryPool;
using Xels.Bitcoin.Features.Miner;
using Xels.Bitcoin.Features.RPC;
using Xels.Bitcoin.Features.Wallet;
using Xels.Bitcoin.IntegrationTests.Common;
using Xels.Bitcoin.IntegrationTests.Common.EnvironmentMockUpHelpers;
using Xels.Bitcoin.Networks;
using Xunit;

namespace Xels.Bitcoin.IntegrationTests
{
    public class CustomNodeBuilderTests
    {
        private readonly Network network;

        public CustomNodeBuilderTests()
        {
            this.network = new BitcoinRegTest();
        }

        //[Fact(Skip = "Investigate PeerConnector shutdown timeout issue")]
        //public void CanUnderstandUnknownParams()
        //{
        //    var extraParams = new NodeConfigParameters
        //    {
        //        { "some_new_unknown_param", "with a value" },
        //    };

        //    using (var nodeBuilder = NodeBuilder.Create(this))
        //    {
        //        var buildAction = new Action<IFullNodeBuilder>(builder =>
        //            builder.UseBlockStore()
        //                .UsePowConsensus()
        //                .UseMempool()
        //                .AddMining()
        //                .UseWallet()
        //                .AddRPC()
        //                .UseApi()
        //                .MockIBD());

        //        var coreNode = nodeBuilder.CreateCustomNode(buildAction, this.network,
        //            ProtocolVersion.PROTOCOL_VERSION, configParameters: extraParams);

        //        coreNode.Start();

        //        coreNode.ConfigParameters["some_new_unknown_param"].Should().Be("with a value");
        //    }
        //}

        //[Fact(Skip = "Investigate PeerConnector shutdown timeout issue")]
        //public void CanUseCustomConfigFileFromParams()
        //{
        //    var specialConf = "special.conf";

        //    var extraParams = new NodeConfigParameters
        //    {
        //        { "conf", specialConf },
        //    };

        //    using (var nodeBuilder = NodeBuilder.Create(this))
        //    {
        //        var buildAction = new Action<IFullNodeBuilder>(builder =>
        //            builder.UseBlockStore()
        //                .UsePowConsensus()
        //                .UseMempool()
        //                .AddMining()
        //                .UseWallet()
        //                .AddRPC()
        //                .UseApi()
        //                .MockIBD());

        //        var coreNode = nodeBuilder.CreateCustomNode(buildAction, this.network, ProtocolVersion.PROTOCOL_VERSION, configParameters: extraParams);

        //        coreNode.Start();

        //        coreNode.ConfigParameters["conf"].Should().Be(specialConf);
        //        File.Exists(Path.Combine(coreNode.DataFolder, specialConf)).Should().BeTrue();
        //    }
        //}
    }
}
