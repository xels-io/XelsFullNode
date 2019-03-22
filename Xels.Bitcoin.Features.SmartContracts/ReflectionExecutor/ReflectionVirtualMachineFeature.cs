using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NBitcoin;
using NBitcoin.Rules;
using Xels.Bitcoin.Builder.Feature;
using Xels.Bitcoin.Features.SmartContracts.ReflectionExecutor.Consensus.Rules;
using Xels.SmartContracts.CLR;

namespace Xels.Bitcoin.Features.SmartContracts.ReflectionExecutor
{
    public sealed class ReflectionVirtualMachineFeature : FullNodeFeature
    {
        private readonly ILogger logger;
        private readonly Network network;
        private readonly ICallDataSerializer callDataSerializer;

        public ReflectionVirtualMachineFeature(ILoggerFactory loggerFactory, Network network, ICallDataSerializer callDataSerializer)
        {
            this.network = network;
            this.callDataSerializer = callDataSerializer;
            this.logger = loggerFactory.CreateLogger(this.GetType().FullName);
        }

        public override Task InitializeAsync()
        {
            this.RegisterRules(this.network.Consensus);

            this.logger.LogInformation("Reflection Virtual Machine Injected.");
            
            return Task.CompletedTask;
        }

        private void RegisterRules(IConsensus consensus)
        {
            consensus.FullValidationRules = new List<IFullValidationConsensusRule>()
            {
                new SmartContractFormatRule(this.callDataSerializer)
            };
        }
    }
}