using Microsoft.Extensions.DependencyInjection;
using Xels.Bitcoin.Builder;
using Xels.Bitcoin.Configuration.Logging;
using Xels.Bitcoin.Consensus;
using Xels.Bitcoin.Consensus.Rules;
using Xels.Bitcoin.Features.Consensus;
using Xels.Bitcoin.Features.Consensus.CoinViews;
using Xels.Bitcoin.Features.Consensus.Interfaces;
using Xels.Bitcoin.Features.Consensus.Rules;
using Xels.Bitcoin.Features.MemoryPool;
using Xels.Bitcoin.Features.Miner;
using Xels.Bitcoin.Features.Miner.Controllers;
using Xels.Bitcoin.Features.Miner.Interfaces;
using Xels.Bitcoin.Features.RPC;
using Xels.Bitcoin.Features.SmartContracts.PoW;
using Xels.Bitcoin.Features.SmartContracts.Wallet;
using Xels.Bitcoin.Mining;

namespace Xels.Bitcoin.Features.SmartContracts.PoS
{
    public static partial class IFullNodeBuilderExtensions
    {

        /// <summary>
        /// Configures the node with the smart contract proof of stake consensus model.
        /// </summary>
        public static IFullNodeBuilder UseSmartContractPosConsensus(this IFullNodeBuilder fullNodeBuilder)
        {
            LoggingConfiguration.RegisterFeatureNamespace<ConsensusFeature>("consensus");

            fullNodeBuilder.ConfigureFeature(features =>
            {
                features
                    .AddFeature<ConsensusFeature>()
                    .FeatureServices(services =>
                    {
                        services.AddSingleton<DBreezeCoinView>();
                        services.AddSingleton<ICoinView, CachedCoinView>();
                        services.AddSingleton<StakeChainStore>().AddSingleton<IStakeChain, StakeChainStore>(provider => provider.GetService<StakeChainStore>());
                        services.AddSingleton<IStakeValidator, StakeValidator>();
                        services.AddSingleton<ConsensusController>();

                        services.AddSingleton<PosConsensusRuleEngine>();
                        services.AddSingleton<IConsensusRuleEngine>(f =>
                        {
                            var concreteRuleEngine = f.GetService<PosConsensusRuleEngine>();
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
        public static IFullNodeBuilder UseSmartContractPosPowMining(this IFullNodeBuilder fullNodeBuilder)
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
                        services.AddSingleton<BlockDefinition, SmartContractPosPowBlockDefinition>();
                        services.AddSingleton<IBlockBufferGenerator, BlockBufferGenerator>();
                        services.AddSingleton<MiningRpcController>();
                        services.AddSingleton<MiningController>();
                        services.AddSingleton<StakingController>();
                        services.AddSingleton<StakingRpcController>();
                        services.AddSingleton<MinerSettings>();
                    });
            });

            return fullNodeBuilder;
        }
    }
}
