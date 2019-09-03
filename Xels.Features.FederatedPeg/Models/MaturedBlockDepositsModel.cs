using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Xels.Bitcoin.Features.Wallet.Models;
using Xels.Features.FederatedPeg.Interfaces;
using Xels.Features.FederatedPeg.SourceChain;

namespace Xels.Features.FederatedPeg.Models
{
    /// <summary>
    /// When a block matures, an instance of this class is created and passed on to the target chain.
    /// If there are no deposits, we still need to send an empty list with corresponding block (height
    /// and hash) so that the target node knows that block has been seen and dealt with.
    /// </summary>
    public class MaturedBlockDepositsModel : RequestModel
    {
        public MaturedBlockDepositsModel(MaturedBlockInfoModel maturedBlockInfo, IReadOnlyList<IDeposit> deposits)
        {
            this.BlockInfo = maturedBlockInfo;
            this.Deposits = deposits;
        }

        [Required(ErrorMessage = "A list of deposits is required")]
        [JsonConverter(typeof(ConcreteConverter<List<Deposit>>))]
        public IReadOnlyList<IDeposit> Deposits { get; set; }

        [Required(ErrorMessage = "A block is required")]
        [JsonConverter(typeof(ConcreteConverter<MaturedBlockInfoModel>))]
        public IMaturedBlockInfo BlockInfo { get; set; }
    }
}
