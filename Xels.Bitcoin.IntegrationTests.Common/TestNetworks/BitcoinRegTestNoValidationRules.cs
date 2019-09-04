using System;
using Xels.Bitcoin.Networks;

namespace Xels.Bitcoin.IntegrationTests.Common.TestNetworks
{
    public sealed class BitcoinRegTestNoValidationRules : BitcoinRegTest
    {
        public BitcoinRegTestNoValidationRules()
        {
            this.Name = Guid.NewGuid().ToString();
        }
    }
}
