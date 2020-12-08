using NBitcoin;

namespace Xels.SmartContracts.CLR
{
    public interface IAddressGenerator
    {
        uint160 GenerateAddress(uint256 seed, ulong nonce);
    }
}