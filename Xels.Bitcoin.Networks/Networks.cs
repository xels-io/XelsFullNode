using NBitcoin;

namespace Xels.Bitcoin.Networks
{
    public static class Networks
    {
        public static NetworksSelector Bitcoin
        {
            get
            {
                return new NetworksSelector(() => new BitcoinMain(), () => new BitcoinTest(), () => new BitcoinRegTest());
            }
        }

        public static NetworksSelector Xels
        {
            get
            {
                return new NetworksSelector(() => new XelsMain(), () => new XelsTest(), () => new XelsRegTest());
            }
        }
    }
}
