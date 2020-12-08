using System.ComponentModel.DataAnnotations;

namespace Xels.Bitcoin.Features.PoA.Voting
{
    public class HexPubKeyModel
    {
        [Required(AllowEmptyStrings = false)]
        public string PubKeyHex { get; set; }
    }
}
