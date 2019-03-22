using Xels.Bitcoin.Features.Miner;

namespace Xels.Bitcoin.Features.SmartContracts
{
    public interface IBlockBufferGenerator
    {
        BlockDefinitionOptions GetOptionsWithBuffer(BlockDefinitionOptions options);
    }
}
