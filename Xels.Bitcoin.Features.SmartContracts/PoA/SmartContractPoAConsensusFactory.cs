﻿using NBitcoin;

namespace Xels.Bitcoin.Features.SmartContracts.PoA
{
    public class SmartContractPoAConsensusFactory : ConsensusFactory
    {
        /// <inheritdoc />
        public override Block CreateBlock()
        {
            return new Block(this.CreateBlockHeader());
        }

        /// <inheritdoc />
        public override BlockHeader CreateBlockHeader()
        {
            return new SmartContractPoABlockHeader();
        }
    }
}
