using System;

namespace Xels.Bitcoin.Features.Miner
{
    public class MinerException : Exception
    {
        public MinerException(string message) : base(message)
        {
        }
    }
}
