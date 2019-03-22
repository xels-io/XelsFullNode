using System.Linq;
using Moq;
using NBitcoin;
using Xels.Bitcoin.Base;
using Xels.Bitcoin.Configuration;
using Xels.Bitcoin.Configuration.Logging;
using Xels.Bitcoin.Consensus;
using Xels.Bitcoin.Features.Consensus.CoinViews;
using Xels.Bitcoin.Features.SmartContracts.PoW;
using Xels.Bitcoin.Features.SmartContracts.ReflectionExecutor;
using Xels.Bitcoin.Features.SmartContracts.ReflectionExecutor.Consensus.Rules;
using Xels.Bitcoin.Tests.Common;
using Xels.Bitcoin.Utilities;
using Xels.SmartContracts.Core;
using Xels.SmartContracts.Core.Receipts;
using Xels.SmartContracts.Core.State;
using Xels.SmartContracts.Core.Util;
using Xels.SmartContracts.CLR;
using Xunit;

namespace Xels.Bitcoin.Features.SmartContracts.Tests.Consensus
{
    public sealed class ReflectionRuleRegistrationTests
    {
        [Fact]
        public void ReflectionVirtualMachineFeature_OnInitialize_RulesAdded()
        {
            Network network = KnownNetworks.XelsRegTest;

            var loggerFactory = new ExtendedLoggerFactory();
            var callDataSerializer = Mock.Of<ICallDataSerializer>();

            var feature = new ReflectionVirtualMachineFeature(loggerFactory, network, callDataSerializer);
            feature.InitializeAsync().GetAwaiter().GetResult();

            Assert.Single(network.Consensus.FullValidationRules.Where(r => r.GetType() == typeof(SmartContractFormatRule)));
        }
    }
}