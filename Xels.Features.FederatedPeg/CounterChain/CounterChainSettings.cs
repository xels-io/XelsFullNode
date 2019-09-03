﻿using NBitcoin;
using Xels.Bitcoin.Configuration;
using Xels.Bitcoin.Utilities;
using Xels.Features.FederatedPeg.Interfaces;

namespace Xels.Features.FederatedPeg.CounterChain
{
    public class CounterChainSettings : ICounterChainSettings
    {
        /// <summary>
        /// Command-line argument to set counter chain host.
        /// </summary>
        public const string CounterChainApiHostParam = "counterchainapihost";

        /// <summary>
        /// Command-line argument to set counter chain port.
        /// </summary>
        public const string CounterChainApiPortParam = "counterchainapiport";

        /// <inheritdoc />
        public string CounterChainApiHost { get; set; }

        /// <inheritdoc />
        public int CounterChainApiPort { get; set; }

        /// <inheritdoc />
        public Network CounterChainNetwork { get; set; }

        public CounterChainSettings(NodeSettings nodeSettings, Network counterChainNetwork)
        {
            Guard.NotNull(nodeSettings, nameof(nodeSettings));

            TextFileConfiguration configReader = nodeSettings.ConfigReader;

            this.CounterChainApiHost = configReader.GetOrDefault(CounterChainApiHostParam, "localhost");
            this.CounterChainApiPort = configReader.GetOrDefault(CounterChainApiPortParam, counterChainNetwork.DefaultAPIPort);
            this.CounterChainNetwork = counterChainNetwork;
        }
    }
}
