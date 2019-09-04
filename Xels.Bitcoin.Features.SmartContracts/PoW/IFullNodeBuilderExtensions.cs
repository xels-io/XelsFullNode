using Microsoft.Extensions.DependencyInjection;
using NBitcoin;
using Xels.Bitcoin.Builder;
using Xels.Bitcoin.Configuration.Logging;
using Xels.Bitcoin.Consensus;
using Xels.Bitcoin.Consensus.Rules;
using Xels.Bitcoin.Features.Consensus;
using Xels.Bitcoin.Features.Consensus.CoinViews;
using Xels.Bitcoin.Features.Consensus.Rules;
using Xels.Bitcoin.Features.MemoryPool;
using Xels.Bitcoin.Features.Miner;
using Xels.Bitcoin.Features.Miner.Controllers;
using Xels.Bitcoin.Features.Miner.Interfaces;
using Xels.Bitcoin.Features.RPC;
using Xels.Bitcoin.Features.SmartContracts.PoS;
using Xels.Bitcoin.Features.SmartContracts.Wallet;
using Xels.Bitcoin.Mining;

namespace Xels.Bitcoin.Features.SmartContracts.PoW
{
    public static partial class IFullNodeBuilderExtensions
    {
        /// <summary>
        /// Configures the node with the smart contract proof of work consensus model.
        /// </summary>
        public static IFullNodeBuilder UseSmartContractPowConsensus(this IFullNodeBuilder fullNodeBuilder)
        {
            LoggingConfiguration.RegisterFeatureNamespace<ConsensusFeature>("consensus");

            fullNodeBuilder.ConfigureFeature(features =>
            {
                features
                .AddFeature<ConsensusFeature>()
                .DependOn<SmartContractFeature>()
                .FeatureServices(services =>
                {
                    services.AddSingleton<ConsensusOptions, ConsensusOptions>();
                    services.AddSingleton<DBreezeCoinView>();
                    services.AddSingleton<ICoinView, CachedCoinView>();
                    services.AddSingleton<ConsensusController>();

                    services.AddSingleton<PowConsensusRuleEngine>();
                    services.AddSingleton<IRuleRegistration, SmartContractPowRuleRegistration>();
                    services.AddSingleton<IConsensusRuleEngine>(f =>
                    {
                        var concreteRuleEngine = f.GetService<PowConsensusRuleEngine>();
                        var ruleRegistration = f.GetService<IRuleRegistration>();

                        return new DiConsensusRuleEngine(concreteRuleEngine, ruleRegistration);
                    });
                });
            });

            return fullNodeBuilder;
        }

        /// <summary>
        /// Adds mining to the smart contract node.
        /// <para>We inject <see cref="IPowMining"/> with a smart contract block provider and definition.</para>
        /// </summary>
        public static IFullNodeBuilder UseSmartContractPowMining(this IFullNodeBuilder fullNodeBuilder)
        {
            LoggingConfiguration.RegisterFeatureNamespace<MiningFeature>("mining");

            fullNodeBuilder.ConfigureFeature(features =>
            {
                features
                    .AddFeature<MiningFeature>()
                    .DependOn<MempoolFeature>()
                    .DependOn<RPCFeature>()
                    .DependOn<SmartContractWalletFeature>()
                    .FeatureServices(services =>
                    {
                        services.AddSingleton<IPowMining, PowMining>();
                        services.AddSingleton<IBlockProvider, SmartContractBlockProvider>();
                        services.AddSingleton<BlockDefinition, SmartContractBlockDefinition>();
                        services.AddSingleton<IBlockBufferGenerator, BlockBufferGenerator>();
                        services.AddSingleton<MiningController>();
                        services.AddSingleton<MiningRpcController>();
                        services.AddSingleton<MinerSettings>();
                    });
            });

            return fullNodeBuilder;
        }
    }
}
