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

        [Retry(1)]
        public void CanOverrideOnlyApiPort()
        {
            var extraParams = new NodeConfigParameters { { "apiport", "12345" } };

            using (var nodeBuilder = NodeBuilder.Create(this))
            {
                var buildAction = new Action<IFullNodeBuilder>(builder =>
                    builder.UseBlockStore()
                        .UsePowConsensus()
                        .UseMempool()
                        .AddMining()
                        .UseWallet()
                        .AddRPC()
                        .UseApi()
                        .MockIBD());

                var coreNode = nodeBuilder.CreateCustomNode(buildAction, this.network,
                    ProtocolVersion.PROVEN_HEADER_VERSION, configParameters: extraParams);

                coreNode.Start();

                coreNode.ApiPort.Should().Be(12345);
                coreNode.FullNode.NodeService<ApiSettings>().ApiPort.Should().Be(12345);

                coreNode.RpcPort.Should().NotBe(0);
                coreNode.FullNode.NodeService<RpcSettings>().RPCPort.Should().NotBe(0);

                coreNode.ProtocolPort.Should().NotBe(0);
                coreNode.FullNode.ConnectionManager.ConnectionSettings.ExternalEndpoint.Port.Should().NotBe(0);
            }
        }

        [Retry(1)]
        public void CanOverrideAllPorts()
        {
            var extraParams = new NodeConfigParameters
            {
                { "port", "123" },
                { "rpcport", "456" },
                { "apiport", "567" }
            };

            using (var nodeBuilder = NodeBuilder.Create(this))
            {
                var buildAction = new Action<IFullNodeBuilder>(builder =>
                    builder.UseBlockStore()
                        .UsePowConsensus()
                        .UseMempool()
                        .AddMining()
                        .UseWallet()
                        .AddRPC()
                        .UseApi()
                        .MockIBD());

                var coreNode = nodeBuilder.CreateCustomNode(buildAction, this.network,
                    ProtocolVersion.PROVEN_HEADER_VERSION, configParameters: extraParams);

                coreNode.Start();

                coreNode.ApiPort.Should().Be(567);
                coreNode.FullNode.NodeService<ApiSettings>().ApiPort.Should().Be(567);

                coreNode.RpcPort.Should().Be(456);
                coreNode.FullNode.NodeService<RpcSettings>().RPCPort.Should().Be(456);

                coreNode.ProtocolPort.Should().Be(123);
                coreNode.FullNode.ConnectionManager.ConnectionSettings.ExternalEndpoint.Port.Should().Be(123);
            }
        }

        [Retry(1)]
        public void CanUnderstandUnknownParams()
        {
            var extraParams = new NodeConfigParameters
            {
                { "some_new_unknown_param", "with a value" },
            };

            using (var nodeBuilder = NodeBuilder.Create(this))
            {
                var buildAction = new Action<IFullNodeBuilder>(builder =>
                    builder.UseBlockStore()
                        .UsePowConsensus()
                        .UseMempool()
                        .AddMining()
                        .UseWallet()
                        .AddRPC()
                        .UseApi()
                        .MockIBD());

                var coreNode = nodeBuilder.CreateCustomNode(buildAction, this.network,
                    ProtocolVersion.PROTOCOL_VERSION, configParameters: extraParams);

                coreNode.Start();

                coreNode.ConfigParameters["some_new_unknown_param"].Should().Be("with a value");
            }
        }

        [Retry(1)]
        public void CanUseCustomConfigFileFromParams()
        {
            var specialConf = "special.conf";

            var extraParams = new NodeConfigParameters
            {
                { "conf", specialConf },
            };

            using (var nodeBuilder = NodeBuilder.Create(this))
            {
                var buildAction = new Action<IFullNodeBuilder>(builder =>
                    builder.UseBlockStore()
                        .UsePowConsensus()
                        .UseMempool()
                        .AddMining()
                        .UseWallet()
                        .AddRPC()
                        .UseApi()
                        .MockIBD());

                var coreNode = nodeBuilder.CreateCustomNode(buildAction, this.network,
                    ProtocolVersion.PROTOCOL_VERSION, configParameters: extraParams);

                coreNode.Start();

                coreNode.ConfigParameters["conf"].Should().Be(specialConf);
                File.Exists(Path.Combine(coreNode.DataFolder, specialConf)).Should().BeTrue();
            }
        }
    }
}
