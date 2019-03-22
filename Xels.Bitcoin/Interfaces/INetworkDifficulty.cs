using NBitcoin;

namespace Xels.Bitcoin.Interfaces
{
    public interface INetworkDifficulty
    {
        Target GetNetworkDifficulty();
    }
}
