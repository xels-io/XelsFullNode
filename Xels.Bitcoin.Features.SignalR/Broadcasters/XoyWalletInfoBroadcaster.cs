using Microsoft.Extensions.Logging;
using Xels.Bitcoin.AsyncWork;
using Xels.Bitcoin.Features.Wallet.Services;
using Xels.Bitcoin.Utilities;

namespace Xels.Bitcoin.Features.SignalR.Broadcasters
{
    /// <summary>
    /// Broadcasts current staking information to SignalR clients
    /// </summary>
    public class XoyWalletInfoBroadcaster : WalletInfoBroadcaster
    {
        public XoyWalletInfoBroadcaster(
            ILoggerFactory loggerFactory,
            IWalletService walletService,
            IAsyncProvider asyncProvider,
            INodeLifetime nodeLifetime,
            EventsHub eventsHub)
            : base(loggerFactory, walletService, asyncProvider, nodeLifetime, eventsHub, true)
        {
        }
    }
}