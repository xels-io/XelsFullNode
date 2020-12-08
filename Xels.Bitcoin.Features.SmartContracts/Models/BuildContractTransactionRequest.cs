using System.ComponentModel.DataAnnotations;
using Xels.Bitcoin.Features.Wallet.Models;
using Xels.Bitcoin.Features.Wallet.Validations;

namespace Xels.Bitcoin.Features.SmartContracts.Models
{
    public class ScTxFeeEstimateRequest : TxFeeEstimateRequest
    {
        [Required(ErrorMessage = "Sender is required.")]
        [IsBitcoinAddress]
        public string Sender { get; set; }
    }

    public class BuildContractTransactionRequest : BuildTransactionRequest
    {
        [Required(ErrorMessage = "Sender is required.")]
        [IsBitcoinAddress]
        public string Sender { get; set; }
    }
}