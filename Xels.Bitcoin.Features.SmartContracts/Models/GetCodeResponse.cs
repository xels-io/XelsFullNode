using Newtonsoft.Json;

namespace Xels.Bitcoin.Features.SmartContracts.Models
{
    public sealed class GetCodeResponse
    {
        [JsonProperty(PropertyName = "bytecode")]
        public string Bytecode { get; set; }

        [JsonProperty(PropertyName = "csharp")]
        public string CSharp { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}