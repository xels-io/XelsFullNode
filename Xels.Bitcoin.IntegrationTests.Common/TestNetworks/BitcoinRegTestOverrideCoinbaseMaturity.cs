﻿using System;
using Xels.Bitcoin.Networks;

namespace Xels.Bitcoin.IntegrationTests.Common.TestNetworks
{
    public class BitcoinRegTestOverrideCoinbaseMaturity : BitcoinRegTest
    {
        public BitcoinRegTestOverrideCoinbaseMaturity(int coinbaseMaturity) : base()
        {
            this.Name = Guid.NewGuid().ToString("N").Substring(0, 7);
            this.Consensus.CoinbaseMaturity = coinbaseMaturity;
        }
    }
}
