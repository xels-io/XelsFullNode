using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NBitcoin;
using Xels.Bitcoin.Base;
using Xels.Bitcoin.Features.Consensus.CoinViews;
using Xels.Bitcoin.Interfaces;
using Xels.Bitcoin.Utilities;

namespace Xels.Bitcoin.Features.Consensus
{
    /// <summary>
    /// A class that provides the ability to query consensus elements.
    /// </summary>
    public class ConsensusQuery : IGetUnspentTransaction, INetworkDifficulty
    {
        private readonly ICoinView coinView;
        private readonly ILogger logger;
        private readonly IChainState chainState;
        private readonly Network network;

        public ConsensusQuery(
            ICoinView coinView,
            IChainState chainState,
            Network network,
            ILoggerFactory loggerFactory)
        {
            this.coinView = coinView;
            this.chainState = chainState;
            this.network = network;
            this.logger = loggerFactory.CreateLogger(this.GetType().FullName);
        }

        /// <inheritdoc />
        public async Task<UnspentOutputs> GetUnspentTransactionAsync(uint256 trxid)
        {
            FetchCoinsResponse response = this.coinView.FetchCoins(new[] { trxid });

            UnspentOutputs unspentOutputs = response.UnspentOutputs.FirstOrDefault();
            return unspentOutputs;
        }

        /// <inheritdoc/>
        public Target GetNetworkDifficulty()
        {
            return this.chainState.ConsensusTip?.GetWorkRequired(this.network.Consensus);
        }
    }
}