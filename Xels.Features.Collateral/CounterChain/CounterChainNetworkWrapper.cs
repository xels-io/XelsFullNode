using NBitcoin;

namespace Xels.Features.Collateral.CounterChain
{
    /// <summary>
    /// Allows us to inject information about the counter chain.
    /// </summary>
    public class CounterChainNetworkWrapper
    {
        /// <summary>
        /// The "other" network that we are connecting to from this node.
        /// E.g. if this is a Xoy sidechain gateway node, the counter-chain would be Xels and vice versa.
        /// </summary>
        public Network CounterChainNetwork { get; }

        public CounterChainNetworkWrapper(Network counterChainNetwork)
        {
            this.CounterChainNetwork = counterChainNetwork;
        }
    }
}
