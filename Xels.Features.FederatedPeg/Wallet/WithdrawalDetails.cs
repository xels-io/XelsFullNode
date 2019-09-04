using NBitcoin;
using Newtonsoft.Json;
using Xels.Bitcoin.Utilities.JsonConverters;

namespace Xels.Features.FederatedPeg.Wallet
{
    public class WithdrawalDetails
    {
        /// <summary>
        /// The deposit ID that the withdrawal is fulfilling.
        /// </summary>
        [JsonProperty(PropertyName = "depositId", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(UInt256JsonConverter))]
        public uint256 MatchingDepositId { get; set; }

        /// <summary>
        /// The amount of money being sent in this withdrawal.
        /// </summary>
        [JsonProperty(PropertyName = "amount")]
        [JsonConverter(typeof(MoneyJsonConverter))]
        public Money Amount { get; set; }

        /// <summary>
        /// The address to withdraw to.
        /// </summary>
        [JsonProperty(PropertyName = "targetAddress")]
        public string TargetAddress { get; set; }
    }
}
