using System;

namespace Xels.Bitcoin.Features.Consensus.ProvenBlockHeaders
{
    public class ProvenBlockHeaderException : Exception
    {
        public ProvenBlockHeaderException(string message) : base(message)
        {
        }
    }
}
