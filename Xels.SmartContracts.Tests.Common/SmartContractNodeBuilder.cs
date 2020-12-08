using System.Runtime.CompilerServices;
using NBitcoin;
using Xels.Bitcoin.Configuration;
using Xels.Bitcoin.Features.PoA;
using Xels.Bitcoin.Features.PoA.IntegrationTests.Common;
using Xels.Bitcoin.IntegrationTests.Common.EnvironmentMockUpHelpers;
using Xels.Bitcoin.Tests.Common;
using Xels.SmartContracts.Networks;

namespace Xels.SmartContracts.Tests.Common
{
    public class SmartContractNodeBuilder : NodeBuilder
    {
        public EditableTimeProvider TimeProvider { get; }

        public SmartContractNodeBuilder(string rootFolder) : base(rootFolder)
        {
            this.TimeProvider = new EditableTimeProvider();
        }

        public CoreNode CreateSmartContractPoANode(SmartContractsPoARegTest network, int nodeIndex)
        {
            string dataFolder = this.GetNextDataFolderName();

            CoreNode node = this.CreateNode(new SmartContractPoARunner(dataFolder, network, this.TimeProvider), "poa.conf");

            var settings = new NodeSettings(network, args: new string[] { "-conf=poa.conf", "-datadir=" + dataFolder });

            var tool = new KeyTool(settings.DataFolder);
            tool.SavePrivateKey(network.FederationKeys[nodeIndex]);

            return node;
        }

        public CoreNode CreateWhitelistedContractPoANode(SmartContractsPoAWhitelistRegTest network, int nodeIndex)
        {
            string dataFolder = this.GetNextDataFolderName();

            CoreNode node = this.CreateNode(new WhitelistedContractPoARunner(dataFolder, network, this.TimeProvider), "poa.conf");
            var settings = new NodeSettings(network, args: new string[] { "-conf=poa.conf", "-datadir=" + dataFolder });

            var tool = new KeyTool(settings.DataFolder);
            tool.SavePrivateKey(network.FederationKeys[nodeIndex]);
            return node;
        }

        public CoreNode CreateSmartContractPowNode()
        {
            Network network = new SmartContractsRegTest();
            return CreateNode(new XelsSmartContractNode(this.GetNextDataFolderName(), network), "xels.conf");
        }

        public CoreNode CreateSmartContractPosNode()
        {
            Network network = new SmartContractPosRegTest();
            return CreateNode(new XelsSmartContractPosNode(this.GetNextDataFolderName(), network), "xels.conf");
        }

        public static SmartContractNodeBuilder Create(object caller, [CallerMemberName] string callingMethod = null)
        {
            string testFolderPath = TestBase.CreateTestDir(caller, callingMethod);
            var builder = new SmartContractNodeBuilder(testFolderPath);
            builder.WithLogsDisabled();
            return builder;
        }
    }
}
