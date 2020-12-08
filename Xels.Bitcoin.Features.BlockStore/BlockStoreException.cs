using System;

namespace Xels.Bitcoin.Features.BlockStore
{
    public class BlockStoreException : Exception
    {
        public BlockStoreException(string message) : base(message)
        {
        }
    }
}
