using NBitcoin;

namespace Xels.Features.FederatedPeg.Interfaces
{
    public interface ICounterChainSettings
    {
        /// <summary>
        /// The API host used to communicate with node on the counter chain.
        /// </summary>
        string CounterChainApiHost { get; }

        /// <summary>
        /// The API port used to communicate with node on the counter chain.
        /// </summary>
        int CounterChainApiPort { get; }

        /// <summary>
        /// The chain that we are connecting to from this node.
        /// E.g. if this is a Cirrus sidechain gateway node, the counter-chain would be Xels and vice versa.
        /// </summary>
        Network CounterChainNetwork { get; }
    }
}
