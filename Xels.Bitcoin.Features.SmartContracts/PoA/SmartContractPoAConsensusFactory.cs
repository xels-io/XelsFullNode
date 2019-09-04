using NBitcoin;
using Xels.Bitcoin.Features.PoA;

namespace Xels.Bitcoin.Features.SmartContracts.PoA
{
    public class SmartContractPoAConsensusFactory : PoAConsensusFactory
    {
        /// <inheritdoc />
        public override BlockHeader CreateBlockHeader()
        {
            return new SmartContractPoABlockHeader();
        }
    }

    public class SmartContractCollateralPoAConsensusFactory : CollateralPoAConsensusFactory
    {
        /// <inheritdoc />
        public override BlockHeader CreateBlockHeader()
        {
            return new SmartContractPoABlockHeader();
        }
    }
}
