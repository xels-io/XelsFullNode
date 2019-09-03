using NBitcoin;
using NBitcoin.Networks;
using Xels.Bitcoin.Networks;

namespace Xels.Bitcoin.Tests.Common
{
    public static class KnownNetworks
    {
        public static Network Main => NetworkRegistration.GetNetwork("Main") ?? NetworkRegistration.Register(new BitcoinMain());

        public static Network TestNet => NetworkRegistration.GetNetwork("TestNet") ?? NetworkRegistration.Register(new BitcoinTest());

        public static Network RegTest => NetworkRegistration.GetNetwork("RegTest") ?? NetworkRegistration.Register(new BitcoinRegTest());

        public static Network XelsMain => NetworkRegistration.GetNetwork("XelsMain") ?? NetworkRegistration.Register(new XelsMain());

        public static Network XelsTest => NetworkRegistration.GetNetwork("XelsTest") ?? NetworkRegistration.Register(new XelsTest());

        public static Network XelsRegTest => NetworkRegistration.GetNetwork("XelsRegTest") ?? NetworkRegistration.Register(new XelsRegTest());
    }
}