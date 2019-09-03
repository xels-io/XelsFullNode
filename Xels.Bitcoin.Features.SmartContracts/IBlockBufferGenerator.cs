using Xels.Bitcoin.Features.Miner;

namespace Xels.Bitcoin.Features.SmartContracts
{
    /// <summary>
    /// Works out how much room needs to be left at the start of the block as a buffer for protocol transactions.
    /// </summary>
    public interface IBlockBufferGenerator
    {
        BlockDefinitionOptions GetOptionsWithBuffer(BlockDefinitionOptions options);
    }
}
