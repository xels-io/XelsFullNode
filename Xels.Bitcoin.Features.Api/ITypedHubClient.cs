using System.Threading.Tasks;

namespace Xels.Bitcoin.Features.Api
{
    public interface ITypedHubClient
    {
        Task BroadcastMessage(string type, string payload);
    }
}
