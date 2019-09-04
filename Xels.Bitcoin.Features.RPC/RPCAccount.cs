using NBitcoin;

namespace Xels.Bitcoin.Features.RPC
{
    public class RPCAccount
    {
        public Money Amount { get; set; }
        public string AccountName { get; set; }
    }
}